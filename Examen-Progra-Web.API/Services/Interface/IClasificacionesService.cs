using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IClasificacionesService
{
    Task<List<Clasificacion>> GetRankingByJuego(string juegoId);
    Task<Clasificacion?> GetMiClasificacion(string jugadorId, string juegoId);
}