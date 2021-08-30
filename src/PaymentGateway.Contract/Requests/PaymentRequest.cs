using PaymentGateway.Contract.Enumerations;
using PaymentGateway.Contract.Models;

namespace PaymentGateway.Contract.Requests
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public Currency Currency { get;set; }
        public PaymentType Type { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public PaymentSource Source { get; set; }
        public Shopper Shopper { get; set; }
    }
}
