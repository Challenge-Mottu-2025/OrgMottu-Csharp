using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Application.Dtos;
using Mottu.Api.Hateoas;
using Mottu.Api.Domain.Entity;
using Mottu.Api.Infrastructure.Repositories;
using Microsoft.ML;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Mottu.Api.Controllers;

[ApiController]
[Route("api/{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
public class UsuarioController : ControllerBase
{
    private readonly IRepository<Usuario> _repository;
    private readonly LinkBuilder _links;

    public UsuarioController(IRepository<Usuario> repository, LinkBuilder links)
    {
        _repository = repository;
        _links = links;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PagedResult<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _repository.GetAllAsync()).AsQueryable().OrderBy(u => u.Cpf);
        var total = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new UsuarioResponseDto(u.Cpf, u.Nome, u.DataNascimento, u.NrCep, u.CdPlaca))
            .ToList();

        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var result = new PagedResult<UsuarioResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };

        result.Links.Add(_links.Self($"/api/{version}/usuarios?page={page}&pageSize={pageSize}"));
        if ((page - 1) * pageSize > 0)
            result.Links.Add(_links.Action("prev", $"/api/{version}/usuarios?page={page - 1}&pageSize={pageSize}", "GET"));
        if (page * pageSize < total)
            result.Links.Add(_links.Action("next", $"/api/{version}/usuarios?page={page + 1}&pageSize={pageSize}", "GET"));

        return Ok(result);
    }

    [HttpGet("{cpf}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetByCpf(string cpf)
    {
        var usuario = await _repository.GetByIdAsync(cpf);
        if (usuario is null) return NotFound();

        var dto = new UsuarioResponseDto(usuario.Cpf, usuario.Nome, usuario.DataNascimento, usuario.NrCep, usuario.CdPlaca);
        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var res = new Resource<UsuarioResponseDto>(dto);
        res.Links.Add(_links.Self($"/api/{version}/usuarios/{cpf}"));
        res.Links.Add(_links.Action("update", $"/api/{version}/usuarios/{cpf}", "PUT"));
        res.Links.Add(_links.Action("delete", $"/api/{version}/usuarios/{cpf}", "DELETE"));

        return Ok(res);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] UsuarioCreateDto dto)
    {
        var exists = (await _repository.GetAllAsync()).Any(u => u.Cpf == dto.Cpf);
        if (exists) return Conflict($"Usuario {dto.Cpf} já existe.");

        var usuario = new Usuario
        {
            Cpf = dto.Cpf,
            Nome = dto.Nome,
            DataNascimento = dto.DataNascimento,
            NrCep = dto.NrCep,
            CdPlaca = dto.CdPlaca
        };

        await _repository.AddAsync(usuario);
        await _repository.SaveChangesAsync();

        var resDto = new UsuarioResponseDto(usuario.Cpf, usuario.Nome, usuario.DataNascimento, usuario.NrCep, usuario.CdPlaca);
        var version = HttpContext.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion?.ToString() ?? "1.0";

        var resource = new Resource<UsuarioResponseDto>(resDto);
        resource.Links.Add(_links.Self($"/api/{version}/usuarios/{usuario.Cpf}"));

        return Created($"/api/{version}/usuarios/{usuario.Cpf}", resource);
    }

    [HttpPut("{cpf}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(string cpf, [FromBody] UsuarioUpdateDto dto)
    {
        var usuario = await _repository.GetByIdAsync(cpf);
        if (usuario is null) return NotFound();

        usuario.Nome = dto.Nome;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.NrCep = dto.NrCep;
        usuario.CdPlaca = dto.CdPlaca;

        _repository.Update(usuario);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{cpf}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(string cpf)
    {
        var usuario = await _repository.GetByIdAsync(cpf);
        if (usuario is null) return NotFound();

        _repository.Delete(usuario);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    // v2 - Previsão de Confiabilidade
    [HttpPost("prever-confiabilidade")]
    [MapToApiVersion("2.0")]
    public IActionResult PreverConfiabilidade([FromBody] UsuarioReliabilityInput input)
    {
        var ml = new MLContext();

        var trainingData = new List<UsuarioReliabilityInput>
        {
            new() { EntregasRealizadas = 100, MediaAvaliacoes = 4.9f, Infracoes = 0 },
            new() { EntregasRealizadas = 50, MediaAvaliacoes = 4.5f, Infracoes = 1 },
            new() { EntregasRealizadas = 10, MediaAvaliacoes = 3.5f, Infracoes = 4 },
            new() { EntregasRealizadas = 70, MediaAvaliacoes = 4.0f, Infracoes = 2 },
            new() { EntregasRealizadas = 150, MediaAvaliacoes = 5.0f, Infracoes = 0 }
        };

        var dataView = ml.Data.LoadFromEnumerable(trainingData);

        var pipeline = ml.Transforms.Concatenate("Features",
                nameof(UsuarioReliabilityInput.EntregasRealizadas),
                nameof(UsuarioReliabilityInput.MediaAvaliacoes),
                nameof(UsuarioReliabilityInput.Infracoes))
            .Append(ml.Regression.Trainers.Sdca(labelColumnName: nameof(UsuarioReliabilityInput.EntregasRealizadas)));

        var model = pipeline.Fit(dataView);
        var engine = ml.Model.CreatePredictionEngine<UsuarioReliabilityInput, UsuarioReliabilityOutput>(model);

        var prediction = engine.Predict(input);
        var score = Math.Clamp(prediction.ScoreConfiabilidade, 0, 100);

        return Ok(new
        {
            input.EntregasRealizadas,
            input.MediaAvaliacoes,
            input.Infracoes,
            ScoreConfiabilidade = Math.Round(score, 2)
        });
    }
}
