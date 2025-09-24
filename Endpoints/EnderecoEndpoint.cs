using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Mottu.Api.Endpoints;

public static class EnderecoEndpoints
{
    public static IEndpointRouteBuilder MapEnderecoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/enderecos").WithTags("Enderecos");

        group.MapGet("/", async (AppDbContext db, LinkBuilder links, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = db.Enderecos.AsNoTracking().OrderBy(e => e.NrCep);
            var total = await query.LongCountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(e => new EnderecoResponseDto(e.NrCep, e.IdPais, e.SiglaEstado, e.IdCidade, e.IdBairro, e.NrNumero, e.Logradouro, e.Complemento))
                .ToListAsync();

            var result = new PagedResult<EnderecoResponseDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
            result.Links.Add(links.Self($"/api/enderecos?page={page}&pageSize={pageSize}"));
            if ((page - 1) * pageSize > 0)
                result.Links.Add(links.Action("prev", $"/api/enderecos?page={page - 1}&pageSize={pageSize}", "GET"));
            if (page * pageSize < total)
                result.Links.Add(links.Action("next", $"/api/enderecos?page={page + 1}&pageSize={pageSize}", "GET"));

            return Results.Ok(result);
        })
        .Produces<PagedResult<EnderecoResponseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{nrCep}", async (string nrCep, AppDbContext db, LinkBuilder links) =>
        {
            var e = await db.Enderecos.AsNoTracking().FirstOrDefaultAsync(x => x.NrCep == nrCep);
            if (e is null) return Results.NotFound();
            var dto = new EnderecoResponseDto(e.NrCep, e.IdPais, e.SiglaEstado, e.IdCidade, e.IdBairro, e.NrNumero, e.Logradouro, e.Complemento);
            var res = new Resource<EnderecoResponseDto>(dto);
            res.Links.Add(links.Self($"/api/enderecos/{nrCep}"));
            res.Links.Add(links.Action("update", $"/api/enderecos/{nrCep}", "PUT"));
            res.Links.Add(links.Action("delete", $"/api/enderecos/{nrCep}", "DELETE"));
            return Results.Ok(res);
        })
        .Produces<Resource<EnderecoResponseDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromBody] EnderecoCreateDto dto, AppDbContext db, LinkBuilder links) =>
        {
            if (await db.Enderecos.AnyAsync(e => e.NrCep == dto.NrCep))
                return Results.Conflict($"Endereço {dto.NrCep} já existe.");

            var model = new Endereco
            {
                NrCep = dto.NrCep,
                IdPais = dto.IdPais,
                SiglaEstado = dto.SiglaEstado,
                IdCidade = dto.IdCidade,
                IdBairro = dto.IdBairro,
                NrNumero = dto.NrNumero,
                Logradouro = dto.Logradouro,
                Complemento = dto.Complemento
            };
            db.Enderecos.Add(model);
            await db.SaveChangesAsync();

            var resDto = new EnderecoResponseDto(model.NrCep, model.IdPais, model.SiglaEstado, model.IdCidade, model.IdBairro, model.NrNumero, model.Logradouro, model.Complemento);
            var resource = new Resource<EnderecoResponseDto>(resDto);
            resource.Links.Add(links.Self($"/api/enderecos/{model.NrCep}"));
            return Results.Created($"/api/enderecos/{model.NrCep}", resource);
        })
        .Produces<Resource<EnderecoResponseDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{nrCep}", async (string nrCep, [FromBody] EnderecoUpdateDto dto, AppDbContext db) =>
        {
            var e = await db.Enderecos.FirstOrDefaultAsync(x => x.NrCep == nrCep);
            if (e is null) return Results.NotFound();

            e.IdPais = dto.IdPais;
            e.SiglaEstado = dto.SiglaEstado;
            e.IdCidade = dto.IdCidade;
            e.IdBairro = dto.IdBairro;
            e.NrNumero = dto.NrNumero;
            e.Logradouro = dto.Logradouro;
            e.Complemento = dto.Complemento;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{nrCep}", async (string nrCep, AppDbContext db) =>
        {
            var e = await db.Enderecos.FirstOrDefaultAsync(x => x.NrCep == nrCep);
            if (e is null) return Results.NotFound();

            db.Enderecos.Remove(e);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}