using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Mottu.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static IEndpointRouteBuilder MapUsuarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/usuarios").WithTags("Usuarios");

        // GET paginado
        group.MapGet("/", async(AppDbContext db, LinkBuilder links, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
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
        group.MapGet("/{cpf}", async (string cpf, AppDbContext db, LinkBuilder links) =>
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
        group.MapPost("/", async ([FromBody] UsuarioCreateDto dto, AppDbContext db, LinkBuilder links) =>
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
        group.MapPut("/{cpf}", async (string cpf, [FromBody] UsuarioUpdateDto dto, AppDbContext db) =>
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
        group.MapDelete("/{cpf}", async (string cpf, AppDbContext db) =>
        {
            var model = await db.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (model is null) return Results.NotFound();
            db.Usuarios.Remove(model);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}