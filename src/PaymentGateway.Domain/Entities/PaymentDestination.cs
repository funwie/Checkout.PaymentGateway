using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.SeedWork;
using System;
using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Domain.Entities
{
    public class PaymentDestination : Entity<Guid>
    {
        public PaymentDestinationType Type { get; }
        public BankAccount BankAccount { get; }
        public Guid MerchantId { get; }

        public PaymentDestination(Guid id, PaymentDestinationType type, BankAccount bankAccount, Guid merchantId)
        {
            base.Id = id;
            Type = type;
            BankAccount = bankAccount;
            MerchantId = merchantId;
        }
    }
}
