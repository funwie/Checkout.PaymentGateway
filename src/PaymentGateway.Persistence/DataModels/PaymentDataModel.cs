using SQLite;
using System;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("Payments")]
    public class PaymentDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("currency")]
        public string Currency { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("source_id")]
        public string SourceId { get; set; }

        [Column("destination_id")]
        public string DestinationId { get; set; }

        [Column("merchant_id")]
        public string MerchantId { get; set; }

        [Column("shopper_id")]
        public string ShopperId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("approved")]
        public bool? Approved { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("requested_on")]
        public DateTime RequestedOn { get; set; }
    }
}
