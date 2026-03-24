namespace Examen_Progra_Web.API.DTOs
{
    public class ClasificacionDto
    {
        public string JugadorId { get; set; } = string.Empty;
        public string JuegoId { get; set; } = string.Empty;
        public int Posicion { get; set; }
        public int PuntosJuego { get; set; }
        public int NivelJuego { get; set; }
        public double RatioVictoria { get; set; }   // 0-100
        public int RachaActual { get; set; }
        public int MedallasOro { get; set; }
        public int MedallasPlata { get; set; }
        public int MedallasBronce { get; set; }
        public List<string> Logros { get; set; } = new();
    }

    // Para el ranking paginado
    public class RankingResponseDto
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public List<ClasificacionDto> Ranking { get; set; } = new();
    }

    // Para mi desempeño personal
    public class MiDesempenoDto
    {
        public int Posicion { get; set; }
        public int Nivel { get; set; }
        public double RatioVictoria { get; set; }
        public int MedallasOro { get; set; }
        public int RachaActual { get; set; }
        public List<string> Logros { get; set; } = new();
    }
}
