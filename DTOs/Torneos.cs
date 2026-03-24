namespace Examen_Progra_Web.API.DTOs;

public class CrearTorneoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string JuegoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Formato { get; set; } = "individual";
    public int MaxParticipantes { get; set; }
    public double PrecioInscripcion { get; set; }
    public double PremioTotal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaLimiteInscripcion { get; set; }
    public int MinNivel { get; set; }
    public int MaxNivel { get; set; }
    public bool RequiereEquipo { get; set; }
    public int TamanioEquipo { get; set; }
}

public class ActualizarTorneoDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? MaxParticipantes { get; set; }
    public double? PrecioInscripcion { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaLimiteInscripcion { get; set; }
    public int? MinNivel { get; set; }
    public int? MaxNivel { get; set; }
}

public class CambiarEstadoTorneoDto
{
    public string NuevoEstado { get; set; } = string.Empty;
}

public class TorneoDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string JuegoId { get; set; } = string.Empty;
    public string JuegoTitulo { get; set; } = string.Empty;
    public string OrganizadorId { get; set; } = string.Empty;
    public string OrganizadorNombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
    public int MaxParticipantes { get; set; }
    public int ParticipantesActuales { get; set; }
    public double PrecioInscripcion { get; set; }
    public double PremioTotal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaLimiteInscripcion { get; set; }
    public int MinNivel { get; set; }
    public int MaxNivel { get; set; }
    public bool RequiereEquipo { get; set; }
    public int TamanioEquipo { get; set; }
}

public class TorneosFiltradosDto
{
    public string? JuegoId { get; set; }
    public string? Estado { get; set; }
    public double? PrecioMin { get; set; }
    public double? PrecioMax { get; set; }
    public int? NivelMin { get; set; }
    public int? NivelMax { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}

public class TorneosPaginadosDto
{
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public List<TorneoDto> Torneos { get; set; } = new();
}
