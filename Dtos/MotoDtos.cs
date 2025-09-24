using System.ComponentModel.DataAnnotations;

namespace Mottu.Api.Dtos;

public record MotoCreateDto(
    [Required, StringLength(7, MinimumLength = 7)] string Placa,
    [StringLength(11, MinimumLength = 11)] string? Cpf,
    string? Nv,
    string? Motor,
    long? Renavam,
    int? Fipe);

public record MotoUpdateDto(
    [StringLength(11, MinimumLength = 11)] string? Cpf,
    string? Nv,
    string? Motor,
    long? Renavam,
    int? Fipe);

public record MotoResponseDto(
    string Placa,
    string? Cpf,
    string? Nv,
    string? Motor,
    long? Renavam,
    int? Fipe);