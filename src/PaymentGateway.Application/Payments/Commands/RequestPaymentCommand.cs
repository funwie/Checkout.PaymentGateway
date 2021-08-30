using MediatR;
using System;
using Checkout.Functional;
using PaymentGateway.Application.Payments.Commands.Validators;

namespace PaymentGateway.Application.Payments.Commands
{
    public class RequestPaymentCommand : IRequest<Result<PaymentResult, ValidationError>>
    {
        public Payment Payment { get; private set; }
        public Guid MerchantId { get; private set; }
        public string RequestIdempotencyKey { get; private set; }

        public RequestPaymentCommand(Payment payment, Guid merchantId, string requestIdempotencyKey)
        {
            Payment = payment;
            MerchantId = merchantId;
            RequestIdempotencyKey = requestIdempotencyKey;
        }
    }
}
