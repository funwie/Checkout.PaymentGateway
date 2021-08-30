using System;
using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("Transactions")]
    public class TransactionDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Indexed]
        [Column("payment_id")]
        public string PaymentId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("approved")]
        public bool? Approved { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("performed_at")]
        public DateTime PerformedAt { get; set; }
    }
}