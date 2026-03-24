using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IJuegosService
{
    Task<Juego> CrearJuego(Juego juego);
    Task<List<Juego>> GetJuegosDisponibles(string? genero, string? plataforma);
    Task<Juego?> GetEstadisticasJuego(string id);
    Task<bool> ActualizarJuego(string id, string? descripcion, double? puntuacion, string? estado);
}
