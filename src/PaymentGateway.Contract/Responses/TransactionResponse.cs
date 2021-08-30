using System;

namespace PaymentGateway.Contract.Responses
{
    public class TransactionResponse
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public bool? Approved { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
