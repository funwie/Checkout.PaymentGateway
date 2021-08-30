using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("PaymentSources")]
    public class PaymentSourceDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("card_id")]
        public string CardId { get; set; }

        [Column("shopper_id")]
        public string ShopperId { get; set; }
    }
}
