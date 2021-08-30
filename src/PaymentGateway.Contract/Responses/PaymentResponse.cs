using System.Collections.Generic;

namespace PaymentGateway.Contract.Responses
{
    public class PaymentResponse
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public bool? Approved { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public ShopperResponse Shopper { get; set; }
        public CardResponse Card { get; set; }
        public AcquirerResponse Acquirer { get; set; }
        public IEnumerable<TransactionResponse> Transactions { get; set; }
    }
}
