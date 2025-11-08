using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Mottu.Api.Application.Dtos;

public record RegisterDto(
    [Required, StringLength(11, MinimumLength = 11)]
    [SwaggerSchema("CPF do usuário (11 dígitos, obrigatório)")]
    string Cpf,

    [Required, StringLength(50)]
    [SwaggerSchema("Nome completo do usuário")]
    string Nome,

    [Required, StringLength(60, MinimumLength = 6)]
    [SwaggerSchema("Senha com no mínimo 6 e no máximo 60 caracteres")]
    string Senha,

    [SwaggerSchema("Data de nascimento do usuário")]
    DateTime? DataNascimento,

    [StringLength(8, MinimumLength = 8)]
    [SwaggerSchema("CEP de residência (8 dígitos, opcional)")]
    string? NrCep
);

public record LoginDto(
    [Required, StringLength(11, MinimumLength = 11)]
    [SwaggerSchema("CPF do usuário (11 dígitos)")]
    string Cpf,

    [Required, StringLength(60, MinimumLength = 6)]
    [SwaggerSchema("Senha do usuário")]
    string Senha
);

public record AuthResponseDto(
    [SwaggerSchema("CPF do usuário autenticado")]
    string Cpf,

    [SwaggerSchema("Nome do usuário")]
    string Nome,

    [SwaggerSchema("Token JWT gerado após autenticação")]
    string Token,

    [SwaggerSchema("Data e hora de expiração do token")]
    DateTime ExpiresAt
);
