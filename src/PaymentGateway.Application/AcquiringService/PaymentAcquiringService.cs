using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.Functional;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.AggregateRoot;

namespace PaymentGateway.Application.AcquiringService
{
    public class PaymentAcquiringService : IPaymentAcquiringService
    {
        private readonly IAcquirerFactory _acquirerFactory;
        private readonly ILogger<PaymentAcquiringService> _logger;

        public PaymentAcquiringService(IAcquirerFactory acquirerFactory, ILogger<PaymentAcquiringService> logger)
        {
            _acquirerFactory = acquirerFactory;
            _logger = logger;
        }

        public async Task<Result<AcquirerResponse, AcquirePaymentError>> AcquirePayment(Payment payment, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Acquiring payment {paymentId}", payment.Id);

            var paymentAcquire = _acquirerFactory.CreateAcquirer(payment.Source.Type);

            try
            {
                return await paymentAcquire.AcquirePayment(payment, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Acquire payment request failed {paymentId}", payment.Id);
                return AcquirePaymentError.RequestFailure;
            }
        }
    }
}