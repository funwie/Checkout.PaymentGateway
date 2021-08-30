using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Contract.Models;
using PaymentGateway.Contract.Requests;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CardPaymentAcquirerBank.Sdk.Models;
using FluentAssertions.Execution;
using PaymentGateway.Domain.Enumerations;
using Xunit;
using Address = PaymentGateway.Contract.Models.Address;
using PaymentSourceType = PaymentGateway.Contract.Enumerations.PaymentSourceType;

namespace PaymentGateway.IntegrationTests
{
    public class RequestPaymentEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly HttpClient _httpClient;
        private readonly string PaymentsEndpoint = "payments";
        private readonly string AcquirerBankBaseUrl = "http://localhost:8081";


        public RequestPaymentEndpointTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task PaymentRequestIsAuthorized()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = new Contract.Models.Card
                {
                    FullName = "Johnathan Wick",
                    CompanyName= "Continental",
                    CardNumber = "5436031030606378",
                    Cvv = "934",
                    ExpiryMonth = "09",
                    ExpiryYear = "2019",
                    BillingAddress = _fixture.Create<Address>()
                }
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            _httpClient.DefaultRequestHeaders.Add("merchantId", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("requestIdempotencyKey", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());

            var expectedAcquirerResponse = new CardPaymentAcquirerResponse
            {
                Name = "The Card Acquirer Bank",
                Amount = paymentRequest.Amount,
                TransactionReference = Guid.NewGuid().ToString(),
                Approved = true,
                Status = PaymentStatus.Authorized,
                PerformedOn = DateTime.UtcNow
            };

            var expectedApiResponse = JsonConvert.SerializeObject(expectedAcquirerResponse);
            var acquirerBankTestApi = new AcquireBankTestApi()
                .WithAcquirePaymentEndpoint(HttpStatusCode.OK, expectedApiResponse)
                .Build();

            using (acquirerBankTestApi.Start(AcquirerBankBaseUrl))
            {
                var paymentRequestResponse = await _httpClient.PostAsJsonAsync(PaymentsEndpoint, paymentRequest, CancellationToken.None);
                var expectedPaymentResult = await paymentRequestResponse.Content.ReadFromJsonAsync<PaymentResult>();

                using (new AssertionScope())
                {
                    paymentRequestResponse.StatusCode.Should().Be(HttpStatusCode.Created);

                    expectedPaymentResult.Should().NotBeNull();
                    expectedPaymentResult.Approved.Should().BeTrue();
                    expectedPaymentResult.Status.Should().Be(PaymentStatus.Authorized);
                }
            }
        }
    }
}
