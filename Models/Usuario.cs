namespace Mottu.Api.Models;

public class Usuario
{
    // PK
    public string Cpf { get; set; } = default!; // CD_CPF

    // FKs
    public string? NrCep { get; set; }         // NR_CEP -> T_MT_Endereco
    public string? CdPlaca { get; set; }       // CD_PLACA -> T_MT_Moto

    // Campos
    public string Nome { get; set; } = default!;
    public DateTime? DataNascimento { get; set; }

    // Navegações
    public Endereco? Endereco { get; set; }
    public Moto? Moto { get; set; }
}