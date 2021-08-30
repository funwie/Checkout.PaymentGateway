using PaymentGateway.Contract.Models;

namespace PaymentGateway.Contract.Responses
{
    public class CardResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string MaskedCardNumber { get; set; }
        public string MaskedCvv { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public Address BillingAddress { get; set; }
    }
}
