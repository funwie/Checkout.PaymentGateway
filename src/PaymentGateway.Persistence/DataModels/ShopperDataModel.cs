using SQLite;

namespace PaymentGateway.Persistence.DataModels
{
    [Table("Shoppers")]
    public class ShopperDataModel
    {
        [PrimaryKey]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("email")]
        public string Email { get; set;  }

        [Column("shipping_house_number")]
        public string HouseNumber { get; set; }

        [Column("shipping_line_1")]
        public string Line1 { get; set; }

        [Column("shipping_line_2")]
        public string Line2 { get; set; }

        [Column("shipping_city")]
        public string City { get; set; }

        [Column("shipping_post_code")]
        public string Postcode { get; set; }

        [Column("shipping_country")]
        public string Country { get; set; }
    }
}
