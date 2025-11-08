namespace OrganizadorMottu.Domain.Entity;

public class Moto
{
    // PK
    public string Placa { get; set; } = null!; // CD_PLACA

    // FK
    public string? Cpf { get; set; }           // CD_CPF -> T_MT_Usuario

    // Campos
    public string? Nv { get; set; }            // CD_NV
    public string? Motor { get; set; }         // CD_MOTOR
    public long? Renavam { get; set; }         // CD_RENAVAM
    public int? Fipe { get; set; }             // CD_FIPE

    // Navegação
    public Usuario? Usuario { get; set; }          // Navegação por CPF
    public Usuario? UsuarioByPlaca { get; set; }   // Relação 1:1 via CD_PLACA
}
