using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/jugadores")]
[Authorize]
public class JugadoresController : ControllerBase
{
    private readonly IJugadoresService _jugadoresService;
    private readonly ILogger<JugadoresController> _logger;

    public JugadoresController(IJugadoresService jugadoresService, ILogger<JugadoresController> logger)
    {
        _jugadoresService = jugadoresService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJugador(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "El ID del jugador es requerido" });
            }

            var jugador = await _jugadoresService.GetJugadorById(id);
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

    [HttpPut("{id}/perfil")]
    public async Task<IActionResult> ActualizarPerfil(string id, [FromBody] ActualizarPerfilDto perfilDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            if (id != userId && userRole != "admin")
            {
                return Forbid();
            }

            if (perfilDto == null)
            {
                return BadRequest(new { message = "El cuerpo de la petición es requerido" });
            }

            var jugadorActualizado = await _jugadoresService.ActualizarPerfil(id, perfilDto);
            if (jugadorActualizado == null)
            {
                return NotFound(new { message = "Jugador no encontrado" });
            }

            var jugadorDto = new JugadorDto
            {
                Id = jugadorActualizado.Id,
                Nombre = jugadorActualizado.Nombre,
                Apellido = jugadorActualizado.Apellido,
                Correo = jugadorActualizado.Correo,
                NombreUsuario = jugadorActualizado.NombreUsuario,
                Edad = jugadorActualizado.Edad,
                Pais = jugadorActualizado.Pais,
                Rol = jugadorActualizado.Rol,
                Activo = jugadorActualizado.Activo,
                PuntosGlobales = jugadorActualizado.PuntosGlobales,
                TorneosGanados = jugadorActualizado.TorneosGanados,
                FechaRegistro = jugadorActualizado.FechaRegistro.ToDateTime(),
                Conectado = jugadorActualizado.Conectado
            };

            return Ok(jugadorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar perfil: {ex.Message}");
            return StatusCode(500, new { message = "Error al actualizar perfil" });
        }
    }
}
