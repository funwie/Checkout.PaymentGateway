using System;

namespace PaymentGateway.Application.Payments.Queries
{
    public class CardProjection
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string MaskedCardNumber { get; set; }
        public string MaskedCvv { get; set; }
        public AddressProjection BillingAddress { get; set; }
    }
}
