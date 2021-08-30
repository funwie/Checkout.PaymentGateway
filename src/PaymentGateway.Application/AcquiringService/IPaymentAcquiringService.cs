using PaymentGateway.Domain.AggregateRoot;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.AcquiringService
{
    public interface IPaymentAcquiringService
    {
        Task<AcquirerResponse> AcquirePayment(Payment payment, CancellationToken cancellationToken);
    }
}
