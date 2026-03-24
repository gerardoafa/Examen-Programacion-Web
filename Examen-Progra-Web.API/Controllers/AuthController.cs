using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (registerDto == null)
            {
                return BadRequest(new { message = "El cuerpo de la petición es requerido" });
            }

            if (string.IsNullOrWhiteSpace(registerDto.Correo) ||
                string.IsNullOrWhiteSpace(registerDto.Contrasena))
            {
                return BadRequest(new { message = "Correo y contraseña son requeridos" });
            }

            if (registerDto.Contrasena.Length < 6)
            {
                return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });
            }

            var jugador = await _authService.Register(registerDto);
            _logger.LogInformation($"Jugador registrado: {jugador.Correo} ({jugador.Rol})");

            var jugadorDto = new JugadorDto
            {
                Id = jugador.Id,
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                Correo = jugador.Correo,
                NombreUsuario = jugador.NombreUsuario,
                Edad = jugador.Edad,
                Pais = jugador.Pais,
                Rol = jugador.Rol,
                Activo = jugador.Activo,
                PuntosGlobales = jugador.PuntosGlobales,
                TorneosGanados = jugador.TorneosGanados,
                FechaRegistro = jugador.FechaRegistro.ToDateTime(),
                Conectado = jugador.Conectado
            };

            return Created($"/api/jugadores/{jugador.Id}", jugadorDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en registro: {ex.Message}");
            return StatusCode(500, new { message = "Error al registrar jugador" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (loginDto == null)
            {
                return BadRequest(new { message = "El cuerpo de la petición es requerido" });
            }

            if (string.IsNullOrWhiteSpace(loginDto.Correo) ||
                string.IsNullOrWhiteSpace(loginDto.Contrasena))
            {
                return BadRequest(new { message = "Correo y contraseña son requeridos" });
            }

            var (jugador, token) = await _authService.Login(loginDto);
            _logger.LogInformation($"Jugador inició sesión: {jugador.Correo} ({jugador.Rol})");

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Inicio de sesión exitoso",
                Token = token,
                Jugador = new JugadorDto
                {
                    Id = jugador.Id,
                    Nombre = jugador.Nombre,
                    Apellido = jugador.Apellido,
                    Correo = jugador.Correo,
                    NombreUsuario = jugador.NombreUsuario,
                    Edad = jugador.Edad,
                    Pais = jugador.Pais,
                    Rol = jugador.Rol,
                    Activo = jugador.Activo,
                    PuntosGlobales = jugador.PuntosGlobales,
                    TorneosGanados = jugador.TorneosGanados,
                    FechaRegistro = jugador.FechaRegistro.ToDateTime(),
                    Conectado = true
                }
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en login: {ex.Message}");
            return StatusCode(500, new
            {
                success = false,
                message = "Error al iniciar sesión"
            });
        }
    }

    [HttpGet("jugadores/{jugadorId}")]
    public async Task<IActionResult> GetJugador(string jugadorId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jugadorId))
            {
                return BadRequest(new { message = "El ID del jugador es requerido" });
            }

            var jugador = await _authService.GetJugadorById(jugadorId);
            if (jugador == null)
            {
                return NotFound(new { message = "Jugador no encontrado" });
            }

            var jugadorDto = new JugadorDto
            {
                Id = jugador.Id,
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                Correo = jugador.Correo,
                NombreUsuario = jugador.NombreUsuario,
                Edad = jugador.Edad,
                Pais = jugador.Pais,
                Rol = jugador.Rol,
                Activo = jugador.Activo,
                PuntosGlobales = jugador.PuntosGlobales,
                TorneosGanados = jugador.TorneosGanados,
                FechaRegistro = jugador.FechaRegistro.ToDateTime(),
                Conectado = jugador.Conectado
            };

            return Ok(jugadorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener jugador: {ex.Message}");
            return StatusCode(500, new { message = "Error al obtener jugador" });
        }
    }
}
