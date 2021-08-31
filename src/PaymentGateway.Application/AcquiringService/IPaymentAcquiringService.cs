using PaymentGateway.Domain.AggregateRoot;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Functional;

namespace PaymentGateway.Application.AcquiringService
{
    public interface IPaymentAcquiringService
    {
        Task<Result<AcquirerResponse, AcquirePaymentError>> AcquirePayment(Payment payment, CancellationToken cancellationToken);
    }
}
