using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.SeedWork;
using System;

namespace PaymentGateway.Domain.Entities
{
    public class Card : Entity<Guid>
    {
        public string FullName { get;}
        public string CompanyName { get; }
        public string CardNumber { get; }
        public string Cvv { get; }
        public string ExpiryMonth { get; }
        public string ExpiryYear { get; }
        public Address BillingAddress { get; }

        public string MaskedCardNumber => string.IsNullOrWhiteSpace(CardNumber) ? "" : $"************{CardNumber.Substring(CardNumber.Length - 4)}";
        public string MaskedCvv => string.IsNullOrWhiteSpace(Cvv) ? "" : $"*{Cvv.Substring(Cvv.Length - 1)}";

        public Card(Guid id, string fullName, string companyName, string cardNumber, 
                    string cvv, string expiryMonth, string expiryYear, Address billingAddress)
        {
            base.Id = id;
            FullName = fullName;
            CompanyName = companyName;
            CardNumber = cardNumber;
            Cvv = cvv;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            BillingAddress = billingAddress;
        }
    }
}
