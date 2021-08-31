using PaymentGateway.Contract.Enumerations;

namespace PaymentGateway.Contract.Models
{
    public class PaymentSource
    {
        /// <summary>
        /// The source of the payment. Example, card
        /// </summary>
        public PaymentSourceType Type { get; set; }

        /// <summary>
        /// The card details from which the amount is taken
        /// </summary>
        public Card Card { get; set; }
    }
}
