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

        private const string AcquireEndpoint = "acquire";

        public CardAcquirerBankClient(HttpClient httpClient, IOptions<CardAcquirerBankSettings> options)
        {
            _httpClient = httpClient;

            var acquirerBankSettings = options.Value;
            _httpClient.BaseAddress = new Uri(acquirerBankSettings.BaseUrl);
        }

        public async Task<CardPaymentAcquirerResponse> AcquirePayment(CardPaymentAcquirerRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(AcquireEndpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CardPaymentAcquirerResponse>(cancellationToken: cancellationToken);
        }
    }
}