using System;

namespace PaymentGateway.Application.Payments.Queries
{
    public class AcquirerProjection
    {
        public string Reference { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
