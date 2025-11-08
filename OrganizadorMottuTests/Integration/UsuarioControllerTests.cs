using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace OrganizadorMottuTests.Integration
{
    public class UsuarioControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UsuarioControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "GET /api/1.0/usuario deve retornar 200 OK")]
        public async Task GetAllUsuarios_DeveRetornarOk()
        {
            // Act
            var response = await _client.GetAsync("/api/1.0/usuario");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}