using CardPaymentAcquirerBank.Sdk;
using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Application.AcquiringService
{
    public class AcquirerFactory : IAcquirerFactory
    {
        private readonly ICardAcquirerBankClient _cardAcquirerBankClient;

        public AcquirerFactory(ICardAcquirerBankClient cardAcquirerBankClient)
        {
            _cardAcquirerBankClient = cardAcquirerBankClient;
        }

        public IAcquirer CreateAcquirer(PaymentSourceType paymentSourceType)
        {
            return paymentSourceType switch
            {
                PaymentSourceType.Card => new CardAcquirer(_cardAcquirerBankClient),
                _ => throw new UnsupportedAcquirerException()
            };
        }
    }
}
