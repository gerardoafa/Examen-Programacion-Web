using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.DTOs;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IJugadoresService
{
    Task<Jugador?> GetJugadorPublico(string id);
    Task<bool> UpdatePerfil(string id, ActualizarPerfilDto perfilDto);
    Task<List<Jugador>> GetRankingGlobal();
}