using PaymentGateway.SeedWork;
using System.Collections.Generic;

namespace PaymentGateway.Domain.ValueObjects
{
    public class BankAccount : ValueObject<BankAccount>
    {
        public string SortCode { get; }
        public string AccountNumber { get; }
        public string BankName { get; }

        public BankAccount(string sortCode, string accountNumber, string bankName)
        {
            SortCode = sortCode;
            AccountNumber = accountNumber;
            BankName = bankName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SortCode;
            yield return AccountNumber;
            yield return BankName;
        }
    }
}
