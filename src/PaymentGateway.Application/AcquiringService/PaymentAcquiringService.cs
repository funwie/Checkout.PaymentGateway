using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Domain.AggregateRoot;

namespace PaymentGateway.Application.AcquiringService
{
    public class PaymentAcquiringService : IPaymentAcquiringService
    {
        private readonly IAcquirerFactory _acquirerFactory;

        public PaymentAcquiringService(IAcquirerFactory acquirerFactory)
        {
            _acquirerFactory = acquirerFactory;
        }

        public async Task<AcquirerResponse> AcquirePayment(Payment payment, CancellationToken cancellationToken)
        {
            var paymentAcquire = _acquirerFactory.CreateAcquirer(payment.Source.Type);

            return await paymentAcquire.AcquirePayment(payment, cancellationToken);
        }
    }
}