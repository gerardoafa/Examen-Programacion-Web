public class ClasificacionDto
{
    public string JugadorId { get; set; } = string.Empty;
    public string JuegoId { get; set; } = string.Empty;
    public int Posicion { get; set; }
    public int PuntosJuego { get; set; }
    public int NivelJuego { get; set; }
    public double RatioVictoria { get; set; }
    public int RachaActual { get; set; }
    public int MedallasOro { get; set; }
    public List<string> Logros { get; set; } = new();
}
