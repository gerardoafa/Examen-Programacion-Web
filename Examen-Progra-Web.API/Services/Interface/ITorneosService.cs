using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface ITorneosService
{
    Task<Torneo> CrearTorneo(Torneo torneo);
    Task CambiarEstado(string torneoId, string nuevoEstado);
    Task<List<Torneo>> ListarTorneos(string? juego, string? estado);
    Task<bool> CancelarTorneo(string id);
}