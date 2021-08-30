namespace PaymentGateway.SeedWork
{
    public interface IRepository<TAggregateRoot, in TIdentifier> where TAggregateRoot : AggregateRoot<TIdentifier>
    {
    }
}
