using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.ML;

namespace Mottu.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static IEndpointRouteBuilder MapUsuarioEndpoints(this IEndpointRouteBuilder app)
    {
        // VERSÃO 1 - API principal.
        var v1 = app.MapGroup("/api/usuarios").WithTags("Usuarios");

        // GET paginado
        v1.MapGet("/", async (AppDbContext db, LinkBuilder links, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = db.Usuarios.AsNoTracking().OrderBy(u => u.Cpf);
            var total = await query.LongCountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(u => new UsuarioResponseDto(u.Cpf, u.Nome, u.DataNascimento, u.NrCep, u.CdPlaca))
                .ToListAsync();

            var result = new PagedResult<UsuarioResponseDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };

            result.Links.Add(links.Self($"/api/usuarios?page={page}&pageSize={pageSize}"));
            if ((page - 1) * pageSize > 0)
                result.Links.Add(links.Action("prev", $"/api/usuarios?page={page - 1}&pageSize={pageSize}", "GET"));
            if (page * pageSize < total)
                result.Links.Add(links.Action("next", $"/api/usuarios?page={page + 1}&pageSize={pageSize}", "GET"));

            return Results.Ok(result);
        })
        .Produces<PagedResult<UsuarioResponseDto>>(StatusCodes.Status200OK)
        .WithName("ListUsuarios");

        // GET by id
        v1.MapGet("/{cpf}", async (string cpf, AppDbContext db, LinkBuilder links) =>
        {
            var u = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Cpf == cpf);
            if (u is null) return Results.NotFound();

            var dto = new UsuarioResponseDto(u.Cpf, u.Nome, u.DataNascimento, u.NrCep, u.CdPlaca);
            var res = new Resource<UsuarioResponseDto>(dto);
            res.Links.Add(links.Self($"/api/usuarios/{cpf}"));
            res.Links.Add(links.Action("update", $"/api/usuarios/{cpf}", "PUT"));
            res.Links.Add(links.Action("delete", $"/api/usuarios/{cpf}", "DELETE"));
            return Results.Ok(res);
        })
        .Produces<Resource<UsuarioResponseDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST
        v1.MapPost("/", async ([FromBody] UsuarioCreateDto dto, AppDbContext db, LinkBuilder links) =>
        {
            if (await db.Usuarios.AnyAsync(u => u.Cpf == dto.Cpf))
                return Results.Conflict($"Usuario {dto.Cpf} já existe.");

            var model = new Usuario
            {
                Cpf = dto.Cpf,
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                NrCep = dto.NrCep,
                CdPlaca = dto.CdPlaca
            };

            db.Usuarios.Add(model);
            await db.SaveChangesAsync();

            var resDto = new UsuarioResponseDto(model.Cpf, model.Nome, model.DataNascimento, model.NrCep, model.CdPlaca);
            var resource = new Resource<UsuarioResponseDto>(resDto);
            resource.Links.Add(links.Self($"/api/usuarios/{model.Cpf}"));
            return Results.Created($"/api/usuarios/{model.Cpf}", resource);
        })
        .Produces<Resource<UsuarioResponseDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status400BadRequest);

        // PUT
        v1.MapPut("/{cpf}", async (string cpf, [FromBody] UsuarioUpdateDto dto, AppDbContext db) =>
        {
            var model = await db.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (model is null) return Results.NotFound();

            model.Nome = dto.Nome;
            model.DataNascimento = dto.DataNascimento;
            model.NrCep = dto.NrCep;
            model.CdPlaca = dto.CdPlaca;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE
        v1.MapDelete("/{cpf}", async (string cpf, AppDbContext db) =>
        {
            var model = await db.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (model is null) return Results.NotFound();
            db.Usuarios.Remove(model);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // Versão 2 - com ML.NET

        var v2 = app.MapGroup("/api/v2/usuarios").WithTags("Usuarios v2");

        v2.MapPost("/prever-confiabilidade", (UsuarioReliabilityInput input) =>
        {
            var ml = new MLContext();

            // Dados simulados para treinamento
            var data = new List<UsuarioReliabilityInput>
            {
                new() { EntregasRealizadas = 100, MediaAvaliacoes = 4.9f, Infracoes = 0 },
                new() { EntregasRealizadas = 50, MediaAvaliacoes = 4.5f, Infracoes = 1 },
                new() { EntregasRealizadas = 10, MediaAvaliacoes = 3.5f, Infracoes = 4 },
                new() { EntregasRealizadas = 70, MediaAvaliacoes = 4.0f, Infracoes = 2 },
                new() { EntregasRealizadas = 150, MediaAvaliacoes = 5.0f, Infracoes = 0 }
            };

            var trainingData = ml.Data.LoadFromEnumerable(data);

            var pipeline = ml.Transforms.Concatenate("Features",
                nameof(UsuarioReliabilityInput.EntregasRealizadas),
                nameof(UsuarioReliabilityInput.MediaAvaliacoes),
                nameof(UsuarioReliabilityInput.Infracoes))
                .Append(ml.Regression.Trainers.Sdca(labelColumnName: nameof(UsuarioReliabilityInput.EntregasRealizadas)));

            var model = pipeline.Fit(trainingData);

            var engine = ml.Model.CreatePredictionEngine<UsuarioReliabilityInput, UsuarioReliabilityOutput>(model);

            var prediction = engine.Predict(input);

            // Escala simplificada de 0–100 para o Score
            var score = Math.Clamp(prediction.ScoreConfiabilidade, 0, 100);

            return Results.Ok(new
            {
                input.EntregasRealizadas,
                input.MediaAvaliacoes,
                input.Infracoes,
                ScoreConfiabilidade = Math.Round(score, 2)
            });
        })
        .WithSummary("Usa ML.NET para prever o score de confiabilidade de um cliente/entregador com base nas entregas, avaliações e infrações.")
        .Produces(StatusCodes.Status200OK)
        .WithName("PreverConfiabilidadeUsuario");

        return app;
    }
}