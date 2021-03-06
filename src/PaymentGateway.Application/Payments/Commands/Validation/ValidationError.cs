using System.Collections.Generic;
using FluentValidation.Results;

namespace PaymentGateway.Application.Payments.Commands.Validation
{
    public class ValidationError
    {
        public string RequestId { get; set; }
        public string Type { get; set; }
        public IEnumerable<ValidationFailure> Errors { get; set; }
    }
}
