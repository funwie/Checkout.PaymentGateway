using System;
using MediatR;

namespace PaymentGateway.Application.Payments.Queries
{
    public class RetrievePaymentQuery : IRequest<PaymentProjection>
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
