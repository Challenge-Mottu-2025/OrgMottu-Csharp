namespace Mottu.Api.Models;

public class UsuarioReliabilityInput
{
    public float EntregasRealizadas { get; set; }
    public float MediaAvaliacoes { get; set; }
    public float Infracoes { get; set; }
}

public class UsuarioReliabilityOutput
{
    public float ScoreConfiabilidade { get; set; }
}