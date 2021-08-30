﻿using System.Runtime.Serialization;

namespace PaymentGateway.Contract.Enumerations
{
    public enum Currency
    {
        [EnumMember(Value = "GBP")]
        GBP = 826,

        [EnumMember(Value = "EUR")]
        EUR = 978,

        [EnumMember(Value = "USD")]
        USD = 840,

        [EnumMember(Value = "JPY")]
        JPY = 392,
        
        [EnumMember(Value = "CNY")]
        CNY = 156
    }
}
