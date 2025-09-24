namespace Mottu.Api.Models;

public class Moto
{
    // PK
    public string Placa { get; set; } = default!; // CD_PLACA

    // FKs
    public string? Cpf { get; set; } // CD_CPF -> T_MT_Usuario

    // Campos
    public string? Nv { get; set; }      // CD_NV
    public string? Motor { get; set; }   // CD_MOTOR
    public long? Renavam { get; set; }   // CD_RENAVAM
    public int? Fipe { get; set; }       // CD_FIPE

    // Navegações
    public Usuario? Usuario { get; set; }
    public Usuario? UsuarioByPlaca { get; set; } // relação 1:1 via CD_PLACA
}