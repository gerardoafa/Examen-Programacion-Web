using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IParticipacionesService
{
    Task<string> InscribirJugador(string torneoId, string jugadorId, bool haPagado);
    Task<List<Participacion>> GetParticipantesTorneo(string torneoId);
    Task<bool> ActualizarResultado(string participacionId, bool victoria, int puntosPartida);
    Task<List<Participacion>> GetMisTorneos(string jugadorId);
    Task<bool> AbandonarTorneo(string participacionId, string jugadorId);
}
