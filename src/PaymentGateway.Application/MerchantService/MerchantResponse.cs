using System;

namespace PaymentGateway.Application.MerchantService
{
    public class MerchantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
