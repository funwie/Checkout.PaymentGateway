using System;

namespace PaymentGateway.Application.Payments.Queries
{
    public class TransactionProjection
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public bool? Approved { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}