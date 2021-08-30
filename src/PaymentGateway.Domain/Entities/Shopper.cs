using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.SeedWork;
using System;

namespace PaymentGateway.Domain.Entities
{
    public class Shopper : Entity<Guid>
    {
        public string Name { get; private set; }
        public string Reference { get; private set; }
        public Contact Contact { get; private set; }
        public Address ShippingAddress { get; private set; }

        public Shopper(Guid id, string name, string reference, Contact contact, Address shippingAddress)
        {
            base.Id = id;
            Name = name;
            Reference = reference;
            Contact = contact;
            ShippingAddress = shippingAddress;
        }
    }
}
