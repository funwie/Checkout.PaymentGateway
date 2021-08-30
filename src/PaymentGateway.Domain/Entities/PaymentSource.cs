using PaymentGateway.Domain.Enumerations;
using PaymentGateway.SeedWork;
using System;

namespace PaymentGateway.Domain.Entities
{
    public class PaymentSource : Entity<Guid>
    {
        public PaymentSourceType Type { get; }
        public Card Card { get; }
        public Guid ShopperId { get; }

        public PaymentSource(Guid id, PaymentSourceType type, Card card, Guid shopperId)
        {
            base.Id = id;
            Type = type;
            Card = card;
            ShopperId = shopperId;
        }
    }
}
