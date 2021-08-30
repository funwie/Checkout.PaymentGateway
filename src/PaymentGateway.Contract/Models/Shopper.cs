namespace PaymentGateway.Contract.Models
{
    public class Shopper
    {
        public string Name { get; set; }
        public string Reference { get;set; }
        public Contact Contact { get; set; }
        public Address ShippingAddress { get; set; }
    }
}
