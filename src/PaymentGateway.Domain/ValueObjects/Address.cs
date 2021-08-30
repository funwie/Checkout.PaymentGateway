using PaymentGateway.SeedWork;
using System.Collections.Generic;

namespace PaymentGateway.Domain.ValueObjects
{
    public class Address : ValueObject<Address>
    {
        public string HouseNumber { get; }
        public string Line1 { get; }
        public string Line2 { get; }
        public string City { get; }
        public string Postcode { get; }
        public string Country { get; }

        public Address(string houseNumber, string line1, string line2, string city, string postcode, string country)
        {
            HouseNumber = houseNumber;
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postcode = postcode;
            Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return HouseNumber;
            yield return Line1;
            yield return Line2;
            yield return City;
            yield return Postcode;
            yield return Country;
        }
    }
}
