using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Application.AcquiringService
{
    public interface IAcquirerFactory
    {
        IAcquirer CreateAcquirer(PaymentSourceType paymentSourceType);
    }
}