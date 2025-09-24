using System.ComponentModel.DataAnnotations;

namespace Mottu.Api.Dtos;

public record EnderecoCreateDto(
    [Required, StringLength(8, MinimumLength = 8)] string NrCep,
    string? IdPais,
    [StringLength(2, MinimumLength = 2)] string? SiglaEstado,
    string? IdCidade,
    string? IdBairro,
    string? NrNumero,
    string? Logradouro,
    string? Complemento);

public record EnderecoUpdateDto(
    string? IdPais,
    [StringLength(2, MinimumLength = 2)] string? SiglaEstado,
    string? IdCidade,
    string? IdBairro,
    string? NrNumero,
    string? Logradouro,
    string? Complemento);

public record EnderecoResponseDto(
    string NrCep,
    string? IdPais,
    string? SiglaEstado,
    string? IdCidade,
    string? IdBairro,
    string? NrNumero,
    string? Logradouro,
    string? Complemento);