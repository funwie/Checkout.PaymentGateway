using CardPaymentAcquirerBank.Sdk.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CardPaymentAcquirerBank.Sdk
{
    public class CardAcquirerBankClient : ICardAcquirerBankClient
    {
        private readonly HttpClient _httpClient;
        private readonly CardAcquirerBankSettings _acquirerBankSettings;

        private readonly string AcquireEndpoint = "acquire";

        public CardAcquirerBankClient(HttpClient httpClient, IOptions<CardAcquirerBankSettings> options)
        {
            _httpClient = httpClient;
            _acquirerBankSettings = options.Value;

            _httpClient.BaseAddress = new Uri(_acquirerBankSettings.BaseUrl);
        }

        public async Task<CardPaymentAcquirerResponse> AcquirePayment(CardPaymentAcquirerRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(AcquireEndpoint, request, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CardPaymentAcquirerResponse>(cancellationToken: cancellationToken);

            //return Task.FromResult(new CardPaymentAcquirerResponse
            //{
            //    Name = "Card Acquirer",
            //    Approved = true,
            //    Status = "Authorized",
            //    TransactionReference = Guid.NewGuid().ToString(),
            //    PerformedOn = DateTime.UtcNow
            //});
        }
    }
}