using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Queries
{
    public class RetrievePaymentQueryHandler : IRequestHandler<RetrievePaymentQuery, PaymentProjection>
    {
        private readonly IPaymentRepository _paymentRepository;

        public RetrievePaymentQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentProjection> Handle(RetrievePaymentQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetById(request.PaymentId, cancellationToken);
            return Map(payment);
        }

        private PaymentProjection Map(Domain.AggregateRoot.Payment payment)
        {
            if (payment is null) return null;

            return new PaymentProjection
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Currency = payment.Currency.ToString(),
                Type = payment.Type.ToString(),
                Approved = payment.Approved,
                Status = payment.Status,
                Reference = payment.Reference,
                Description = payment.Description,
                Shopper = Map(payment.Shopper),
                Card = Map(payment.Source?.Card),
                Acquirer = new AcquirerProjection(),
                Transactions = new List<TransactionProjection>()
            };
        }

        private static ShopperProjection Map(Shopper shopper)
        {
            if (shopper is null) return null;

            return new ShopperProjection
            {
                Id = shopper.Id,
                Name = shopper.Name,
                Reference = shopper.Reference,
                Contact = new ContactProjection{Phone = shopper.Contact?.Phone, Email = shopper.Contact?.Email},
                ShippingAddress = new AddressProjection
                {
                    HouseNumber = shopper.ShippingAddress?.HouseNumber,
                    Line1 = shopper.ShippingAddress?.Line1,
                    Line2 = shopper.ShippingAddress?.Line2,
                    City = shopper.ShippingAddress?.City,
                    Postcode = shopper.ShippingAddress?.Postcode,
                    Country = shopper.ShippingAddress?.Country
                }
            };
        }

        private static CardProjection Map(Card card)
        {
            if (card is null) return null;

            return new CardProjection
            {
                Id = card.Id,
                FullName = card.FullName,
                CompanyName = card.CompanyName,
                CardNumber = card.CardNumber,
                Cvv = card.Cvv,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                MaskedCardNumber = card.MaskedCardNumber,
                MaskedCvv = card.MaskedCvv,
                BillingAddress = new AddressProjection
                {
                    HouseNumber = card.BillingAddress?.HouseNumber,
                    Line1 = card.BillingAddress?.Line1,
                    Line2 = card.BillingAddress?.Line2,
                    City = card.BillingAddress?.City,
                    Postcode = card.BillingAddress?.Postcode,
                    Country = card.BillingAddress?.Country
                }
            };
        }

        private static IEnumerable<TransactionProjection> Map(IEnumerable<Transaction> transactions)
        {
            return transactions.Select(transaction => new TransactionProjection
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Approved = transaction.Approved,
                Type = transaction.Type,
                Reference = transaction.Reference,
                PerformedOn = transaction.PerformedOn
            });
        }
    }
}
