using System;

namespace PaymentGateway.Application.Payments.Queries
{
    public class ShopperProjection
    {
        public Guid  Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public ContactProjection Contact { get; set; }
        public AddressProjection ShippingAddress { get; set; }
    }
}
