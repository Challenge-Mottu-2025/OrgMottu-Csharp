using Microsoft.AspNetCore.Mvc;
using OrganizadorMottu.Application.Dtos;
using OrganizadorMottu.Hateoas;
using OrganizadorMottu.Infrastructure.Repositories;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/1.0/enderecos")]
[Produces("application/json")]
public class EnderecoController : ControllerBase
{
    private readonly IRepository<Endereco> _repository;
    private readonly LinkBuilder _links;

    public EnderecoController(IRepository<Endereco> repository, LinkBuilder links)
    {
        _repository = repository;
        _links = links;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EnderecoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _repository.GetAllAsync()).AsQueryable().OrderBy(e => e.NrCep);
        var total = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EnderecoResponseDto(e.NrCep, e.IdPais, e.SiglaEstado, e.IdCidade, e.IdBairro, e.NrNumero, e.Logradouro, e.Complemento))
            .ToList();

        var result = new PagedResult<EnderecoResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };

        result.Links.Add(_links.Self($"/api/enderecos?page={page}&pageSize={pageSize}"));
        if ((page - 1) * pageSize > 0)
            result.Links.Add(_links.Action("prev", $"/api/enderecos?page={page - 1}&pageSize={pageSize}", "GET"));
        if (page * pageSize < total)
            result.Links.Add(_links.Action("next", $"/api/enderecos?page={page + 1}&pageSize={pageSize}", "GET"));

        return Ok(result);
    }

    [HttpGet("{nrCep}")]
    [ProducesResponseType(typeof(Resource<EnderecoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCep(string nrCep)
    {
        var endereco = (await _repository.GetAllAsync()).FirstOrDefault(e => e.NrCep == nrCep);
        if (endereco is null) return NotFound();

        var dto = new EnderecoResponseDto(endereco.NrCep, endereco.IdPais, endereco.SiglaEstado, endereco.IdCidade, endereco.IdBairro, endereco.NrNumero, endereco.Logradouro, endereco.Complemento);
        var res = new Resource<EnderecoResponseDto>(dto);
        res.Links.Add(_links.Self($"/api/enderecos/{nrCep}"));
        res.Links.Add(_links.Action("update", $"/api/enderecos/{nrCep}", "PUT"));
        res.Links.Add(_links.Action("delete", $"/api/enderecos/{nrCep}", "DELETE"));

        return Ok(res);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Resource<EnderecoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] EnderecoCreateDto dto)
    {
        var exists = (await _repository.GetAllAsync()).Any(e => e.NrCep == dto.NrCep);
        if (exists)
            return Conflict($"Endereço {dto.NrCep} já existe.");

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

        await _repository.AddAsync(model);
        await _repository.SaveChangesAsync();

        var resDto = new EnderecoResponseDto(model.NrCep, model.IdPais, model.SiglaEstado, model.IdCidade, model.IdBairro, model.NrNumero, model.Logradouro, model.Complemento);
        var resource = new Resource<EnderecoResponseDto>(resDto);
        resource.Links.Add(_links.Self($"/api/enderecos/{model.NrCep}"));

        return Created($"/api/enderecos/{model.NrCep}", resource);
    }

    [HttpPut("{nrCep}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string nrCep, [FromBody] EnderecoUpdateDto dto)
    {
        var endereco = (await _repository.GetAllAsync()).FirstOrDefault(e => e.NrCep == nrCep);
        if (endereco is null) return NotFound();

        endereco.IdPais = dto.IdPais;
        endereco.SiglaEstado = dto.SiglaEstado;
        endereco.IdCidade = dto.IdCidade;
        endereco.IdBairro = dto.IdBairro;
        endereco.NrNumero = dto.NrNumero;
        endereco.Logradouro = dto.Logradouro;
        endereco.Complemento = dto.Complemento;

        _repository.Update(endereco);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{nrCep}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string nrCep)
    {
        var endereco = (await _repository.GetAllAsync()).FirstOrDefault(e => e.NrCep == nrCep);
        if (endereco is null) return NotFound();

        _repository.Delete(endereco);
        await _repository.SaveChangesAsync();

        return NoContent();
    }
}
