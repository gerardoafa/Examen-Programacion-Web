using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services;

public class ReportesService
{
    private readonly FirestoreDb _db;

    public ReportesService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<List<TorneoPopularDto>> GetTorneosPopularesAsync()
    {
        var treintaDiasAtras = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-30));

        var query = _db.Collection("torneos")
            .WhereGreaterThanOrEqualTo("FechaCreacion", treintaDiasAtras)
            .OrderByDescending("ParticipantesActuales")
            .Limit(10);

        var snapshot = await query.GetSnapshotAsync();
        var torneos = new List<TorneoPopularDto>();

        foreach (var doc in snapshot.Documents)
        {
            var torneo = doc.ConvertTo<Torneo>();
            var juego = await GetJuegoAsync(torneo.Juego);
            torneos.Add(new TorneoPopularDto
            {
                Nombre = torneo.Nombre,
                Juego = juego?.Titulo ?? "Desconocido",
                Inscripciones = torneo.ParticipantesActuales,
                PremioTotal = torneo.PremioTotal,
                Estado = torneo.Estado
            });
        }

        return torneos;
    }

    public async Task<List<JugadorDestacadoDto>> GetJugadoresDestacadosAsync()
    {
        var query = _db.Collection("jugadores")
            .OrderByDescending("PuntosGlobales")
            .Limit(20);

        var snapshot = await query.GetSnapshotAsync();

        return snapshot.Documents.Select(doc =>
        {
            var jugador = doc.ConvertTo<Jugador>();
            return new JugadorDestacadoDto
            {
                Nombre = $"{jugador.Nombre} {jugador.Apellido}",
                PuntosGlobales = jugador.PuntosGlobales,
                TorneosGanados = jugador.TorneosGanados
            };
        }).ToList();
    }

    public async Task<MiDesempenoDto?> GetMiDesempenoAsync(string jugadorId, string juegoId)
    {
        var clasQuery = _db.Collection("clasificaciones")
            .WhereEqualTo("JugadorId", jugadorId)
            .WhereEqualTo("JuegoId", juegoId);

        var clasSnapshot = await clasQuery.GetSnapshotAsync();
        var clasDoc = clasSnapshot.Documents.FirstOrDefault();

        if (clasDoc == null) return null;

        var clasificacion = clasDoc.ConvertTo<Clasificacion>();

        var partiQuery = _db.Collection("participaciones")
            .WhereEqualTo("JugadorId", jugadorId)
            .OrderByDescending("FechaInscripcion")
            .Limit(3);

        var partiSnapshot = await partiQuery.GetSnapshotAsync();

        return new MiDesempenoDto
        {
            Posicion = clasificacion.Posicion,
            Nivel = clasificacion.NivelJuego,
            RatioVictoria = clasificacion.RatioVictoria,
            RachaActual = clasificacion.Racha,
            MedallasOro = clasificacion.MedallasOro,
            Logros = clasificacion.Logros
        };
    }

    public async Task<TendenciasDto> GetTendenciasAsync()
    {
        var juegoQuery = _db.Collection("juegos")
            .OrderByDescending("JugadoresActivos")
            .Limit(5);

        var juegoSnapshot = await juegoQuery.GetSnapshotAsync();
        var juegosPopulares = juegoSnapshot.Documents
            .Select(d => d.ConvertTo<Juego>())
            .Select(j => j.Titulo)
            .ToList();

        var torneoQuery = _db.Collection("torneos")
            .WhereEqualTo("Estado", "en progreso");

        var torneoSnapshot = await torneoQuery.GetSnapshotAsync();

        return new TendenciasDto
        {
            JuegosMasPopulares = juegosPopulares,
            TotalTorneosActivos = torneoSnapshot.Count,
            HoraPicoActividad = "20:00 - 23:00"
        };
    }

    private async Task<Juego?> GetJuegoAsync(string juegoId)
    {
        var doc = await _db.Collection("juegos").Document(juegoId).GetSnapshotAsync();
        return doc.Exists ? doc.ConvertTo<Juego>() : null;
    }
}
