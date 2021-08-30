namespace CardPaymentAcquirerBank.Sdk.Models
{
    public class CardPaymentAcquirerRequest
    {
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public Card SourceCard { get; set; }
        public BankAccount DestinationBankAccount { get; set; }
    }
}
