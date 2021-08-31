using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Contract.Models;
using PaymentGateway.Contract.Requests;
using PaymentGateway.Controllers;
using PaymentGateway.Domain.Enumerations;
using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentSourceType = PaymentGateway.Contract.Enumerations.PaymentSourceType;

namespace PaymentGateway.Tests
{
    public class PaymentsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly PaymentsController _sut;
        private readonly Mock<IMediator> _mediatorMock;

        public PaymentsControllerTests()
        {
            _fixture = new Fixture();
            _mediatorMock = new Mock<IMediator>();

            _sut = new PaymentsController(_mediatorMock.Object, Mock.Of<ILogger<PaymentsController>>());
        }

        [Test]
        [Description(@"GIVEN a payment request
                       AND the payment will be successfully approved
                       WHEN RequestPayment is called
                       THEN Created status code is returned")]
        public async Task PaymentIsCreated()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            var expectedPaymentResult = new PaymentResult
            {
                PaymentId = Guid.NewGuid().ToString(),
                Approved = true,
                Status = PaymentStatus.Authorized
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RequestPaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPaymentResult);

            var expectedPaymentUri = $"/payments/{expectedPaymentResult.PaymentId}";

            var actionResult = await _sut.RequestPayment(paymentRequest,
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                CancellationToken.None);

            using (new AssertionScope())
            {
                actionResult.Should().BeOfType<CreatedResult>();
                var result = actionResult as CreatedResult;
                result.Should().NotBeNull();
                result.Location.Should().Be(expectedPaymentUri);
            }
        }

        [Test]
        [Description(@"GIVEN a payment request
                       AND the payment will not have approved outcome
                       WHEN RequestPayment is called
                       THEN Accepted status code is returned")]
        public async Task PaymentIsAccepted()
        {
            var paymentSource = new PaymentSource
            {
                Type = PaymentSourceType.Card,
                Card = CreateValidCard()
            };

            var paymentRequest = _fixture.Build<PaymentRequest>()
                .With(req => req.Source, paymentSource)
                .Create();

            var expectedPaymentResult = new PaymentResult
            {
                PaymentId = Guid.NewGuid().ToString(),
                Approved = null,
                Status = PaymentStatus.Processing
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RequestPaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPaymentResult);

            var expectedPaymentUri = $"/payments/{expectedPaymentResult.PaymentId}";

            var actionResult = await _sut.RequestPayment(paymentRequest,
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                CancellationToken.None);

            using (new AssertionScope())
            {
                actionResult.Should().BeOfType<AcceptedResult>();
                var result = actionResult as AcceptedResult;
                result.Should().NotBeNull();
                result.Location.Should().Be(expectedPaymentUri);
            }
        }

        private Card CreateValidCard()
        {
            return new Card
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
    }
}
