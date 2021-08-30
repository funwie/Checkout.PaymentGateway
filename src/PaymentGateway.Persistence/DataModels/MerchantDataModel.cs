using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("Merchants")]
    public class MerchantDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
