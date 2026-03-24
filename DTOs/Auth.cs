using Examen_Progra_Web.API.DTOs;

namespace Examen_Progra_Web.API.DTOs;

public class RegisterDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Pais { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public JugadorDto? Jugador { get; set; }
}

public class JugadorDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int PuntosGlobales { get; set; }
    public int TorneosGanados { get; set; }
}
}
