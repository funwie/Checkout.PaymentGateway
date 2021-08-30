using FluentValidation;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Payments.Commands.Validators
{
    public class PaymentSourceValidator : AbstractValidator<PaymentSource>
    {
        public PaymentSourceValidator()
        {
            CascadeMode = CascadeMode.Continue;

            RuleFor(source => source)
                .NotEmpty()
                .WithMessage("Null payment.");

            RuleFor(source => source.Id)
                .NotEmpty()
                .WithMessage("Invalid Id.");

            RuleFor(source => source.Type)
                .IsInEnum()
                .WithMessage("Payment source type is not supported.");

            RuleFor(source => source.Card)
                .NotEmpty()
                .WithMessage("Card is required.")
                .SetValidator(new CardValidator());
        }
    }
}
