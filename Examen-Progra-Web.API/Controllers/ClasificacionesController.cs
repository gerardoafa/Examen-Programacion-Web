using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using System.Security.Claims;
using Examen_Progra_Web.API.Models;

[ApiController]
[Route("api/clasificaciones")]
[Authorize]
public class ClasificacionesController : ControllerBase
{
    private readonly FirestoreDb _db;

    public ClasificacionesController(FirestoreDb db)
    {
        _db = db;
    }

    // ENDPOINT 1: Ranking global por juego (paginado)
    [HttpGet("{juegoId}")]
    public async Task<IActionResult> GetRanking(string juegoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        if (pageSize > 50) pageSize = 50;

        var query = _db.Collection("clasificaciones")
            .WhereEqualTo("JuegoId", juegoId)
            .OrderBy("Posicion")
            .Limit(pageSize)
            .Offset((page - 1) * pageSize);

        var snapshot = await query.GetSnapshotAsync();

        var ranking = snapshot.Documents.Select(d =>
        {
            var c = d.ConvertTo<Clasificacion>(); // tu modelo en Models
            return new ClasificacionDto
            {
                JugadorId = c.JugadorId,
                JuegoId = c.JuegoId,
                Posicion = c.Posicion,
                PuntosJuego = c.PuntosJuego,
                NivelJuego = c.NivelJuego,
                RatioVictoria = c.RatioVictoria,
                RachaActual = c.Racha,
                MedallasOro = c.MedallasOro,
                MedallasPlata = c.MedallasPlata,
                MedallasBronce = c.MedallasBronce,
                Logros = c.Logros
            };
        }).ToList();

        var response = new RankingResponseDto
        {
            PaginaActual = page,
            TotalPaginas = 10, // puedes calcular real si quieres
            TotalRegistros = ranking.Count,
            Ranking = ranking
        };

        return Ok(response);
    }

    // ENDPOINT 2: Posición personal del jugador autenticado
    [HttpGet("jugador/clasificacion/{juegoId}")]
    public async Task<IActionResult> GetPosicionPersonal(string juegoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = _db.Collection("clasificaciones")
            .WhereEqualTo("JugadorId", userId)
            .WhereEqualTo("JuegoId", juegoId);

        var snapshot = await query.GetSnapshotAsync();
        var doc = snapshot.Documents.FirstOrDefault();

        if (doc == null) return NotFound(new { mensaje = "No tienes clasificación en este juego" });

        var c = doc.ConvertTo<Clasificacion>();
        var dto = new MiDesempenoDto
        {
            Posicion = c.Posicion,
            Nivel = c.NivelJuego,
            RatioVictoria = c.RatioVictoria,
            MedallasOro = c.MedallasOro,
            RachaActual = c.Racha,
            Logros = c.Logros
        };

        return Ok(dto);
    }
}
