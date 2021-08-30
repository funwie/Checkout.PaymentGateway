using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace PaymentGateway.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/healthcheck");

            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
