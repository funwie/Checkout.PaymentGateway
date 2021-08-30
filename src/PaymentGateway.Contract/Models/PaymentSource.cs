using PaymentGateway.Contract.Enumerations;

namespace PaymentGateway.Contract.Models
{
    public class PaymentSource
    {
        public PaymentSourceType Type { get; set; }
        public Card Card { get; set; }
    }
}
