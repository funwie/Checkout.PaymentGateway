using System;
using System.Runtime.Serialization;

namespace PaymentGateway.Persistence
{
    [Serializable]
    public class RepositoryNotFoundException : Exception
    {
        public RepositoryNotFoundException()
        {
        }

        public RepositoryNotFoundException(string message) : base(message)
        {
        }

        public RepositoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RepositoryNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}