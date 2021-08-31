using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Moq;
using NUnit.Framework;
using PaymentGateway.Application.AcquiringService;
using PaymentGateway.Application.MerchantService;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Payments.Commands.Validation;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Tests
{
    public class RequestPaymentCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly RequestPaymentCommandHandler _sut;
        private readonly Mock<IPaymentAcquiringService> _paymentAcquiringServiceMock;
        private readonly Mock<IMerchantService> _merchantServiceMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;

        public RequestPaymentCommandHandlerTests()
        {
            _fixture = new Fixture();

            _paymentAcquiringServiceMock = new Mock<IPaymentAcquiringService>();
            _fixture.Inject(_paymentAcquiringServiceMock.Object);

            _merchantServiceMock = new Mock<IMerchantService>();
            _fixture.Inject(_merchantServiceMock.Object);

            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _fixture.Inject(_paymentRepositoryMock.Object);

            var requestPaymentCommandValidatorMock = new Mock<IValidator<RequestPaymentCommand>>();
            requestPaymentCommandValidatorMock
                .Setup(validator => validator.ValidateAsync(It.IsAny<RequestPaymentCommand>(), It.IsAny<CancellationToken>()))
                .Returns((RequestPaymentCommand command, CancellationToken token) => 
                    new RequestPaymentCommandValidator().ValidateAsync(command, token));
            _fixture.Inject(requestPaymentCommandValidatorMock.Object);

            _sut = _fixture.Create<RequestPaymentCommandHandler>();
        }

        [TestCase(PaymentStatus.Authorized)]
        [TestCase(PaymentStatus.Declined)]
        [Description(@"GIVEN a valid command to request a payment
                       And acquirer service will return a result
                       WHEN handle is called
                       THEN expected payment result is returned")]
        public async Task PaymentResultIsReturned(string expectedAcquirerPaymentStatus)
        {
            var shopper = new Shopper(Guid.NewGuid(), "Johnathan Wick", "SHOPPER-100",
                new Contact("071020304050", "johnwick@continental.com"),
                new Address("120", "Northern court", "West lane", "Uptown", "NN0 3kk", "England"));

            var paymentToRequest = _fixture.Build<Payment>()
                .With(payment =>
                    payment.Source, new PaymentSource(Guid.NewGuid(), PaymentSourceType.Card, CreateValidCard(), shopper.Id))
                .Create();

            var requestPaymentCommand =
                new RequestPaymentCommand(paymentToRequest, Guid.NewGuid(), _fixture.Create<string>());

            var expectedPaymentApproved = expectedAcquirerPaymentStatus == PaymentStatus.Authorized ? true : false;
            var acquirerResponse = _fixture.Build<AcquirerResponse>()
                .With(response => response.Approved, expectedPaymentApproved)
                .With(response => response.Status, expectedAcquirerPaymentStatus)
                .Create();
    
            _paymentAcquiringServiceMock.Setup(service =>
                    service.AcquirePayment(It.IsAny<Domain.AggregateRoot.Payment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acquirerResponse);

            _merchantServiceMock.Setup(service => service.GetMerchant(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.Create<MerchantResponse>());

            _paymentRepositoryMock.Setup(repo => repo.Add(It.IsAny<Domain.AggregateRoot.Payment>()))
                .Returns(Task.CompletedTask);

            var requestPaymentResult = await _sut.Handle(requestPaymentCommand, CancellationToken.None);

            using (new AssertionScope())
            {
                requestPaymentResult.IsSuccess.Should().BeTrue();

                var paymentResult = requestPaymentResult.Success;
                paymentResult.Approved.Should().Be(expectedPaymentApproved);
                paymentResult.Status.Should().Be(expectedAcquirerPaymentStatus);
                paymentResult.PaymentId.Should().Be(paymentToRequest.Id.ToString());
            }
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
