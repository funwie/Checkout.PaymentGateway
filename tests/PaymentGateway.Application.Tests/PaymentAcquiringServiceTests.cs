using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.AcquiringService;
using PaymentGateway.Domain.AggregateRoot;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Tests
{
    public class PaymentAcquiringServiceTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly PaymentAcquiringService _sut;
        private readonly Mock<IAcquirerFactory> _acquirerFactoryMock;
        private readonly Mock<IAcquirer> _acquirerMock;

        public PaymentAcquiringServiceTests()
        {
            _acquirerMock = new Mock<IAcquirer>();

            _acquirerFactoryMock = new Mock<IAcquirerFactory>();
            _acquirerFactoryMock.Setup(factory => factory.CreateAcquirer(It.IsAny<PaymentSourceType>()))
                .Returns(_acquirerMock.Object);
            _fixture.Inject(_acquirerFactoryMock.Object);

            var loggerMock = new Mock<ILogger<PaymentAcquiringService>>();
            _fixture.Inject(loggerMock.Object);

            _sut = _fixture.Create<PaymentAcquiringService>();
        }

        [Test]
        [Description(@"GIVEN request to acquire a payment
                       AND the payment source type acquirer exist 
                       WHEN the sut AcquirePayment is is called
                       THEN Acquirer AcquirePayment is called")]
        public async Task AcquirerIsCalled()
        {
            var paymentToAcquire = CreatePayment();
            _acquirerMock.Setup(acquirer => acquirer.AcquirePayment(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.Create<AcquirerResponse>())
                .Verifiable();

            var acquirePaymentResult = await _sut.AcquirePayment(paymentToAcquire, CancellationToken.None);

            acquirePaymentResult.IsSuccess.Should().BeTrue();
        }

        [Test]
        [Description(@"GIVEN request to acquire a payment
                       AND the payment source type acquirer does not exist 
                       WHEN the sut AcquirePayment is is called
                       THEN UnsupportedAcquirerException is thrown")]
        public async Task UnsupportedAcquirer()
        {
            var paymentToAcquire = CreatePayment();
            _acquirerFactoryMock.Setup(factory => factory.CreateAcquirer(It.IsAny<PaymentSourceType>()))
                .Throws<UnsupportedAcquirerException>();

            Func<Task> act = async () => await _sut.AcquirePayment(paymentToAcquire, CancellationToken.None);

            await act.Should().ThrowAsync<UnsupportedAcquirerException>();
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
                PaymentStatus.Authorized, DateTime.UtcNow,
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
    }
}
