using PaymentGateway.Contract.Enumerations;
using PaymentGateway.Contract.Models;

namespace PaymentGateway.Contract.Requests
{
    public class PaymentRequest
    {
        /// <summary>
        /// Amount to request from payment source
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Three letter currency symbol
        /// </summary>
        public Currency Currency { get;set; }

        /// <summary>
        /// The payment type
        /// </summary>
        public PaymentType Type { get; set; }

        /// <summary>
        /// Description of the payment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Reference of the payment
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// The source from which the amount will be taken
        /// </summary>
        public PaymentSource Source { get; set; }

        /// <summary>
        /// The shopper
        /// </summary>
        public Shopper Shopper { get; set; }
    }
}
