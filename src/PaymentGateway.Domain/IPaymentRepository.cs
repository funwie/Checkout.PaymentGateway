using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.SeedWork;

namespace PaymentGateway.Domain
{
    public interface IPaymentRepository : IRepository<Payment, Guid>
    {
        Task Add(Payment payment);
        Task Update(Payment payment);
        Task<Payment> GetById(Guid id, CancellationToken cancellationToken);
    }
}
