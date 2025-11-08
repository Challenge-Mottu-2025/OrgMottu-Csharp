using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Mottu.Api.Application.Dtos;

public record UsuarioCreateDto(
    [Required, StringLength(11, MinimumLength = 11)]
    [SwaggerSchema("CPF do usuário (somente números, exatamente 11 dígitos)")]
    string Cpf,

    [Required, StringLength(50)]
    [SwaggerSchema("Nome completo do usuário (até 50 caracteres)")]
    string Nome,

    [SwaggerSchema("Data de nascimento do usuário (formato ISO 8601)")]
    DateTime? DataNascimento,

    [StringLength(8, MinimumLength = 8)]
    [SwaggerSchema("CEP do usuário (8 dígitos, opcional)")]
    string? NrCep,

    [StringLength(7, MinimumLength = 7)]
    [SwaggerSchema("Placa da moto associada ao usuário (7 caracteres, opcional)")]
    string? CdPlaca
);

public record UsuarioUpdateDto(
    [Required, StringLength(50)]
    [SwaggerSchema("Nome atualizado do usuário")]
    string Nome,

    [SwaggerSchema("Nova data de nascimento do usuário")]
    DateTime? DataNascimento,

    [StringLength(8, MinimumLength = 8)]
    [SwaggerSchema("Novo CEP do usuário (opcional)")]
    string? NrCep,

    [StringLength(7, MinimumLength = 7)]
    [SwaggerSchema("Nova placa da moto associada ao usuário (opcional)")]
    string? CdPlaca
);

public record UsuarioResponseDto(
    [SwaggerSchema("CPF do usuário retornado")]
    string Cpf,

    [SwaggerSchema("Nome completo do usuário")]
    string Nome,

    [SwaggerSchema("Data de nascimento retornada")]
    DateTime? DataNascimento,

    [SwaggerSchema("CEP associado ao usuário")]
    string? NrCep,

    [SwaggerSchema("Placa da moto associada")]
    string? CdPlaca
);