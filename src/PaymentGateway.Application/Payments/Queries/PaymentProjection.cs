using System;
using System.Collections.Generic;

namespace PaymentGateway.Application.Payments.Queries
{
    public class PaymentProjection
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public bool? Approved { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime RequestedOn { get; set; }
        public ShopperProjection Shopper { get; set; }
        public CardProjection Card { get; set; }
        public AcquirerProjection Acquirer { get; set; }
        public IEnumerable<TransactionProjection> Transactions { get; set; }
    }
}