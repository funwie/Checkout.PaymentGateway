namespace PaymentGateway.Application.Payments.Commands
{
    public class PaymentResult
    {
        public string PaymentId { get; set; }
        public bool? Approved { get; set; }
        public string Status { get; set; }
    }
}
