using FluentValidation;
using MediatR;
using PaymentGateway.Application.AcquiringService;
using PaymentGateway.Application.MerchantService;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Functional;
using PaymentGateway.Application.Payments.Commands.Validation;
using BankAccount = PaymentGateway.Application.MerchantService.BankAccount;

namespace PaymentGateway.Application.Payments.Commands
{
    public class RequestPaymentCommandHandler :  IRequestHandler<RequestPaymentCommand, Result<PaymentResult, ValidationError>>
    {
        private readonly IPaymentAcquiringService _paymentAcquiringService;
        private readonly IMerchantService _merchantService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IValidator<RequestPaymentCommand> _validator;

        public RequestPaymentCommandHandler(IPaymentAcquiringService paymentAcquiringService, 
                                            IMerchantService merchantService, 
                                            IPaymentRepository paymentRepository,
                                            IValidator<RequestPaymentCommand> validator)
        {
            _paymentAcquiringService = paymentAcquiringService;
            _merchantService = merchantService;
            _paymentRepository = paymentRepository;
            _validator = validator;
        }

        public async Task<Result<PaymentResult, ValidationError>> Handle(RequestPaymentCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid is false)
                return ValidationErrors(validationResult);

            var payment = request.Payment;
            var paymentAggregate = new Domain.AggregateRoot.Payment(payment.Id, 
                                                                    payment.Amount, 
                                                                    payment.Currency,
                                                                    payment.Type, 
                                                                    payment.Description, 
                                                                    payment.Reference);


            // The section exist just as a way to fake getting merchant's info for the payment request
            // This will actually go to the merchant service to get the destination account and billing description
            // There is assumption here that this fake service always returns the merchant
            var merchant = await _paymentRepository.GetMerchantById(request.MerchantId, cancellationToken);
            MerchantResponse merchantResponse = null;
            if (merchant is null)
            {
                merchantResponse = await _merchantService.GetMerchant(request.MerchantId, cancellationToken);
                merchant = new Merchant(merchantResponse.Id, merchantResponse.Name);
            }
            
            var paymentDestination = new PaymentDestination(Guid.NewGuid(), PaymentDestinationType.BankAccount,Map(merchantResponse?.BankAccount), merchant.Id);

            paymentAggregate.AddSource(payment.Source);
            paymentAggregate.AddMerchant(merchant);
            paymentAggregate.AddShopper(payment.Shopper);
            paymentAggregate.AddDestination(paymentDestination);

            var acquirePaymentResult = await _paymentAcquiringService.AcquirePayment(paymentAggregate, cancellationToken);

            if (acquirePaymentResult.IsSuccess)
            {
                var acquirerResponse = acquirePaymentResult.Success;
                var acquirerResult = new AcquirerResult(acquirerResponse.Name, acquirerResponse.Approved,
                    acquirerResponse.Reference, acquirerResponse.Status, acquirerResponse.PerformedOn, acquirerResponse.Amount);

                paymentAggregate.Complete(acquirerResult);
            }
            
            await _paymentRepository.Add(paymentAggregate);

            return new PaymentResult
            {
                Approved = paymentAggregate.Approved,
                Status = paymentAggregate.Status, 
                PaymentId = paymentAggregate.Id.ToString()
            };
        }

        private static Domain.ValueObjects.BankAccount Map(BankAccount bankAccount)
        {
            if (bankAccount is null) return null;

            return new Domain.ValueObjects.BankAccount(bankAccount.SortCode, 
                                                       bankAccount.AccountNumber,
                                                       bankAccount.BankName);
        }

        private static ValidationError ValidationErrors(FluentValidation.Results.ValidationResult validationResult)
        {
            return new ValidationError
            {
                Type = "Invalid Payment Request",
                Errors = validationResult.Errors
            };
        }
    }
}
