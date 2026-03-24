using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Examen_Progra_Web.API.Controllers;

[ApiController]
[Route("api/jugadores")]
public class JugadoresController : ControllerBase
{
    private readonly IJugadoresService _jugadoresService;

    public JugadoresController(IJugadoresService jugadoresService)
    {
        _jugadoresService = jugadoresService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerJugador(string id)
    {
        var jugador = await _jugadoresService.GetJugadorPublico(id);
        if (jugador == null)
            return NotFound(new { mensaje = "Jugador no encontrado" });

        return Ok(new
        {
            id = jugador.Id,
            nombre = jugador.Nombre,
            apellido = jugador.Apellido,
            nombreUsuario = jugador.NombreUsuario,
            correo = jugador.Correo,
            edad = jugador.Edad,
            pais = jugador.Pais,
            rol = jugador.Rol,
            activo = jugador.Activo,
            puntosGlobales = jugador.PuntosGlobales,
            torneoGanados = jugador.TorneosGanados,
            conectado = jugador.Conectado,
            fechaRegistro = jugador.FechaRegistro.ToDateTime()
        });
    }

    [HttpPut("{id}/perfil")]
    public async Task<IActionResult> ActualizarPerfil(string id, [FromBody] ActualizarPerfilDto dto)
    {
        var resultado = await _jugadoresService.UpdatePerfil(id, dto);
        if (!resultado)
            return NotFound(new { mensaje = "Jugador no encontrado" });

        return Ok(new { mensaje = "Perfil actualizado exitosamente" });
    }

    [HttpGet("ranking")]
    public async Task<IActionResult> GetRanking()
    {
        var ranking = await _jugadoresService.GetRankingGlobal();
        return Ok(ranking);
    }
}
