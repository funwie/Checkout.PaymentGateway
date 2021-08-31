using System;
using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("AcquirerResults")]
    public class AcquirerResultDataModel
    {
        [Column("id")]
        public string Id { get; set; }

        [Indexed]
        [Column("payment_id")]
        public string PaymentId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("approved")]
        public bool? Approved { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("performed_on")]
        public DateTime PerformedOn { get; set; }
    }
}
