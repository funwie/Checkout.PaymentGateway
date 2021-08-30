using System.Collections.Generic;

namespace PaymentGateway.SeedWork
{
    public abstract class AggregateRoot<TIdentifier> : Entity<TIdentifier> 
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        public long Version { get; protected set; }

        protected void AddDomainEvent(IDomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }
        public void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}
