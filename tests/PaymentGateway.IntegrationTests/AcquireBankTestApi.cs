using Checkout.ApiFaker;
using System.Net;
using System.Net.Http;

namespace PaymentGateway.IntegrationTests
{
    internal class AcquirerBankTestApi
    {
        private readonly ApiServerBuilder _builder;

        public AcquirerBankTestApi()
        {
            _builder = new ApiServerBuilder();
        }

        public ApiServer Build() => _builder.Build();

        public AcquirerBankTestApi WithAcquirePaymentEndpoint(HttpStatusCode responseStatusCode, string expectedContent)
        {
            _builder.WithEndpoint("/acquire", HttpMethod.Post, responseStatusCode, "application/json", expectedContent);

            return this;
        }
    }
}
