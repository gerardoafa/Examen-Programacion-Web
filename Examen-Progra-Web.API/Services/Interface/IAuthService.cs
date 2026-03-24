using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services.Interface;

public interface IAuthService
{
    Task<Jugador> Register(RegisterDto registerDto);
    Task<(Jugador jugador, string token)> Login(LoginDto loginDto);
    Task<bool> ValidateToken(string token);
    Task<Jugador?> GetJugadorById(string jugadorId);
    string GenerateJwtToken(Jugador jugador);
}