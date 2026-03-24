using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Authorize]
public class ParticipacionesController : ControllerBase
{
    private readonly IParticipacionesService _participacionesService;
    private readonly ITorneosService _torneosService;

    public ParticipacionesController(IParticipacionesService participacionesService, ITorneosService torneosService)
    {
        _participacionesService = participacionesService;
        _torneosService = torneosService;
    }

    [HttpPost("api/torneos/{id}/inscribirse")]
    public async Task<IActionResult> Inscribirse(string id, [FromBody] InscribirseDto dto)
    {
        try
        {
            var jugadorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(jugadorId))
                return Unauthorized(new { mensaje = "Usuario no autenticado" });

            var torneo = await _torneosService.ObtenerTorneoPorId(id);
            if (torneo == null)
                return NotFound(new { mensaje = "Torneo no encontrado" });

            if (torneo.Estado != "próximo")
                return BadRequest(new { mensaje = "El torneo no acepta inscripciones" });

            await _participacionesService.InscribirJugador(id, jugadorId, dto.ConfirmoPago);
            return Ok(new { mensaje = "Inscripción exitosa" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al inscribirse" });
        }
    }

    [HttpGet("api/torneos/{id}/participantes")]
    public async Task<IActionResult> GetParticipantes(string id)
    {
        try
        {
            var participantes = await _participacionesService.GetParticipantesTorneo(id);
            return Ok(participantes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener participantes" });
        }
    }

    [HttpPut("api/torneos/{id}/participantes/{participacionId}/actualizar-resultado")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> ActualizarResultado(string id, string participacionId, [FromBody] ActualizarResultadoDto dto)
    {
        try
        {
            var resultado = await _participacionesService.ActualizarResultado(participacionId, dto.Victoria, dto.PuntosPartida);
            if (!resultado)
                return NotFound(new { mensaje = "Participación no encontrada" });

            return Ok(new { mensaje = "Resultado actualizado" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar resultado" });
        }
    }

    [HttpGet("api/jugador/mis-torneos")]
    public async Task<IActionResult> GetMisTorneos()
    {
        try
        {
            var jugadorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(jugadorId))
                return Unauthorized(new { mensaje = "Usuario no autenticado" });

            var torneos = await _participacionesService.GetMisTorneos(jugadorId);
            return Ok(torneos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener torneos" });
        }
    }

    [HttpDelete("api/torneos/{id}/participantes/{participacionId}/abandonar")]
    public async Task<IActionResult> AbandonarTorneo(string id, string participacionId)
    {
        try
        {
            var jugadorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(jugadorId))
                return Unauthorized(new { mensaje = "Usuario no autenticado" });

            var resultado = await _participacionesService.AbandonarTorneo(participacionId, jugadorId);
            if (!resultado)
                return BadRequest(new { mensaje = "No se puede abandonar este torneo" });

            return Ok(new { mensaje = "Has abandonado el torneo" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al abandonar torneo" });
        }
    }
}
