using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    /// <summary>
    /// The type of payment
    /// </summary>
    public enum PaymentType
    {
        [EnumMember(Value = "Regular")]
        Regular
    }
}
