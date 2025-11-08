using Microsoft.AspNetCore.Mvc;
using OrganizadorMottu.Application.Dtos;
using OrganizadorMottu.Services;
using OrganizadorMottu.Infrastructure.Repositories;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/1.0/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly JwtTokenService _jwt;

    public AuthController(IRepository<Usuario> usuarioRepository, JwtTokenService jwt)
    {
        _usuarioRepository = usuarioRepository;
        _jwt = jwt;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var existingUser = (await _usuarioRepository.GetAllAsync())
            .FirstOrDefault(u => u.Cpf == dto.Cpf);

        if (existingUser is not null)
            return Conflict("Usuário já existe.");

        var user = new Usuario
        {
            Cpf = dto.Cpf,
            Nome = dto.Nome,
            DataNascimento = dto.DataNascimento,
            NrCep = dto.NrCep,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha, workFactor: 11)
        };

        await _usuarioRepository.AddAsync(user);
        await _usuarioRepository.SaveChangesAsync();

        var (token, exp) = _jwt.Generate(user);
        return Created($"/api/usuarios/{user.Cpf}", new AuthResponseDto(user.Cpf, user.Nome, token, exp));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = (await _usuarioRepository.GetAllAsync())
            .FirstOrDefault(u => u.Cpf == dto.Cpf);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Senha, user.SenhaHash))
            return Unauthorized();

        var (token, exp) = _jwt.Generate(user);
        return Ok(new AuthResponseDto(user.Cpf, user.Nome, token, exp));
    }
}
