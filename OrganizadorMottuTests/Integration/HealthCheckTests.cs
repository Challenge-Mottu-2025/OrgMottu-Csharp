using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace OrganizadorMottuTests.Integration
{
    public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthCheckTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "GET /health deve retornar 200 OK")]
        public async Task Health_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/health");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}