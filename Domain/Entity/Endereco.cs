namespace Mottu.Api.Domain.Entity;

public class Endereco
{
    // PK
    public string NrCep { get; set; } = default!;

    // Campos
    public string? IdPais { get; set; }
    public string? SiglaEstado { get; set; }
    public string? IdCidade { get; set; }
    public string? IdBairro { get; set; }
    public string? NrNumero { get; set; }
    public string? Logradouro { get; set; }
    public string? Complemento { get; set; }

    // Relacionamento com Usuario
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();


}
