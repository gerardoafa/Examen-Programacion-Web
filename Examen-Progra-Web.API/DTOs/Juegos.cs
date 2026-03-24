namespace Examen_Progra_Web.API.DTOs;

public class CrearJuegoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Desarrollador { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public List<string> Plataformas { get; set; } = new();
    public DateTime FechaLanzamiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class ActualizarJuegoDto
{
    public string Descripcion { get; set; } = string.Empty;
    public double PuntuacionPromedio { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class JuegoDto
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Desarrollador { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public List<string> Plataformas { get; set; } = new();
    public DateTime FechaLanzamiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int JugadoresActivos { get; set; }
    public int TorneoActivos { get; set; }
    public string Estado { get; set; } = string.Empty;
    public double PuntuacionPromedio { get; set; }
}

public class JuegoEstadisticasDto
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int JugadoresActivos { get; set; }
    public int TorneoActivos { get; set; }
    public double PuntuacionPromedio { get; set; }
}

public class JuegosFiltradosDto
{
    public string? Genero { get; set; }
    public string? Plataforma { get; set; }
    public string? Desarrollador { get; set; }
}
