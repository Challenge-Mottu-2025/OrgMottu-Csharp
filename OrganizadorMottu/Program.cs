using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oracle.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using HealthChecks.UI.Client;
using OrganizadorMottu.Application.Dtos;
using OrganizadorMottu.Hateoas;
using OrganizadorMottu.Services;
using OrganizadorMottu.Infrastructure.Repositories;
using OrganizadorMottu.Infrastructure.Context;
using OrganizadorMottu.Configurations;
using OrganizadorMottu.Application.Swagger;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Banco de Dados (Oracle)
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var cs = builder.Configuration.GetConnectionString("Oracle")
                 ?? throw new InvalidOperationException("Connection string 'Oracle' is missing.");
        options.UseOracle(cs);
    });
}

// JWT Authentication
builder.Services.AddSingleton<JwtTokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

// Versionamento
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "VVV"; // Ex: 1.0, 2.0
    setup.SubstituteApiVersionInUrl = true;
});

// Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
    options.EnableAnnotations();
    options.SupportNonNullableReferenceTypes();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// Controllers + HATEOAS + Repositories
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LinkBuilder>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Middlewares
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionProvider.ApiVersionDescriptions)
    {
        var group = description.GroupName; // Ex: 1.0
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"OrgMottu API - {description.GroupName}");
    }
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

// Necessário para testes com WebApplicationFactory
public partial class Program { }
