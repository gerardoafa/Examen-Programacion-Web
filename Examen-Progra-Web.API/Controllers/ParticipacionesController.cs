using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
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
            var torneo = await _torneosService.ObtenerTorneoPorId(id);
            if (torneo == null)
                return NotFound(new { mensaje = "Torneo no encontrado" });

            if (torneo.Estado != "próximo")
                return BadRequest(new { mensaje = "El torneo no acepta inscripciones" });

            var participacionId = await _participacionesService.InscribirJugador(id, "anonimo", dto.ConfirmoPago);
            return Ok(new { mensaje = "Inscripción exitosa", participacionId });
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

    [HttpDelete("api/torneos/{id}/participantes/{participacionId}")]
    public async Task<IActionResult> AbandonarTorneo(string id, string participacionId)
    {
        try
        {
            var resultado = await _participacionesService.AbandonarTorneo(participacionId, "anonimo");
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
