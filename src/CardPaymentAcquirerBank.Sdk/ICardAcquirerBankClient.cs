using CardPaymentAcquirerBank.Sdk.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CardPaymentAcquirerBank.Sdk
{
    public interface ICardAcquirerBankClient
    {
        Task<CardPaymentAcquirerResponse> AcquirePayment(CardPaymentAcquirerRequest request, CancellationToken cancellationToken);
    }
}
