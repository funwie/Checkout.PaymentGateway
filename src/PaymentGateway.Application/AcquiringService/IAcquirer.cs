using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.AcquiringService
{
    public interface IAcquirer
    {
        Task<AcquirerResponse> AcquirePayment(Domain.AggregateRoot.Payment payment, CancellationToken cancellationToken);
    }
}
