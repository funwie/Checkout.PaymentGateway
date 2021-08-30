namespace PaymentGateway.Contract.Models
{
    public class Card
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public Address BillingAddress { get; set; }
    }
}
