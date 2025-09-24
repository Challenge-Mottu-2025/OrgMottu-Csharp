using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Mottu.Api.Endpoints;

public static class MotoEndpoints
{
    public static IEndpointRouteBuilder MapMotoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/motos").WithTags("Motos");

        group.MapGet("/", async (AppDbContext db, LinkBuilder links, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = db.Motos.AsNoTracking().OrderBy(m => m.Placa);
            var total = await query.LongCountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(m => new MotoResponseDto(m.Placa, m.Cpf, m.Nv, m.Motor, m.Renavam, m.Fipe))
                .ToListAsync();

            var result = new PagedResult<MotoResponseDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
            result.Links.Add(links.Self($"/api/motos?page={page}&pageSize={pageSize}"));
            if ((page - 1) * pageSize > 0)
                result.Links.Add(links.Action("prev", $"/api/motos?page={page - 1}&pageSize={pageSize}", "GET"));
            if (page * pageSize < total)
                result.Links.Add(links.Action("next", $"/api/motos?page={page + 1}&pageSize={pageSize}", "GET"));

            return Results.Ok(result);
        })
        .Produces<PagedResult<MotoResponseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{placa}", async (string placa, AppDbContext db, LinkBuilder links) =>
        {
            var m = await db.Motos.AsNoTracking().FirstOrDefaultAsync(x => x.Placa == placa);
            if (m is null) return Results.NotFound();
            var dto = new MotoResponseDto(m.Placa, m.Cpf, m.Nv, m.Motor, m.Renavam, m.Fipe);
            var res = new Resource<MotoResponseDto>(dto);
            res.Links.Add(links.Self($"/api/motos/{placa}"));
            res.Links.Add(links.Action("update", $"/api/motos/{placa}", "PUT"));
            res.Links.Add(links.Action("delete", $"/api/motos/{placa}", "DELETE"));
            return Results.Ok(res);
        })
        .Produces<Resource<MotoResponseDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromBody] MotoCreateDto dto, AppDbContext db, LinkBuilder links) =>
        {
            if (await db.Motos.AnyAsync(m => m.Placa == dto.Placa))
                return Results.Conflict($"Moto {dto.Placa} já existe.");

            var model = new Moto
            {
                Placa = dto.Placa,
                Cpf = dto.Cpf,
                Nv = dto.Nv,
                Motor = dto.Motor,
                Renavam = dto.Renavam,
                Fipe = dto.Fipe
            };
            db.Motos.Add(model);
            await db.SaveChangesAsync();

            var resDto = new MotoResponseDto(model.Placa, model.Cpf, model.Nv, model.Motor, model.Renavam, model.Fipe);
            var resource = new Resource<MotoResponseDto>(resDto);
            resource.Links.Add(links.Self($"/api/motos/{model.Placa}"));
            return Results.Created($"/api/motos/{model.Placa}", resource);
        })
        .Produces<Resource<MotoResponseDto>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPut("/{placa}", async (string placa, [FromBody] MotoUpdateDto dto, AppDbContext db) =>
        {
            var m = await db.Motos.FirstOrDefaultAsync(x => x.Placa == placa);
            if (m is null) return Results.NotFound();

            m.Cpf = dto.Cpf;
            m.Nv = dto.Nv;
            m.Motor = dto.Motor;
            m.Renavam = dto.Renavam;
            m.Fipe = dto.Fipe;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{placa}", async (string placa, AppDbContext db) =>
        {
            var m = await db.Motos.FirstOrDefaultAsync(x => x.Placa == placa);
            if (m is null) return Results.NotFound();
            db.Motos.Remove(m);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}