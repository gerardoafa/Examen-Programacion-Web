using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/juegos")]
public class JuegosController : ControllerBase
{
    private readonly IJuegosService _juegosService;

    public JuegosController(IJuegosService juegosService)
    {
        _juegosService = juegosService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearJuego([FromBody] Juego juego)
    {
        try
        {
            var plataformasValidas = new[] { "PC", "PS5", "Xbox", "Switch" };
            if (juego.Plataformas.Any(p => !plataformasValidas.Contains(p)))
                return BadRequest(new { mensaje = "Plataformas inválidas. Use: PC, PS5, Xbox, Switch" });

            if (string.IsNullOrEmpty(juego.Desarrollador))
                return BadRequest(new { mensaje = "El desarrollador es requerido" });

            var resultado = await _juegosService.CrearJuego(juego);
            return Created($"/api/juegos/{resultado.Id}", resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al crear juego" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetJuegos(
        [FromQuery] string? genero = null,
        [FromQuery] string? plataforma = null,
        [FromQuery] string? desarrollador = null)
    {
        try
        {
            var juegos = await _juegosService.GetJuegosDisponibles(genero, plataforma);
            
            if (!string.IsNullOrEmpty(desarrollador))
                juegos = juegos.Where(j => j.Desarrollador.Contains(desarrollador, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(juegos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener juegos" });
        }
    }

    [HttpGet("{id}/estadisticas")]
    public async Task<IActionResult> GetEstadisticas(string id)
    {
        var juego = await _juegosService.GetEstadisticasJuego(id);
        if (juego == null)
            return NotFound(new { mensaje = "Juego no encontrado" });

        return Ok(new
        {
            id = juego.Id,
            titulo = juego.Titulo,
            jugadoresActivos = juego.JugadoresActivos,
            torneoActivos = juego.TorneoActivos,
            puntuacionPromedio = juego.PuntuacionPromedio
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarJuego(string id, [FromBody] ActualizarJuegoDto dto)
    {
        try
        {
            var estadosValidos = new[] { "disponible", "mantenimiento", "descontinuado" };
            if (!string.IsNullOrEmpty(dto.Estado) && !estadosValidos.Contains(dto.Estado))
                return BadRequest(new { mensaje = "Estado inválido" });

            var resultado = await _juegosService.ActualizarJuego(id, dto.Descripcion, dto.PuntuacionPromedio, dto.Estado);
            if (!resultado)
                return NotFound(new { mensaje = "Juego no encontrado" });

            return Ok(new { mensaje = "Juego actualizado" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar juego" });
        }
    }
}
