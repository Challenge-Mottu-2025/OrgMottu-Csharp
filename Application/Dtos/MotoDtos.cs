using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Mottu.Api.Application.Dtos;

public record MotoCreateDto(
    [Required, StringLength(7, MinimumLength = 7)]
    [SwaggerSchema("Placa da moto (formato com 7 caracteres obrigatórios)")]
    string Placa,

    [StringLength(11, MinimumLength = 11)]
    [SwaggerSchema("CPF do proprietário vinculado à moto (11 dígitos, opcional)")]
    string? Cpf,

    [SwaggerSchema("Número de série (NV) da moto, opcional")]
    string? Nv,

    [SwaggerSchema("Código do motor ou especificação da moto")]
    string? Motor,

    [SwaggerSchema("Número do Renavam (Registro Nacional de Veículos Automotores)")]
    long? Renavam,

    [SwaggerSchema("Valor de referência da Tabela FIPE")]
    int? Fipe
);

public record MotoUpdateDto(
    [StringLength(11, MinimumLength = 11)]
    [SwaggerSchema("Novo CPF associado à moto (11 dígitos, opcional)")]
    string? Cpf,

    [SwaggerSchema("Novo número de série da moto (NV), se aplicável")]
    string? Nv,

    [SwaggerSchema("Novo motor ou código de motor")]
    string? Motor,

    [SwaggerSchema("Novo número Renavam")]
    long? Renavam,

    [SwaggerSchema("Novo valor da Tabela FIPE")]
    int? Fipe
);

public record MotoResponseDto(
    [SwaggerSchema("Placa da moto cadastrada")]
    string Placa,

    [SwaggerSchema("CPF do proprietário associado")]
    string? Cpf,

    [SwaggerSchema("Número de série (NV) da moto")]
    string? Nv,

    [SwaggerSchema("Especificação ou tipo de motor")]
    string? Motor,

    [SwaggerSchema("Registro Renavam do veículo")]
    long? Renavam,

    [SwaggerSchema("Valor FIPE da moto")]
    int? Fipe
);
