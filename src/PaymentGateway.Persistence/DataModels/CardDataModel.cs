using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("Cards")]
    public class CardDataModel
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("company_name")]
        public string CompanyName { get; set; }

        [Column("card_number")]
        public string CardNumber { get; set; }

        [Column("cvv")]
        public string Cvv { get; set; }

        [Column("expiry_month")]
        public string ExpiryMonth { get; set; }

        [Column("expiry_year")]
        public string ExpiryYear { get; set; }
        
        [Column("masked_card_number")]
        public string MaskedCardNumber { get; set; }

        [Column("masked_cvv")]
        public string MaskedCvv { get; set; }

        [Column("billing_house_number")]
        public string HouseNumber { get; set; }

        [Column("billing_line_1")]
        public string Line1 { get; set; }

        [Column("billing_line_2")]
        public string Line2 { get; set; }

        [Column("billing_city")]
        public string City { get; set; }

        [Column("billing_post_code")]
        public string Postcode { get; set; }

        [Column("billing_country")]
        public string Country { get; set; }
    }
}
