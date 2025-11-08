namespace Mottu.Api.Domain.Entity;

public class Funcionario
{
    // PK
    public long IdFuncionario { get; set; }

    // Campos
    public string? NrCep { get; set; }   // FK -> Endereco
    public string? Senha { get; set; }
    public string? Cpf { get; set; }
    public string? Nome { get; set; }
}
