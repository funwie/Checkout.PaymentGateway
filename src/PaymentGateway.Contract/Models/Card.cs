namespace PaymentGateway.Contract.Models
{
    public class Card
    {
        /// <summary>
        /// Name of the card holder
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Company name of card holder
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Card Number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 3 or 4 digits card cvv value
        /// </summary>
        public string Cvv { get; set; }

        /// <summary>
        /// Card expiry month - 01 to 31
        /// </summary>
        public string ExpiryMonth { get; set; }

        /// <summary>
        /// Card expiry year - example 2025, 2050, 2100
        /// </summary>
        public string ExpiryYear { get; set; }

        /// <summary>
        /// The Card billing address
        /// </summary>
        public Address BillingAddress { get; set; }
    }
}
