using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Endpoints;
using Mottu.Api.Services;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Oracle.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// DbContext (Oracle)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Oracle")
             ?? throw new InvalidOperationException("Connection string 'Oracle' is missing.");
    options.UseOracle(cs);
});

// JWT
builder.Services.AddSingleton<JwtTokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
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

// Health Check
builder.Services.AddHealthChecks();

// Swagger + examples
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc(builder.Configuration["Swagger:Version"] ?? "v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"] ?? "API",
        Version = builder.Configuration["Swagger:Version"] ?? "v1",
        Description = "API RESTful em .NET 8 com boas práticas (paginação, HATEOAS, códigos HTTP)."
    });
    o.EnableAnnotations();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// HATEOAS helpers
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LinkBuilder>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Endpoint groups
app.MapUsuarioEndpoints();
app.MapMotoEndpoints();
app.MapEnderecoEndpoints();

// Endpoint de Health Check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program { }