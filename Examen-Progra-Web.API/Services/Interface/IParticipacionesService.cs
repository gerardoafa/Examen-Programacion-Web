using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IParticipacionesService
{
    Task InscribirJugador(string torneoId, string jugadorId, bool haPagado);
    Task ActualizarResultado(string participacionId, bool victoria, int puntosPartida);
    Task<List<Participacion>> GetMisTorneos(string jugadorId);
}