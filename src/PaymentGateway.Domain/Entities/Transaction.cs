using PaymentGateway.SeedWork;
using System;

namespace PaymentGateway.Domain.Entities
{
    public class Transaction : Entity<Guid>
    {
        public decimal Amount { get; }
        public bool? Approved { get; }
        public string Type { get; }
        public string Reference { get; }
        public DateTime PerformedOn { get; }

        public Transaction(Guid id, decimal amount, bool? approved, string type, string reference, DateTime performedOn)
        {
            base.Id = id;
            Amount = amount;
            Approved = approved;
            Type = type;
            Reference = reference;
            PerformedOn = performedOn;
        }
    }
}
