namespace Mottu.Api.Models;

public class Endereco
{
    // PK
    public string NrCep { get; set; } = default!;

    public string? IdPais { get; set; }
    public string? SiglaEstado { get; set; }
    public string? IdCidade { get; set; }
    public string? IdBairro { get; set; }
    public string? NrNumero { get; set; }
    public string? Logradouro { get; set; }
    public string? Complemento { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}