using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.MerchantService
{
    public interface IMerchantService
    {
        Task<MerchantResponse> GetMerchant(Guid merchantId, CancellationToken cancellationToken);
    }

    public class MerchantService : IMerchantService
    {
        public Task<MerchantResponse> GetMerchant(Guid merchantId, CancellationToken cancellationToken)
        {
            var merchantResponse = new MerchantResponse
            {
                Id = merchantId,
                Name = "Merchant Billing Name",
                BankAccount = new BankAccount
                {
                    BankName = "The Merchant's Bank",
                    SortCode = "334343",
                    AccountNumber = "945453443"
                }
            };

            return Task.FromResult(merchantResponse);
        }
    }
}
