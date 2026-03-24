using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using System.Security.Claims;
using Examen_Progra_Web.API.Models;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly FirestoreDb _db;

    public ReportesController(FirestoreDb db)
    {
        _db = db;
    }

    // ENDPOINT 3: Top 10 torneos más populares (últimos 30 días)
    [HttpGet("torneos-populares")]
    [Authorize(Roles = "admin,organizador")]
    public async Task<IActionResult> TorneosPopulares()
    {
        var fechaLimite = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-30));

        var query = _db.Collection("torneos")
            .WhereGreaterThan("fechaCreacion", fechaLimite)
            .OrderByDescending("participantesActuales")
            .Limit(10);

        var snapshot = await query.GetSnapshotAsync();

        var lista = snapshot.Documents.Select(d => new TorneoPopularDto
        {
            Nombre = d.GetValue<string>("nombre"),
            Juego = d.GetValue<string>("juego"),
            Inscripciones = d.GetValue<int>("participantesActuales"),
            PremioTotal = d.GetValue<double>("premioTotal"),
            Estado = d.GetValue<string>("estado")
        }).ToList();

        return Ok(lista);
    }

    // ENDPOINT 4: Top 20 jugadores destacados
    [HttpGet("jugadores-destacados")]
    public async Task<IActionResult> JugadoresDestacados()
    {
        var query = _db.Collection("jugadores")
            .OrderByDescending("puntosGlobales")
            .Limit(20);

        var snapshot = await query.GetSnapshotAsync();

        var lista = snapshot.Documents.Select(d => new JugadorDestacadoDto
        {
            Nombre = d.GetValue<string>("nombre"),
            PuntosGlobales = d.GetValue<int>("puntosGlobales"),
            TorneosGanados = d.GetValue<int>("torneoGanados")
        }).ToList();

        return Ok(lista);
    }

    // ENDPOINT 5: Mi desempeño (solo jugador autenticado)
    [HttpGet("mi-desempeno/{juegoId}")]
    public async Task<IActionResult> MiDesempeno(string juegoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Reutilizamos la misma lógica del otro controller (o duplicamos query)
        var query = _db.Collection("clasificaciones")
            .WhereEqualTo("JugadorId", userId)
            .WhereEqualTo("JuegoId", juegoId);

        var snapshot = await query.GetSnapshotAsync();
        var doc = snapshot.Documents.FirstOrDefault();

        if (doc == null) return NotFound(new { mensaje = "No tienes datos en este juego" });

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

    // ENDPOINT 6: Tendencias (solo admin)
    [HttpGet("tendencias")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Tendencias()
    {
        var juegosPopulares = await _db.Collection("juegos")
            .OrderByDescending("torneoActivos")
            .Limit(5)
            .GetSnapshotAsync();

        var dto = new TendenciasDto
        {
            JuegosMasPopulares = juegosPopulares.Documents.Select(d => d.GetValue<string>("titulo")).ToList(),
            HoraPicoActividad = "20:00 - 23:00",
            TotalTorneosActivos = 42 // puedes contar real si quieres
        };

        return Ok(dto);
    }
}
