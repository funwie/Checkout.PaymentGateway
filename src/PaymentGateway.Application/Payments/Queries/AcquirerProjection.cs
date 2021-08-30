using System;

namespace PaymentGateway.Application.Payments.Queries
{
    public class AcquirerProjection
    {
        public string AuthorizationCode { get; set; }
        public string Reference { get; set; }
        public DateTime AcquiredOn { get; set; }
    }
}
