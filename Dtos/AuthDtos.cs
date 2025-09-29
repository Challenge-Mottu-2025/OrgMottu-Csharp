using System.ComponentModel.DataAnnotations;

namespace Mottu.Api.Dtos;

public record RegisterDto(
    [Required, StringLength(11, MinimumLength = 11)] string Cpf,
    [Required, StringLength(50)] string Nome,
    [Required, StringLength(60, MinimumLength = 6)] string Senha,
    DateTime? DataNascimento,
    [StringLength(8, MinimumLength = 8)] string? NrCep);

public record LoginDto(
    [Required, StringLength(11, MinimumLength = 11)] string Cpf,
    [Required, StringLength(60, MinimumLength = 6)] string Senha);

public record AuthResponseDto(
    string Cpf,
    string Nome,
    string Token,
    DateTime ExpiresAt);