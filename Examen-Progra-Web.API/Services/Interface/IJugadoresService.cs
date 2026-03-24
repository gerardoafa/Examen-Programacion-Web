using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IJugadoresService
{
    Task<Jugador?> GetJugadorById(string jugadorId);
    Task<Jugador?> ActualizarPerfil(string jugadorId, ActualizarPerfilDto perfilDto);
}
