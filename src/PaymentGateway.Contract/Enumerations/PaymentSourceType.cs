using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    /// <summary>
    /// The source of the payment
    /// </summary>
    public enum PaymentSourceType
    {
        [EnumMember(Value = "Card")]
        Card
    }
}
