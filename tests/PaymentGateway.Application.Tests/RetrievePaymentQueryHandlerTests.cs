using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Domain;
using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Tests
{
    public class RetrievePaymentQueryHandlerTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly RetrievePaymentQueryHandler _sut;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;

        public RetrievePaymentQueryHandlerTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _fixture.Inject(_paymentRepositoryMock.Object);

            var loggerMock = new Mock<ILogger<RetrievePaymentQueryHandler>>();

            _fixture.Inject(_paymentRepositoryMock.Object);
            _fixture.Inject(loggerMock.Object);

            _sut = _fixture.Create<RetrievePaymentQueryHandler>();
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a payment
                       AND the payment is found
                       WHEN the query is handled
                       THEN a payment projection is returned")]
        public async Task PaymentIsRetrieved()
        {
            var existingPayment = CreatePayment();
            _paymentRepositoryMock.Setup(service => service.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPayment);

            var expectedPayment = Map(existingPayment);

            var query = new RetrievePaymentQuery(existingPayment.Id, existingPayment.Merchant.Id);
            var retrievePaymentResult = await _sut.Handle(query, CancellationToken.None);
            var actualPayment = retrievePaymentResult.Success;
            
            retrievePaymentResult.IsSuccess.Should().BeTrue();
            actualPayment.Should().BeEquivalentTo(expectedPayment);
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a payment
                       AND the payment is not found
                       WHEN the query is handled
                       THEN PaymentNotFound error is returned")]
        public async Task PaymentNotFoundError()
        {
            _paymentRepositoryMock.Setup(service => service.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new RepositoryNotFoundException());

            var query = new RetrievePaymentQuery(Guid.NewGuid(), Guid.NewGuid());
            var retrievePaymentResult = await _sut.Handle(query, CancellationToken.None);

            retrievePaymentResult.IsSuccess.Should().BeFalse();
            retrievePaymentResult.Error.Should().Be(RetrievePaymentQueryError.PaymentNotFound);
        }

        [Test]
        [Description(@"GIVEN a query to retrieve a payment
                       AND repository fails to retrieve the payment
                       WHEN the query is handled
                       THEN FailedToRetrievePayment error is returned")]
        public async Task RepositoryFailure()
        {
            _paymentRepositoryMock.Setup(service => service.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new Exception());

            var query = new RetrievePaymentQuery(Guid.NewGuid(), Guid.NewGuid());
            var retrievePaymentResult = await _sut.Handle(query, CancellationToken.None);

            retrievePaymentResult.IsSuccess.Should().BeFalse();
            retrievePaymentResult.Error.Should().Be(RetrievePaymentQueryError.FailedToRetrievePayment);
        }

        private static Payment CreatePayment()
        {
            var shopper = new Shopper(Guid.NewGuid(), "Johnathan Wick", "SHOPPER-100",
                new Contact("071020304050", "johnwick@continental.com"),
                new Address("120", "Northern court", "West lane", "Uptown", "NN0 3kk", "England"));
            var merchant = new Merchant(Guid.NewGuid(), "The Merchant Name");

            return new Payment(Guid.NewGuid(),
                100,
                true,
                "A sales",
                "SALE-299",
                SupportedCurrency.EUR,
                PaymentType.Regular,
                PaymentStatus.Authorized, 
                DateTime.UtcNow,
                new PaymentSource(Guid.NewGuid(), PaymentSourceType.Card, CreateValidCard(), shopper.Id),
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

        private PaymentProjection Map(Payment payment)
        {
            if (payment is null) return null;

            return new PaymentProjection
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Currency = payment.Currency.ToString(),
                Type = payment.Type.ToString(),
                Approved = payment.Approved,
                Status = payment.Status,
                Reference = payment.Reference,
                Description = payment.Description,
                RequestedOn = payment.RequestedOn,
                Shopper = Map(payment.Shopper),
                Card = Map(payment.Source?.Card),
                Acquirer = Map(payment.AcquirerResult),
                Transactions = Map(payment.Transactions)
            };
        }

        private static ShopperProjection Map(Shopper shopper)
        {
            if (shopper is null) return null;

            return new ShopperProjection
            {
                Id = shopper.Id,
                Name = shopper.Name,
                Reference = shopper.Reference,
                Contact = new ContactProjection { Phone = shopper.Contact?.Phone, Email = shopper.Contact?.Email },
                ShippingAddress = new AddressProjection
                {
                    HouseNumber = shopper.ShippingAddress?.HouseNumber,
                    Line1 = shopper.ShippingAddress?.Line1,
                    Line2 = shopper.ShippingAddress?.Line2,
                    City = shopper.ShippingAddress?.City,
                    Postcode = shopper.ShippingAddress?.Postcode,
                    Country = shopper.ShippingAddress?.Country
                }
            };
        }

        private static CardProjection Map(Card card)
        {
            if (card is null) return null;

            return new CardProjection
            {
                Id = card.Id,
                FullName = card.FullName,
                CompanyName = card.CompanyName,
                CardNumber = card.CardNumber,
                Cvv = card.Cvv,
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                MaskedCardNumber = card.MaskedCardNumber,
                MaskedCvv = card.MaskedCvv,
                BillingAddress = new AddressProjection
                {
                    HouseNumber = card.BillingAddress?.HouseNumber,
                    Line1 = card.BillingAddress?.Line1,
                    Line2 = card.BillingAddress?.Line2,
                    City = card.BillingAddress?.City,
                    Postcode = card.BillingAddress?.Postcode,
                    Country = card.BillingAddress?.Country
                }
            };
        }

        private static AcquirerProjection Map(AcquirerResult acquirerResult)
        {
            if (acquirerResult is null) return null;

            return new AcquirerProjection
            {
                Reference = acquirerResult.Reference,
                PerformedOn = acquirerResult.PerformedOn
            };
        }

        private static IEnumerable<TransactionProjection> Map(IEnumerable<Transaction> transactions)
        {
            return transactions.Select(transaction => new TransactionProjection
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Approved = transaction.Approved,
                Type = transaction.Type,
                Reference = transaction.Reference,
                PerformedOn = transaction.PerformedOn
            });
        }
    }
}
