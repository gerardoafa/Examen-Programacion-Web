namespace Examen_Progra_Web.API.DTOs
{
    public class TorneoPopularDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Juego { get; set; } = string.Empty;
        public int Inscripciones { get; set; }
        public double PremioTotal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class JugadorDestacadoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int PuntosGlobales { get; set; }
        public int TorneosGanados { get; set; }
    }

    public class TendenciasDto
    {
        public List<string> JuegosMasPopulares { get; set; } = new();
        public string HoraPicoActividad { get; set; } = "20:00 - 23:00";
        public int TotalTorneosActivos { get; set; }
    }
}
