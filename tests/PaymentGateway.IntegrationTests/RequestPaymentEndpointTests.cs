using AutoFixture;
using CardPaymentAcquirerBank.Sdk.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Contract.Models;
using PaymentGateway.Contract.Requests;
using PaymentGateway.Domain.Enumerations;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Address = PaymentGateway.Contract.Models.Address;
using PaymentSourceType = PaymentGateway.Contract.Enumerations.PaymentSourceType;

namespace PaymentGateway.IntegrationTests
{
    public class RequestPaymentEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        private readonly IFixture _fixture = new Fixture();

        private readonly string RequestPaymentEndpoint = "payments";
        private readonly string AcquirerBankBaseUrl = "http://localhost:8088";

        public RequestPaymentEndpointTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        [Description(@"GIVEN a valid payment request
                       AND the acquier bank api will authorize the payment
                       WHEN the request payment endpoint is called
                       THEN created 201 status code is returned
                       AND the payment result is returned with approved equals true and Authorized status")]
        public async Task PaymentRequestIsAuthorized()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            var expectedAuthorizedAcquirerResponse = new CardPaymentAcquirerResponse
            {
                Name = "The Card Acquirer Bank",
                Amount = paymentRequest.Amount,
                TransactionReference = Guid.NewGuid().ToString(),
                Approved = true,
                Status = PaymentStatus.Authorized,
                PerformedOn = DateTime.UtcNow
            };

            var expectedApiResponse = JsonConvert.SerializeObject(expectedAuthorizedAcquirerResponse);
            var acquirerBankTestApi = new AcquirerBankTestApi()
                .WithAcquirePaymentEndpoint(HttpStatusCode.OK, expectedApiResponse)
                .Build();

            using (acquirerBankTestApi.Start(AcquirerBankBaseUrl))
            {
                SetRequestHeaders();
                var paymentRequestResponse = await _httpClient.PostAsJsonAsync(RequestPaymentEndpoint, paymentRequest, CancellationToken.None);
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

        [Fact]
        [Description(@"GIVEN a valid payment request
                       AND the acquier bank api will decline the payment
                       WHEN the request payment endpoint is called
                       THEN created 201 status code is returned
                       AND the payment result is returned with approved equals false and Declined status")]
        public async Task PaymentRequestIsDeclined()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            var expectedDeclinedAcquirerResponse = new CardPaymentAcquirerResponse
            {
                Name = "The Card Acquirer Bank",
                Amount = paymentRequest.Amount,
                Approved = false,
                TransactionReference = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Declined,
                PerformedOn = DateTime.UtcNow
            };

            var expectedApiResponse = JsonConvert.SerializeObject(expectedDeclinedAcquirerResponse);
            var acquirerBankTestApi = new AcquirerBankTestApi()
                .WithAcquirePaymentEndpoint(HttpStatusCode.OK, expectedApiResponse)
                .Build();

            using (acquirerBankTestApi.Start(AcquirerBankBaseUrl))
            {
                SetRequestHeaders();
                var paymentRequestResponse = await _httpClient.PostAsJsonAsync(RequestPaymentEndpoint, paymentRequest, CancellationToken.None);
                var expectedPaymentResult = await paymentRequestResponse.Content.ReadFromJsonAsync<PaymentResult>();

                using (new AssertionScope())
                {
                    paymentRequestResponse.StatusCode.Should().Be(HttpStatusCode.Created);

                    expectedPaymentResult.Should().NotBeNull();
                    expectedPaymentResult.Approved.Should().BeFalse();
                    expectedPaymentResult.Status.Should().Be(PaymentStatus.Declined);
                }
            }
        }

        [Fact]
        [Description(@"GIVEN a valid payment request
                       AND the request to the acquier bank api will fail
                       WHEN the request payment endpoint is called
                       THEN accepted 202 status code is returned
                       AND the payment result is returned with approved equals null and Processing status")]
        public async Task AcquirerBankRequestFailure()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            var acquirerBankTestApi = new AcquirerBankTestApi()
                .WithAcquirePaymentEndpoint(HttpStatusCode.InternalServerError, null)
                .Build();

            using (acquirerBankTestApi.Start(AcquirerBankBaseUrl))
            {
                SetRequestHeaders();
                var paymentRequestResponse = await _httpClient.PostAsJsonAsync(RequestPaymentEndpoint, paymentRequest, CancellationToken.None);
                var expectedPaymentResult = await paymentRequestResponse.Content.ReadFromJsonAsync<PaymentResult>();

                using (new AssertionScope())
                {
                    paymentRequestResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

                    expectedPaymentResult.Should().NotBeNull();
                    expectedPaymentResult.Approved.Should().BeNull();
                    expectedPaymentResult.Status.Should().Be(PaymentStatus.Processing);
                }
            }
        }

        [Fact]
        [Description(@"GIVEN a valid payment request
                       AND merchant id that does not exist
                       WHEN the request payment endpoint is called
                       THEN unauthorized status code is returned")]
        public async Task Unauthorized()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            SetRequestHeaders(Guid.Empty);
            var paymentRequestResponse = await _httpClient.PostAsJsonAsync(RequestPaymentEndpoint, paymentRequest, CancellationToken.None);

            paymentRequestResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        [Description(@"GIVEN a payment request
                       AND card number is not valid
                       WHEN the request payment endpoint is called
                       THEN bad request status code is returned")]
        public async Task InvalidPaymentRequest()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = new Contract.Models.Card
                {
                    FullName = "Johnathan Wick",
                    CompanyName = "Continental",
                    CardNumber = "notacardnumber",
                    Cvv = "934",
                    ExpiryMonth = "09",
                    ExpiryYear = "2025",
                    BillingAddress = _fixture.Create<Address>()
                }
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            SetRequestHeaders();
            var paymentRequestResponse = await _httpClient.PostAsJsonAsync(RequestPaymentEndpoint, paymentRequest, CancellationToken.None);

            paymentRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private Contract.Models.Card CreateValidCard()
        {
            return new Contract.Models.Card
            {
                FullName = "Johnathan Wick",
                CompanyName = "Continental",
                CardNumber = "5436031030606378",
                Cvv = "934",
                ExpiryMonth = "09",
                ExpiryYear = "2025",
                BillingAddress = _fixture.Create<Address>()
            };
        }

        private void SetRequestHeaders(Guid? merchantId = null, string requestIdempotencyKey = null, string requestId = null)
        {
            _httpClient.DefaultRequestHeaders.Add("merchantId", merchantId?.ToString() ?? Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("requestIdempotencyKey", requestIdempotencyKey ?? Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("x-requestid", requestId ?? Guid.NewGuid().ToString());
        }
    }
}
