using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using System.Security.Claims;
using Examen_Progra_Web.API.Models;

[ApiController]
[Route("api/clasificaciones")]
public class ClasificacionesController : ControllerBase
{
    private readonly FirestoreDb _db;

    public ClasificacionesController(FirestoreDb db)
    {
        _db = db;
    }

    [HttpGet("{juegoId}")]
    public async Task<IActionResult> GetRanking(string juegoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        if (pageSize > 50) pageSize = 50;

        var snapshot = await _db.Collection("clasificaciones")
            .WhereEqualTo("JuegoId", juegoId)
            .GetSnapshotAsync();

        var ranking = snapshot.Documents
            .Select(d => d.ConvertTo<Clasificacion>())
            .OrderBy(c => c.Posicion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClasificacionDto
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
            }).ToList();

        var response = new RankingResponseDto
        {
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(snapshot.Count / (double)pageSize),
            TotalRegistros = snapshot.Count,
            Ranking = ranking
        };

        return Ok(response);
    }

    [HttpGet("jugador/clasificacion/{juegoId}")]
    public async Task<IActionResult> GetPosicionPersonal(string juegoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var snapshot = await _db.Collection("clasificaciones")
            .WhereEqualTo("JugadorId", userId)
            .GetSnapshotAsync();

        var doc = snapshot.Documents.FirstOrDefault(d => d.GetValue<string>("JuegoId") == juegoId);

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
