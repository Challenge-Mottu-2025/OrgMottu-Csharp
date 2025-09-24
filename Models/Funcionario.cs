namespace Mottu.Api.Models;

public class Funcionario
{
    public long IdFuncionario { get; set; } // PK
    public string? NrCep { get; set; }      // FK -> Endereco
    public string? Senha { get; set; }
    public string? Cpf { get; set; }
    public string? Nome { get; set; }
}