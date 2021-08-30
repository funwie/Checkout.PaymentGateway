using System;

namespace CardPaymentAcquirerBank.Sdk.Models
{
    public class CardPaymentAcquirerResponse
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public bool? Approved { get; set; }
        public string Status { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
