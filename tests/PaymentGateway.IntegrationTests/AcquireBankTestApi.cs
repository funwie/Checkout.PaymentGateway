using Checkout.ApiFaker;
using System.Net;
using System.Net.Http;

namespace PaymentGateway.IntegrationTests
{
    internal class AcquireBankTestApi
    {
        private readonly ApiServerBuilder _builder;

        public AcquireBankTestApi()
        {
            _builder = new ApiServerBuilder();
        }

        public ApiServer Build() => _builder.Build();

        public AcquireBankTestApi WithAcquirePaymentEndpoint(HttpStatusCode responseStatusCode, string expectedContent)
        {
            _builder.WithEndpoint("/acquire", HttpMethod.Post, responseStatusCode, "application/json", expectedContent);

            return this;
        }
    }
}
