using FluentValidation;

namespace PaymentGateway.Application.Payments.Commands.Validation
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            CascadeMode = CascadeMode.Continue;

            RuleFor(payment => payment)
                .NotEmpty()
                .WithMessage("Null payment.");

            RuleFor(payment => payment.Id)
                .NotEmpty()
                .WithMessage("Invalid id.");

            RuleFor(payment => payment.Amount)
                .GreaterThanOrEqualTo(0.0M)
                .WithMessage("Amount is invalid as negative value.");

            RuleFor(payment => payment.Type)
                .IsInEnum()
                .WithMessage("Unsupported payment type.");

            RuleFor(payment => payment.Currency)
                .IsInEnum()
                .WithMessage("Unsupported currency.");

            RuleFor(payment => payment.Source)
                .NotEmpty()
                .WithMessage("Payment source is required.")
                .SetValidator(new PaymentSourceValidator());
        }
    }
}
