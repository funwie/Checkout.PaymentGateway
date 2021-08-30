using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("PaymentDestinations")]
    public class PaymentDestinationDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("sort_code")]
        public string SortCode { get; set; }

        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Column("bank_name")]
        public string BankName { get; set; }

        [Column("merchant_id")]
        public string MerchantId { get; set; }
    }
}
