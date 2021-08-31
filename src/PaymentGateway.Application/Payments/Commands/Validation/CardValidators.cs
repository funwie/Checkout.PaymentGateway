using FluentValidation;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Payments.Commands.Validation
{
    public class CardValidator :  AbstractValidator<Card>
    {
        public CardValidator()
        {
            RuleFor(card => card)
                .NotEmpty()
                .WithMessage("Null card.");

            RuleFor(card => card.Id)
                .NotEmpty()
                .WithMessage("Invalid Id.");

            RuleFor(card => card.FullName)
                .NotEmpty().When(card => string.IsNullOrWhiteSpace(card.CompanyName))
                .WithMessage("Card holder full name or company name is required.");

            RuleFor(card => card.CompanyName)
                .NotEmpty().When(card => string.IsNullOrWhiteSpace(card.FullName))
                .WithMessage("Card holder full name or company name is required.");

            RuleFor(card => card.CardNumber)
                .CreditCard()
                .WithMessage("Credit card number is invalid.");

            RuleFor(card => card.Cvv)
                .Matches(@"^[0-9]{3,4}$")
                .WithMessage("Invalid cvv value.");

            RuleFor(card => card.ExpiryMonth)
                .Matches(@"^[0-9]{2}$")
                .WithMessage("Invalid expiry month.");

            RuleFor(card => card.ExpiryYear)
                .Matches(@"^[0-9]{2,4}$")
                .WithMessage("Invalid expiry year.");
        }
    }
}
