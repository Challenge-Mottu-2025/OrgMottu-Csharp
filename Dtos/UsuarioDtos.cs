using System.ComponentModel.DataAnnotations;

namespace Mottu.Api.Dtos;

public record UsuarioCreateDto(
    [Required, StringLength(11, MinimumLength = 11)] string Cpf,
    [Required, StringLength(50)] string Nome,
    DateTime? DataNascimento,
    [StringLength(8, MinimumLength = 8)] string? NrCep,
    [StringLength(7, MinimumLength = 7)] string? CdPlaca);

public record UsuarioUpdateDto(
    [Required, StringLength(50)] string Nome,
    DateTime? DataNascimento,
    [StringLength(8, MinimumLength = 8)] string? NrCep,
    [StringLength(7, MinimumLength = 7)] string? CdPlaca);

public record UsuarioResponseDto(
    string Cpf,
    string Nome,
    DateTime? DataNascimento,
    string? NrCep,
    string? CdPlaca);