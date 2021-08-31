using System;

namespace PaymentGateway.Contract.Responses
{
    public class AcquirerResponse
    {
        public string Reference { get; set; }
        public DateTime? PerformedOn { get; set; }
    }
}
