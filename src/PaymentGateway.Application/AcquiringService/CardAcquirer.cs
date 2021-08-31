using CardPaymentAcquirerBank.Sdk;
using CardPaymentAcquirerBank.Sdk.Models;
using PaymentGateway.Domain.AggregateRoot;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.AcquiringService
{
    public class CardAcquirer : IAcquirer
    {
        private readonly ICardAcquirerBankClient _acquirerBankClient;

        public CardAcquirer(ICardAcquirerBankClient acquirerBankClient)
        {
            _acquirerBankClient = acquirerBankClient;
        }

        public async Task<AcquirerResponse> AcquirePayment(Payment payment, CancellationToken cancellationToken)
        {
            var request = CreateCardPaymentAcquirerRequest(payment);
            var acquirerResponse = await _acquirerBankClient.AcquirePayment(request, cancellationToken);
            return Map(acquirerResponse);
        }

        private static CardPaymentAcquirerRequest CreateCardPaymentAcquirerRequest(Payment payment)
        {
            var card = payment.Source.Card;
            var bankAccount = payment.Destination.BankAccount;

            return new CardPaymentAcquirerRequest
            {
                Amount = payment.Amount,
                Type = payment.Type.ToString(),
                Reference = payment.Reference,
                DestinationBankAccount = new BankAccount
                {
                    SortCode = bankAccount.SortCode,
                    AccountNumber = bankAccount.AccountNumber,
                    BankName = bankAccount.BankName
                },
                SourceCard = new Card
                {
                    FullName = card.FullName,
                    CompanyName = card.CompanyName,
                    CardNumber = card.CardNumber,
                    Cvv = card.Cvv,
                    ExpiryMonth = card.ExpiryMonth,
                    ExpiryYear = card.ExpiryYear,
                    BillingAddress = new Address
                    {
                        HouseNumber = card.BillingAddress?.HouseNumber,
                        AddressLine1 = card.BillingAddress?.Line1,
                        AddressLine2 = card.BillingAddress?.Line2,
                        City = card.BillingAddress?.City,
                        Postcode = card.BillingAddress?.Postcode,
                        Country = card.BillingAddress?.Country
                    }
                }

            };
        }

        private static AcquirerResponse Map(CardPaymentAcquirerResponse acquirerResponse)
        {
            if (acquirerResponse is null) return null;

            return new AcquirerResponse
            {
                Name = acquirerResponse.Name,
                Amount = acquirerResponse.Amount,
                Approved = acquirerResponse.Approved,
                Reference = acquirerResponse.TransactionReference,
                Status = acquirerResponse.Status,
                PerformedOn = acquirerResponse.PerformedOn
            };
        }
    }
}