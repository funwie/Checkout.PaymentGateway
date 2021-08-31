using FluentValidation;

namespace PaymentGateway.Application.Payments.Commands.Validation
{
    public class RequestPaymentCommandValidator : AbstractValidator<RequestPaymentCommand>
    {
        public RequestPaymentCommandValidator()
        {
            CascadeMode = CascadeMode.Continue;

            RuleFor(request => request)
                .NotEmpty()
                .WithMessage("Null request was provided");

            RuleFor(request => request.Payment)
                .NotEmpty()
                .WithMessage("Payment is required")
                .SetValidator(new PaymentValidator());
        }
    }
}
