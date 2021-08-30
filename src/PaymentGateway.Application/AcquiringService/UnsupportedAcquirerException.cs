using System;
using System.Runtime.Serialization;

namespace PaymentGateway.Application.AcquiringService
{
    [Serializable]
    public class UnsupportedAcquirerException : Exception
    {
        public UnsupportedAcquirerException()
        {
        }

        public UnsupportedAcquirerException(string message) : base(message)
        {
        }

        public UnsupportedAcquirerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnsupportedAcquirerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}