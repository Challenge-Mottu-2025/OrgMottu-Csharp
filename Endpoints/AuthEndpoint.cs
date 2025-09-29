using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Models;
using Mottu.Api.Services;
using BCrypt.Net;

namespace Mottu.Api.Endpoints;

public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async ([FromBody] RegisterDto dto, AppDbContext db, JwtTokenService jwt) =>
        {
            if (await db.Usuarios.AnyAsync(u => u.Cpf == dto.Cpf))
                return Results.Conflict("Usuário já existe.");

            var user = new Usuario
            {
                Cpf = dto.Cpf,
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                NrCep = dto.NrCep,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha, workFactor: 11)
            };

            db.Usuarios.Add(user);
            await db.SaveChangesAsync();

            var (token, exp) = jwt.Generate(user);
            return Results.Created($"/api/usuarios/{user.Cpf}", new AuthResponseDto(user.Cpf, user.Nome, token, exp));
        })
        .Produces<AuthResponseDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login", async ([FromBody] LoginDto dto, AppDbContext db, JwtTokenService jwt) =>
        {
            var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Cpf == dto.Cpf);
            if (user is null)
                return Results.Unauthorized();

            var ok = BCrypt.Net.BCrypt.Verify(dto.Senha, user.SenhaHash);
            if (!ok)
                return Results.Unauthorized();

            var (token, exp) = jwt.Generate(user);
            return Results.Ok(new AuthResponseDto(user.Cpf, user.Nome, token, exp));
        })
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}