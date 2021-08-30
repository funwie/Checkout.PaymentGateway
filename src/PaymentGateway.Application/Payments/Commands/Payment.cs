using System;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Application.Payments.Commands
{
    public class Payment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public SupportedCurrency Currency { get; set; }
        public PaymentType Type { get; set; }
        public PaymentSource Source { get; set; }
        public Shopper Shopper { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
    }
}
