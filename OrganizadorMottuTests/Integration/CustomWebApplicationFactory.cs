using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrganizadorMottu;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using OrganizadorMottu.OrganizadorMottu.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OrganizadorMottuTests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6",
                    ["Jwt:Issuer"] = "MottuApi",
                    ["Jwt:Audience"] = "MottuMobile"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove completamente registros relacionados ao Oracle e DbContexts existentes
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                services.RemoveAll(typeof(DbContext));

                // Adiciona o banco de dados InMemory
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Garante que HTTPS não interfira nos testes
                services.PostConfigure<HttpsRedirectionOptions>(o => o.HttpsPort = 443);
            });
        }
    }
}