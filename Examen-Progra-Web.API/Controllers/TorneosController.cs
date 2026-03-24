using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/torneos")]
public class TorneosController : ControllerBase
{
    private readonly ITorneosService _torneosService;

    public TorneosController(ITorneosService torneosService)
    {
        _torneosService = torneosService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearTorneo([FromBody] CrearTorneoDto dto)
    {
        try
        {
            if (dto.FechaInicio <= DateTime.UtcNow)
                return BadRequest(new { message = "La fecha de inicio debe ser posterior a hoy" });

            if (dto.FechaLimiteInscripcion >= dto.FechaInicio)
                return BadRequest(new { message = "La fecha límite debe ser antes de la fecha de inicio" });

            if (dto.MaxParticipantes <= 2)
                return BadRequest(new { message = "El máximo de participantes debe ser mayor a 2" });

            var formatosValidos = new[] { "individual", "equipos", "royale" };
            if (!formatosValidos.Contains(dto.Formato.ToLower()))
                return BadRequest(new { message = "El formato debe ser 'individual', 'equipos' o 'royale'" });

            var torneo = await _torneosService.CrearTorneo(dto, "anonimo");
            return Created($"/api/torneos/{torneo.Id}", new { message = "Torneo creado exitosamente", id = torneo.Id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear torneo" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarTorneos()
    {
        try
        {
            var torneos = await _torneosService.ListarTorneos(new TorneosFiltradosDto());
            return Ok(torneos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al listar torneos" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarTorneo(string id)
    {
        try
        {
            var resultado = await _torneosService.EliminarTorneo(id, "anonimo", "admin");
            if (!resultado)
                return NotFound(new { message = "Torneo no encontrado" });

            return Ok(new { message = "Torneo cancelado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar torneo" });
        }
    }
}
