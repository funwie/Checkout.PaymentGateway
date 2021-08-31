﻿using Checkout.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Payments.Commands.Validation;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Contract.Enumerations;
using PaymentGateway.Contract.Requests;
using PaymentGateway.Contract.Responses;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaymentSourceType = PaymentGateway.Contract.Enumerations.PaymentSourceType;
using PaymentType = PaymentGateway.Contract.Enumerations.PaymentType;

namespace PaymentGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")] 
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Request a payment.
        /// </summary>
        /// <param name="request">The Payment Request</param>
        /// <param name="merchantId">The Id of the merchant requesting the payment.</param>
        /// <param name="requestIdempotencyKey">Idempotency key to prevent duplicating payments.</param>
        /// <response code="201">Payment processed successfully.</response>
        /// <response code="202">Payment is processing or requires further action.</response>
        /// <response code="400">Payment request with invalid data was sent.</response>
        /// <response code="401">Merchant unauthorized.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RequestPayment([FromBody] PaymentRequest request,
                                                        [FromHeader] Guid merchantId,
                                                        [FromHeader] string requestIdempotencyKey,
                                                        [FromHeader(Name = "x-requestid")] string requestId,
                                                        CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing payment request {requestId}", requestId);

            if (Guid.Empty.Equals(merchantId)) return Unauthorized();
            if (request is null)
                return new BadRequestObjectResult(new ValidationError { Type = "Null payment request" });

            var createPaymentCommand = CreateRequestPaymentCommand(request, merchantId, requestIdempotencyKey);
            Result<PaymentResult, ValidationError> requestPaymentResult;
            try
            {
                requestPaymentResult = await _mediator.Send(createPaymentCommand, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,"Failed to process payment request {requestId}", requestId);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (requestPaymentResult.IsSuccess is false)
                return new BadRequestObjectResult(requestPaymentResult.Error);

            _logger.LogInformation($"Payment processed {requestId}");

            var paymentResult = requestPaymentResult.Success;
            var paymentUri = $"/payments/{paymentResult.PaymentId}";

            return paymentResult.Approved.HasValue
                ? new CreatedResult(paymentUri, paymentResult)
                : new AcceptedResult(paymentUri, paymentResult);
        }

        /// <summary>
        /// Retrieve a payment.
        /// </summary>
        /// <param name="paymentId">The id of the payment.</param>
        /// <param name = "merchantId" > The Id of the merchant accessing the payment.</param>
        /// <response code="200">Payment response.</response>
        /// <response code="401">Merchant unauthorized.</response>
        /// <response code="404">Payment not found.</response>
        [HttpGet("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RetrievePayment(Guid paymentId,
                                                        [FromHeader] Guid merchantId,
                                                        [FromHeader(Name = "x-requestid")] string requestId,
                                                        CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving payment {paymentId} {requestId}", paymentId, requestId);
            if (Guid.Empty.Equals(merchantId)) return Unauthorized();

            var retrievePaymentQuery = new RetrievePaymentQuery(paymentId, merchantId);
            var retrievePaymentResult = await _mediator.Send(retrievePaymentQuery, cancellationToken);
            
            if (retrievePaymentResult.IsSuccess)
                return Ok(Map(retrievePaymentResult.Success));

            return retrievePaymentResult.Error switch
            {
                RetrievePaymentQueryError.PaymentNotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        private static RequestPaymentCommand CreateRequestPaymentCommand(PaymentRequest request, Guid merchantId, string requestIdempotencyKey)
        {
            var paymentShopper = Map(request.Shopper);
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Currency = Map(request.Currency),
                Amount = request.Amount,
                Description = request.Description,
                Reference = request.Reference,
                Type = Map(request.Type),
                Source = new PaymentSource(Guid.NewGuid(), Map(request.Source.Type), Map(request.Source.Card), paymentShopper.Id),
                Shopper = paymentShopper
            };

            return new RequestPaymentCommand(payment, merchantId, requestIdempotencyKey);
        }

        private static Shopper Map(Contract.Models.Shopper shopper)
        {
            if (shopper is null) return null;

            return new Shopper(Guid.NewGuid(), shopper.Name, shopper.Reference,
                new Contact(shopper.Contact?.Phone, shopper.Contact?.Email),
                Map(shopper.ShippingAddress));
        }

        private static Card Map(Contract.Models.Card card)
        {
            if (card is null) return null;

            return new Card(Guid.NewGuid(),
                            card.FullName, 
                            card.CompanyName, 
                            card.CardNumber,
                            card.Cvv,
                            card.ExpiryMonth, 
                            card.ExpiryYear, 
                            Map(card.BillingAddress));
        }

        private static Address Map(Contract.Models.Address address)
        {
            if (address is null) return null;

            return new Address(address.HouseNumber, 
                               address.Line1, 
                               address.Line2, 
                               address.City, 
                               address.Postcode,
                               address.Country);
        }

        private PaymentResponse Map(PaymentProjection payment)
        {
            var card = payment.Card;
            var shopper = payment.Shopper;

            return new PaymentResponse
            {
                Id = payment.Id.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                Approved = payment.Approved,
                Status = payment.Status,
                Type = payment.Type,
                Reference = payment.Reference,
                Description = payment.Description,
                RequestedOn = payment.RequestedOn,
                Card = new CardResponse
                {
                    Id = card.Id.ToString(),
                    FullName = card.FullName,
                    CompanyName = card.CompanyName,
                    MaskedCardNumber = card.MaskedCardNumber,
                    MaskedCvv = card.MaskedCvv,
                    ExpiryMonth = card.ExpiryMonth,
                    ExpiryYear = card.ExpiryYear,
                    BillingAddress = new Contract.Models.Address
                    {
                        HouseNumber = card.BillingAddress?.HouseNumber,
                        Line1 = card.BillingAddress?.Line1,
                        Line2 = card.BillingAddress?.Line2,
                        City = card.BillingAddress?.City,
                        Postcode = card.BillingAddress?.Postcode,
                        Country = card.BillingAddress?.Country
                    }
                },
                Shopper = new ShopperResponse
                {
                    Id = shopper.Id.ToString(),
                    Name = shopper.Name,
                    Reference = shopper.Reference,
                    Contact = new Contract.Models.Contact { Phone = shopper.Contact?.Phone, Email = shopper.Contact?.Email },
                    ShippingAddress = new Contract.Models.Address
                    {
                        HouseNumber = shopper.ShippingAddress?.HouseNumber,
                        Line1 = shopper.ShippingAddress?.Line1,
                        Line2 = shopper.ShippingAddress?.Line2,
                        City = shopper.ShippingAddress?.City,
                        Postcode = shopper.ShippingAddress?.Postcode,
                        Country = shopper.ShippingAddress?.Country
                    }
                },
                Acquirer = new AcquirerResponse
                {
                    Reference = payment.Acquirer?.Reference,
                    PerformedOn = payment.Acquirer?.PerformedOn
                },
                Transactions = payment.Transactions.Select(transaction => new TransactionResponse
                {
                    Id = transaction.Id.ToString(),
                    Amount = transaction.Amount,
                    Approved = transaction.Approved,
                    Type = transaction.Type,
                    Reference = transaction.Reference,
                    PerformedOn = transaction.PerformedOn
                })
            };
        }

        private static Domain.Enumerations.PaymentSourceType Map(PaymentSourceType type)
        {
            return type switch
            {
                PaymentSourceType.Card => Domain.Enumerations.PaymentSourceType.Card,
                _ => (Domain.Enumerations.PaymentSourceType)int.MaxValue
            };
        }

        private static Domain.Enumerations.PaymentType Map(PaymentType type)
        {
            return type switch
            {
                PaymentType.Regular => Domain.Enumerations.PaymentType.Regular,
                _ => (Domain.Enumerations.PaymentType)int.MaxValue
            };
        }

        private static SupportedCurrency Map(Currency currency)
        {
            if (Enum.TryParse(currency.ToString(), out SupportedCurrency supportedCurrency))
            {
                return supportedCurrency;
            }

            return (SupportedCurrency)int.MaxValue;
        }
    }
}
