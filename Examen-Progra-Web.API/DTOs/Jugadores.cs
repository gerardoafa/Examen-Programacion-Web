namespace Examen_Progra_Web.API.DTOs;

public class JugadorDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Pais { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int PuntosGlobales { get; set; }
    public int TorneosGanados { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Conectado { get; set; }
}

public class ActualizarPerfilDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Pais { get; set; } = string.Empty;
}

public class JugadorDetalleDto : JugadorDto
{
    public DateTime? UltimaConexion { get; set; }
}
