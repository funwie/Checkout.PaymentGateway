using System;

namespace PaymentGateway.Application.AcquiringService
{
    public class AcquirerResponse
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool? Approved { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
