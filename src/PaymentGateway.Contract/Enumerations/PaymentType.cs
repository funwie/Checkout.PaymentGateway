using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    public enum PaymentType
    {
        [EnumMember(Value = "Regular")]
        Regular
    }
}
