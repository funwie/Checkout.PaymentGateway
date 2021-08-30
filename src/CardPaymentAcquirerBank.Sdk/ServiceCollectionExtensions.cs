using Microsoft.Extensions.DependencyInjection;

namespace CardPaymentAcquirerBank.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCardAcquirerBankClient(this IServiceCollection services)
        {
            services.AddHttpClient<ICardAcquirerBankClient, CardAcquirerBankClient>();
            return services;
        }
    }
}
