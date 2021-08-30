namespace PaymentGateway.Contract.Models
{
    public class Address
    {
        public string HouseNumber { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}
