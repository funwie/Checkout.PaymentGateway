using System;
using Checkout.Functional;
using MediatR;

namespace PaymentGateway.Application.Payments.Queries
{
    public class RetrievePaymentQuery : IRequest<Result<PaymentProjection, RetrievePaymentQueryError>>
    {
        public Guid PaymentId { get; private set; }
        public Guid MerchantId { get; private set; }

        public RetrievePaymentQuery(Guid paymentId, Guid merchantId)
        {
            PaymentId = paymentId;
            MerchantId = merchantId;
        }
    }
}
