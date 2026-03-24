namespace Examen_Progra_Web.API.DTOs;

public class InscribirseDto
{
    public bool ConfirmoPago { get; set; }
}

public class ActualizarResultadoDto
{
    public bool Victoria { get; set; }
    public int PuntosPartida { get; set; }
    public int? Asesinatos { get; set; }
    public int? Muertes { get; set; }
    public int? Asistencias { get; set; }
    public double? DanoCausado { get; set; }
}

public class ParticipacionDto
{
    public string Id { get; set; } = string.Empty;
    public string JugadorId { get; set; } = string.Empty;
    public string JugadorNombre { get; set; } = string.Empty;
    public string JugadorNombreUsuario { get; set; } = string.Empty;
    public string TorneoId { get; set; } = string.Empty;
    public string? EquipoId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int? Posicion { get; set; }
    public int PuntosObtenidos { get; set; }
    public int PartidasJugadas { get; set; }
    public int Victorias { get; set; }
    public int Derrotas { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public DateTime? FechaEliminacion { get; set; }
    public EstadisticasPartidaDto Estadisticas { get; set; } = new();
    public int Penalizaciones { get; set; }
    public bool Pagado { get; set; }
}

public class EstadisticasPartidaDto
{
    public int Asesinatos { get; set; }
    public int Muertes { get; set; }
    public int Asistencias { get; set; }
    public double DanoCausado { get; set; }
}

public class ParticipantesPaginadosDto
{
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public List<ParticipacionDto> Participantes { get; set; } = new();
}

public class MisTorneosDto
{
    public string TorneoId { get; set; } = string.Empty;
    public string TorneoNombre { get; set; } = string.Empty;
    public string JuegoTitulo { get; set; } = string.Empty;
    public string EstadoTorneo { get; set; } = string.Empty;
    public string EstadoParticipacion { get; set; } = string.Empty;
    public int? Posicion { get; set; }
    public int PuntosObtenidos { get; set; }
    public int Victorias { get; set; }
    public int Derrotas { get; set; }
    public DateTime FechaInscripcion { get; set; }
}
