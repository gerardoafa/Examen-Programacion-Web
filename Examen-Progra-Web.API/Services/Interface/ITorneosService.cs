using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface ITorneosService
{
    Task<Torneo> CrearTorneo(CrearTorneoDto dto, string organizadorId);
    Task<List<TorneoDto>> ListarTorneos(TorneosFiltradosDto filtros);
    Task<Torneo?> ObtenerTorneoPorId(string torneoId);
    Task<Torneo?> ActualizarTorneo(string torneoId, ActualizarTorneoDto dto, string usuarioId, string rol);
    Task<bool> EliminarTorneo(string torneoId, string usuarioId, string rol);
    Task<Torneo?> CambiarEstado(string torneoId, string nuevoEstado, string usuarioId, string rol);
}
