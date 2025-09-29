namespace Mottu.Api.Models;

public class Usuario
{
    public string Cpf { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public DateTime? DataNascimento { get; set; }

    public string? NrCep { get; set; }
    public string? CdPlaca { get; set; }
    public Endereco? Endereco { get; set; }
    public Moto? Moto { get; set; }
    
    public string SenhaHash { get; set; } = string.Empty;
}