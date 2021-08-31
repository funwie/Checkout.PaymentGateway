using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Functional;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.Persistence;

namespace PaymentGateway.Application.Payments.Queries
{
    public class RetrievePaymentQueryHandler : IRequestHandler<RetrievePaymentQuery, Result<PaymentProjection, RetrievePaymentQueryError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<RetrievePaymentQueryHandler> _logger;

        public RetrievePaymentQueryHandler(IPaymentRepository paymentRepository, ILogger<RetrievePaymentQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<Result<PaymentProjection, RetrievePaymentQueryError>> Handle(RetrievePaymentQuery request, CancellationToken cancellationToken)
        {
            Payment retrievedPayment;
            try
            {
                retrievedPayment = await _paymentRepository.GetById(request.PaymentId, cancellationToken);
            }
            catch (RepositoryNotFoundException repositoryNotFoundException)
            {
                _logger.LogInformation(repositoryNotFoundException, "Payment was not found {paymentId}", request.PaymentId);
                return RetrievePaymentQueryError.PaymentNotFound;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to retrieve payment {paymentId}", request.PaymentId);
                return RetrievePaymentQueryError.FailedToRetrievePayment;
            }

            // This is just to simulate merchants are allowed to access only their payment
            // This will be done in the actual query after the merchant is authenticated
            if (request.MerchantId != retrievedPayment.Merchant?.Id) 
                return RetrievePaymentQueryError.PaymentNotFound;

            return Map(retrievedPayment);
        }

        private PaymentProjection Map(Payment payment)
        {
            if (payment is null) return null;

            return new PaymentProjection
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Currency = payment.Currency.ToString(),
                Type = payment.Type.ToString(),
                Approved = payment.Approved,
                Status = payment.Status,
                Reference = payment.Reference,
                Description = payment.Description,
                RequestedOn = payment.RequestedOn,
                Shopper = Map(payment.Shopper),
                Card = Map(payment.Source?.Card),
                Acquirer = Map(payment.AcquirerResult),
                Transactions = Map(payment.Transactions)
            };
        }

        private static ShopperProjection Map(Shopper shopper)
        {
            if (shopper is null) return null;

            return new ShopperProjection
            {
                Id = shopper.Id,
                Name = shopper.Name,
                Reference = shopper.Reference,
                Contact = new ContactProjection{Phone = shopper.Contact?.Phone, Email = shopper.Contact?.Email},
                ShippingAddress = new AddressProjection
                {
                    HouseNumber = shopper.ShippingAddress?.HouseNumber,
                    Line1 = shopper.ShippingAddress?.Line1,
                    Line2 = shopper.ShippingAddress?.Line2,
                    City = shopper.ShippingAddress?.City,
                    Postcode = shopper.ShippingAddress?.Postcode,
                    Country = shopper.ShippingAddress?.Country
                }
            };
        }

        private static CardProjection Map(Card card)
        {
            if (card is null) return null;

            return new CardProjection
            {
                Id = card.Id,
                FullName = card.FullName,
                CompanyName = card.CompanyName,
                CardNumber = card.CardNumber,
                Cvv = card.Cvv,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                MaskedCardNumber = card.MaskedCardNumber,
                MaskedCvv = card.MaskedCvv,
                BillingAddress = new AddressProjection
                {
                    HouseNumber = card.BillingAddress?.HouseNumber,
                    Line1 = card.BillingAddress?.Line1,
                    Line2 = card.BillingAddress?.Line2,
                    City = card.BillingAddress?.City,
                    Postcode = card.BillingAddress?.Postcode,
                    Country = card.BillingAddress?.Country
                }
            };
        }

        private static AcquirerProjection Map(AcquirerResult acquirerResult)
        {
            if (acquirerResult is null) return null;

            return new AcquirerProjection
            {
                Reference = acquirerResult.Reference,
                PerformedOn = acquirerResult.PerformedOn
            };
        }

        private static IEnumerable<TransactionProjection> Map(IEnumerable<Transaction> transactions)
        {
            return transactions.Select(transaction => new TransactionProjection
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Approved = transaction.Approved,
                Type = transaction.Type,
                Reference = transaction.Reference,
                PerformedOn = transaction.PerformedOn
            });
        }
    }
}
