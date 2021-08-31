using PaymentGateway.SeedWork;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Domain.ValueObjects
{
    public class AcquirerResult : ValueObject<AcquirerResult>
    {
        public string Name { get; }
        public decimal Amount { get; }
        public bool? Approved { get; }
        public string Reference { get; }
        public string Status { get; }
        public DateTime PerformedOn { get; }

        public AcquirerResult(string name, bool? approved, string reference, string status, DateTime performedOn, decimal amount)
        {
            Name = name;
            Approved = approved;
            Reference = reference;
            Status = status;
            PerformedOn = performedOn;
            Amount = amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Approved;
            yield return Reference;
            yield return Status;
            yield return PerformedOn;
            yield return Amount;
        }
    }
}
