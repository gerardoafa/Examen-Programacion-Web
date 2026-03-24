using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/torneos")]
[Authorize]
public class TorneosController : ControllerBase
{
    private readonly ITorneosService _torneosService;
    private readonly ILogger<TorneosController> _logger;

    public TorneosController(ITorneosService torneosService, ILogger<TorneosController> logger)
    {
        _torneosService = torneosService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> CrearTorneo([FromBody] CrearTorneoDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            if (dto.FechaInicio <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "La fecha de inicio debe ser posterior a hoy" });
            }

            if (dto.FechaLimiteInscripcion >= dto.FechaInicio)
            {
                return BadRequest(new { message = "La fecha límite de inscripción debe ser antes de la fecha de inicio" });
            }

            if (dto.MaxParticipantes <= 2)
            {
                return BadRequest(new { message = "El máximo de participantes debe ser mayor a 2" });
            }

            var formatosValidos = new[] { "individual", "equipos", "royale" };
            if (!formatosValidos.Contains(dto.Formato.ToLower()))
            {
                return BadRequest(new { message = "El formato debe ser 'individual', 'equipos' o 'royale'" });
            }

            var torneo = await _torneosService.CrearTorneo(dto, userId);
            _logger.LogInformation($"Torneo creado: {torneo.Nombre} por usuario {userId}");

            return Created($"/api/torneos/{torneo.Id}", new { message = "Torneo creado exitosamente", id = torneo.Id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear torneo: {ex.Message}");
            return StatusCode(500, new { message = "Error al crear torneo" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarTorneos(
        [FromQuery] string? juegoId = null,
        [FromQuery] string? estado = null,
        [FromQuery] double? precioMin = null,
        [FromQuery] double? precioMax = null,
        [FromQuery] int nivelMin = 0,
        [FromQuery] int nivelMax = 100)
    {
        try
        {
            var filtros = new TorneosFiltradosDto
            {
                JuegoId = juegoId,
                Estado = estado,
                PrecioMin = precioMin,
                PrecioMax = precioMax,
                NivelMin = nivelMin,
                NivelMax = nivelMax
            };

            var torneos = await _torneosService.ListarTorneos(filtros);
            return Ok(torneos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al listar torneos: {ex.Message}");
            return StatusCode(500, new { message = "Error al listar torneos" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerTorneo(string id)
    {
        try
        {
            var torneo = await _torneosService.ObtenerTorneoPorId(id);
            if (torneo == null)
            {
                return NotFound(new { message = "Torneo no encontrado" });
            }

            return Ok(torneo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener torneo: {ex.Message}");
            return StatusCode(500, new { message = "Error al obtener torneo" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> ActualizarTorneo(string id, [FromBody] ActualizarTorneoDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var torneo = await _torneosService.ActualizarTorneo(id, dto, userId, rol ?? "");
            if (torneo == null)
            {
                return NotFound(new { message = "Torneo no encontrado" });
            }

            return Ok(torneo);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar torneo: {ex.Message}");
            return StatusCode(500, new { message = "Error al actualizar torneo" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> EliminarTorneo(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var resultado = await _torneosService.EliminarTorneo(id, userId, rol ?? "");
            if (!resultado)
            {
                return NotFound(new { message = "Torneo no encontrado" });
            }

            return Ok(new { message = "Torneo cancelado exitosamente" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar torneo: {ex.Message}");
            return StatusCode(500, new { message = "Error al eliminar torneo" });
        }
    }

    [HttpPatch("{id}/cambiar-estado")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> CambiarEstado(string id, [FromBody] CambiarEstadoTorneoDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var torneo = await _torneosService.CambiarEstado(id, dto.NuevoEstado, userId, rol ?? "");
            if (torneo == null)
            {
                return NotFound(new { message = "Torneo no encontrado" });
            }

            return Ok(torneo);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cambiar estado: {ex.Message}");
            return StatusCode(500, new { message = "Error al cambiar estado" });
        }
    }
}
