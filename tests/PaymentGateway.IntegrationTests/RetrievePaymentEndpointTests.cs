using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using PaymentGateway.Contract.Responses;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using BankAccount = PaymentGateway.Domain.ValueObjects.BankAccount;
using Card = PaymentGateway.Domain.Entities.Card;
using Contact = PaymentGateway.Domain.ValueObjects.Contact;
using Payment = PaymentGateway.Domain.AggregateRoot.Payment;
using Shopper = PaymentGateway.Domain.Entities.Shopper;

namespace PaymentGateway.IntegrationTests
{
    public class RetrievePaymentEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly IFixture _fixture = new Fixture();
        private string RetrievePaymentEndpoint(string paymentId) => $"payments/{paymentId}";

        public RetrievePaymentEndpointTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        [Description(@"GIVEN a payment id
                       AND the payment exist
                       WHEN the retrieve payment endpoint is called
                       THEN OK 200 status code is returned
                       AND the payment response is returned")]
        public async Task PaymentRetrieved()
        {
            var databaseSettings = new DatabaseSettings() { ConnectionString = "PaymentGateway.db" };
            var options = Options.Create(databaseSettings);
            var paymentRepository = new PaymentRepository(options);

            var payment = CreatePayment();
            await paymentRepository.Add(payment);

            var paymentCard = payment.Source.Card;
            var expectedMaskCardNumber = $"************{paymentCard.CardNumber.Substring(paymentCard.CardNumber.Length - 4)}";
            var expectedMaskCvv = $"*{paymentCard.Cvv.Substring(paymentCard.Cvv.Length - 1)}";

            SetRequestHeaders(payment.Merchant.Id);
            var response = await _httpClient.GetAsync(RetrievePaymentEndpoint(payment.Id.ToString()), CancellationToken.None);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

            using (new AssertionScope())
            {
                paymentResponse.Should().NotBeNull();
                paymentResponse.Approved.Should().BeTrue();
                paymentResponse.Status.Should().Be(PaymentStatus.Authorized);

                paymentResponse.Card.MaskedCardNumber.Should().Be(expectedMaskCardNumber);
                paymentResponse.Card.MaskedCvv.Should().Be(expectedMaskCvv);
            }
        }

        [Fact]
        [Description(@"GIVEN a payment id that does not exist
                       AND merchant id that owns the payment
                       WHEN the retrieve payment endpoint is called
                       THEN Not Found status code is return")]
        public async Task PaymentNotFound()
        {
            var paymentId = Guid.NewGuid().ToString();
            SetRequestHeaders();
            var response = await _httpClient.GetAsync(RetrievePaymentEndpoint(paymentId), CancellationToken.None);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Description(@"GIVEN a payment id
                       AND invalid merchant id 
                       WHEN the retrieve payment endpoint is called
                       THEN Unauthorized status code is return")]
        public async Task Unauthorized()
        {
            var paymentId = Guid.NewGuid().ToString();

            SetRequestHeaders(Guid.Empty);
            var response = await _httpClient.GetAsync(RetrievePaymentEndpoint(paymentId), CancellationToken.None);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private void SetRequestHeaders(Guid? merchantId = null, string requestId = null)
        {
            _httpClient.DefaultRequestHeaders.Add("merchantId", merchantId?.ToString() ?? Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("x-requestid", requestId ?? Guid.NewGuid().ToString());
        }

        private static Payment CreatePayment()
        {
            var shopper = new Shopper(Guid.NewGuid(), "Johnathan Wick", "SHOPPER-100",
                new Contact("071020304050", "johnwick@continental.com"),
                new Domain.ValueObjects.Address("120", "Northern court", "West lane", "Uptown", "NN0 3kk", "England"));
            var merchant = new Merchant(Guid.NewGuid(), "The Merchant Name");

            return new Payment(Guid.NewGuid(),
                100,
                true,
                "A sales",
                "SALE-299",
                SupportedCurrency.EUR,
                PaymentType.Regular,
                PaymentStatus.Authorized, DateTime.UtcNow,
                new Domain.Entities.PaymentSource(Guid.NewGuid(), PaymentSourceType.Card, CreateValidCard(), shopper.Id),
                new PaymentDestination(Guid.NewGuid(), PaymentDestinationType.BankAccount,
                    new BankAccount("893838", "848484848", "The Bank Name"), merchant.Id),
                merchant,
                shopper,
                new AcquirerResult("The Acquirer", true, "ACQ-3493", PaymentStatus.Authorized, DateTime.UtcNow, 100),
                new List<Transaction>());
        }

        private static Card CreateValidCard()
        {
            return new Card(Guid.NewGuid(),
                "Johnathan Wick",
                "Continental",
                "5436031030606378",
                "934",
                "09",
                "2025",
                new Address("120", "Northern court", "West lane", "Uptown", "NN0 3kk", "England"));
        }
    }
}
