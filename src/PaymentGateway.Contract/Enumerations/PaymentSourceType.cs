using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    public enum PaymentSourceType
    {
        [EnumMember(Value = "Card")]
        Card
    }
}
