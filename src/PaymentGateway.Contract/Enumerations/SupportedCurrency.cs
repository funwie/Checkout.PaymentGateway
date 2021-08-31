using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    /// <summary>
    /// Three letter currency symbol
    /// </summary>
    public enum Currency
    {
        [EnumMember(Value = "GBP")]
        GBP = 826,

        [EnumMember(Value = "EUR")]
        EUR = 978,

        [EnumMember(Value = "USD")]
        USD = 840
    }
}
