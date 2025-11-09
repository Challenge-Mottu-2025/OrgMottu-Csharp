using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using OrganizadorMottu.Application.Dtos;
using OrganizadorMottu.Hateoas;
using OrganizadorMottu.Infrastructure.Repositories;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Controllers;

[ApiController]
[Route("api/{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
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

    // V1 - CRUD COMPLETO NORMAL

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PagedResult<EnderecoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _repository.GetAllAsync()).AsQueryable().OrderBy(e => e.NrCep);
        var total = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EnderecoResponseDto(e.NrCep, e.IdPais, e.SiglaEstado, e.IdCidade, e.IdBairro, e.NrNumero, e.Logradouro, e.Complemento))
            .ToList();

        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var result = new PagedResult<EnderecoResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };

        result.Links.Add(_links.Self($"/api/{version}/enderecos?page={page}&pageSize={pageSize}"));
        if ((page - 1) * pageSize > 0)
            result.Links.Add(_links.Action("prev", $"/api/{version}/enderecos?page={page - 1}&pageSize={pageSize}", "GET"));
        if (page * pageSize < total)
            result.Links.Add(_links.Action("next", $"/api/{version}/enderecos?page={page + 1}&pageSize={pageSize}", "GET"));

        return Ok(result);
    }

    [HttpGet("{nrCep}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByCep(string nrCep)
    {
        var endereco = await _repository.GetByIdAsync(nrCep);
        if (endereco is null) return NotFound();

        var dto = new EnderecoResponseDto(endereco.NrCep, endereco.IdPais, endereco.SiglaEstado, endereco.IdCidade,
                                          endereco.IdBairro, endereco.NrNumero, endereco.Logradouro, endereco.Complemento);

        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var res = new Resource<EnderecoResponseDto>(dto);
        res.Links.Add(_links.Self($"/api/{version}/enderecos/{nrCep}"));
        res.Links.Add(_links.Action("update", $"/api/{version}/enderecos/{nrCep}", "PUT"));
        res.Links.Add(_links.Action("delete", $"/api/{version}/enderecos/{nrCep}", "DELETE"));

        return Ok(res);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Resource<EnderecoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] EnderecoCreateDto dto)
    {
        var exists = (await _repository.GetAllAsync()).Any(e => e.NrCep == dto.NrCep);
        if (exists) return Conflict($"Endereço {dto.NrCep} já existe.");

        var endereco = new Endereco
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

        await _repository.AddAsync(endereco);
        await _repository.SaveChangesAsync();

        var resDto = new EnderecoResponseDto(endereco.NrCep, endereco.IdPais, endereco.SiglaEstado,
                                             endereco.IdCidade, endereco.IdBairro, endereco.NrNumero,
                                             endereco.Logradouro, endereco.Complemento);

        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var resource = new Resource<EnderecoResponseDto>(resDto);
        resource.Links.Add(_links.Self($"/api/{version}/enderecos/{endereco.NrCep}"));

        return Created($"/api/{version}/enderecos/{endereco.NrCep}", resource);
    }

    [HttpPut("{nrCep}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(string nrCep, [FromBody] EnderecoUpdateDto dto)
    {
        var endereco = await _repository.GetByIdAsync(nrCep);
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
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(string nrCep)
    {
        var endereco = await _repository.GetByIdAsync(nrCep);
        if (endereco is null) return NotFound();

        _repository.Delete(endereco);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    // V2 - INSERT VIA PROCEDURE ORACLE
    [HttpPost("procedure")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> CreateViaProcedure([FromBody] EnderecoCreateDto dto)
    {
        var parametros = new Dictionary<string, object>
        {
            { "p_nr_cep", dto.NrCep },
            { "p_id_pais", dto.IdPais! },
            { "p_sg_estado", dto.SiglaEstado! },
            { "p_id_cidade", dto.IdCidade! },
            { "p_id_bairro", dto.IdBairro! },
            { "p_nr_numero", dto.NrNumero },
            { "p_ds_logradouro", dto.Logradouro! },
            { "p_ds_complemento", dto.Complemento ?? ""}
        };

        await _repository.ExecutarProcedureAsync("pkg_mottu.pr_inserir_endereco", parametros);
        return Ok("Endereço inserido via procedure (v2).");
    }
}
