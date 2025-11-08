namespace OrganizadorMottu.Domain.Entity;

public class Usuario
{
    public string Cpf { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public DateTime? DataNascimento { get; set; }

    public string? NrCep { get; set; }
    public string? CdPlaca { get; set; }

    public string SenhaHash { get; set; } = string.Empty;

    //Relacionamentos
    public int? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }

    public int? MotoId { get; set; }
    public Moto? Moto { get; set; }
}
