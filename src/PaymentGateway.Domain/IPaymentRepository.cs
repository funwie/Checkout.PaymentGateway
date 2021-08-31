using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.SeedWork;
using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain
{
    public interface IPaymentRepository : IRepository<Payment, Guid>
    {
        Task Add(Payment payment);
        Task Update(Payment payment);
        Task<Payment> GetById(Guid id, CancellationToken cancellationToken);
        
        // Does not belong here. Added to facilitate dupe merchant checking for now
        Task<Merchant> GetMerchantById(Guid merchantId, CancellationToken cancellationToken);
    }
}
