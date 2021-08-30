using PaymentGateway.SeedWork;
using System.Collections.Generic;

namespace PaymentGateway.Domain.ValueObjects
{
    public class Contact : ValueObject<Contact>
    {
        public string Phone { get; }
        public string Email { get; }

        public Contact(string phone, string email)
        {
            Phone = phone;
            Email = email;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Phone;
            yield return Email;
        }
    }
}
