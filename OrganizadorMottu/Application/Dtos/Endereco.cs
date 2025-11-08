using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace OrganizadorMottu.Application.Dtos;

public record EnderecoCreateDto(
    [Required, StringLength(8, MinimumLength = 8)]
    [SwaggerSchema("Número do CEP (8 dígitos obrigatórios, sem hífen)")]
    string NrCep,

    [SwaggerSchema("Identificador do país (opcional)")]
    string? IdPais,

    [StringLength(2, MinimumLength = 2)]
    [SwaggerSchema("Sigla do estado (ex: SP, RJ)")]
    string? SiglaEstado,

    [SwaggerSchema("Identificador da cidade (opcional)")]
    string? IdCidade,

    [SwaggerSchema("Identificador do bairro (opcional)")]
    string? IdBairro,

    [SwaggerSchema("Número da residência ou estabelecimento (opcional)")]
    string? NrNumero,

    [SwaggerSchema("Nome do logradouro, como rua ou avenida (opcional)")]
    string? Logradouro,

    [SwaggerSchema("Complemento do endereço (ex: apto, bloco)")]
    string? Complemento
);

public record EnderecoUpdateDto(
    [SwaggerSchema("Novo identificador do país")]
    string? IdPais,

    [StringLength(2, MinimumLength = 2)]
    [SwaggerSchema("Nova sigla do estado (ex: SP, RJ)")]
    string? SiglaEstado,

    [SwaggerSchema("Novo identificador da cidade")]
    string? IdCidade,

    [SwaggerSchema("Novo identificador do bairro")]
    string? IdBairro,

    [SwaggerSchema("Novo número do endereço")]
    string? NrNumero,

    [SwaggerSchema("Novo logradouro (rua, avenida, etc.)")]
    string? Logradouro,

    [SwaggerSchema("Novo complemento (bloco, apartamento, etc.)")]
    string? Complemento
);

public record EnderecoResponseDto(
    [SwaggerSchema("CEP cadastrado (8 dígitos)")]
    string NrCep,

    [SwaggerSchema("Identificador do país")]
    string? IdPais,

    [SwaggerSchema("Sigla do estado (ex: SP)")]
    string? SiglaEstado,

    [SwaggerSchema("Identificador da cidade")]
    string? IdCidade,

    [SwaggerSchema("Identificador do bairro")]
    string? IdBairro,

    [SwaggerSchema("Número da residência ou ponto de entrega")]
    string? NrNumero,

    [SwaggerSchema("Nome do logradouro")]
    string? Logradouro,

    [SwaggerSchema("Complemento do endereço")]
    string? Complemento
);