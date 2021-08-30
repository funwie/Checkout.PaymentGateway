using FluentValidation;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Payments.Commands.Validators
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
                .NotEmpty()
                .WithMessage("Card cvv is required.");

            RuleFor(card => card.ExpiryMonth)
                .NotEmpty()
                .WithMessage("Id invalid");

            RuleFor(card => card.ExpiryYear)
                .NotEmpty()
                .WithMessage("Id invalid");
        }
    }
}
