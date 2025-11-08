using Microsoft.AspNetCore.Mvc;
using Mottu.Api.Application.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Domain.Entity;
using Mottu.Api.Infrastructure.Repositories;

namespace Mottu.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/1.0/motos")]
[Produces("application/json")]
public class MotoController : ControllerBase
{
    private readonly IRepository<Moto> _repository;
    private readonly LinkBuilder _links;

    public MotoController(IRepository<Moto> repository, LinkBuilder links)
    {
        _repository = repository;
        _links = links;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MotoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _repository.GetAllAsync()).AsQueryable().OrderBy(m => m.Placa);
        var total = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(m => new MotoResponseDto(m.Placa, m.Cpf, m.Nv, m.Motor, m.Renavam, m.Fipe))
            .ToList();

        var result = new PagedResult<MotoResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };

        result.Links.Add(_links.Self($"/api/motos?page={page}&pageSize={pageSize}"));
        if ((page - 1) * pageSize > 0)
            result.Links.Add(_links.Action("prev", $"/api/motos?page={page - 1}&pageSize={pageSize}", "GET"));
        if (page * pageSize < total)
            result.Links.Add(_links.Action("next", $"/api/motos?page={page + 1}&pageSize={pageSize}", "GET"));

        return Ok(result);
    }

    [HttpGet("{placa}")]
    [ProducesResponseType(typeof(Resource<MotoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPlaca(string placa)
    {
        var moto = (await _repository.GetAllAsync()).FirstOrDefault(m => m.Placa == placa);
        if (moto is null) return NotFound();

        var dto = new MotoResponseDto(moto.Placa, moto.Cpf, moto.Nv, moto.Motor, moto.Renavam, moto.Fipe);
        var res = new Resource<MotoResponseDto>(dto);
        res.Links.Add(_links.Self($"/api/motos/{placa}"));
        res.Links.Add(_links.Action("update", $"/api/motos/{placa}", "PUT"));
        res.Links.Add(_links.Action("delete", $"/api/motos/{placa}", "DELETE"));

        return Ok(res);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Resource<MotoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] MotoCreateDto dto)
    {
        var exists = (await _repository.GetAllAsync()).Any(m => m.Placa == dto.Placa);
        if (exists)
            return Conflict($"Moto {dto.Placa} já existe.");

        var model = new Moto
        {
            Placa = dto.Placa,
            Cpf = dto.Cpf,
            Nv = dto.Nv,
            Motor = dto.Motor,
            Renavam = dto.Renavam,
            Fipe = dto.Fipe
        };

        await _repository.AddAsync(model);
        await _repository.SaveChangesAsync();

        var resDto = new MotoResponseDto(model.Placa, model.Cpf, model.Nv, model.Motor, model.Renavam, model.Fipe);
        var resource = new Resource<MotoResponseDto>(resDto);
        resource.Links.Add(_links.Self($"/api/motos/{model.Placa}"));

        return Created($"/api/motos/{model.Placa}", resource);
    }

    [HttpPut("{placa}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string placa, [FromBody] MotoUpdateDto dto)
    {
        var moto = (await _repository.GetAllAsync()).FirstOrDefault(m => m.Placa == placa);
        if (moto is null) return NotFound();

        moto.Cpf = dto.Cpf;
        moto.Nv = dto.Nv;
        moto.Motor = dto.Motor;
        moto.Renavam = dto.Renavam;
        moto.Fipe = dto.Fipe;

        _repository.Update(moto);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{placa}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string placa)
    {
        var moto = (await _repository.GetAllAsync()).FirstOrDefault(m => m.Placa == placa);
        if (moto is null) return NotFound();

        _repository.Delete(moto);
        await _repository.SaveChangesAsync();

        return NoContent();
    }
}