using PaymentGateway.SeedWork;
using System;

namespace PaymentGateway.Domain.Entities
{
    public class Merchant : Entity<Guid>
    {
        public string Name { get; private set; }

        public Merchant(Guid id, string name)
        {
            base.Id = id;
            Name = name;
        }
    }
}
