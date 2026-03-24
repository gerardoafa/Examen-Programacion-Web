using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using System.Security.Claims;

namespace Examen_Progra_Web.API.Services.Interface
{
    public class ReportesService
    {
        private readonly FirestoreDb _db;

        public ReportesService(FirestoreDb db)
        {
            _db = db;
        }

        // ENDPOINT 3: Top 10 torneos más populares (últimos 30 días) - Escenario 5
        public async Task<List<TorneoPopularDto>> GetTorneosPopularesAsync()
        {
            var fechaLimite = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-30));

            var query = _db.Collection("torneos")
                .WhereGreaterThan("fechaCreacion", fechaLimite)
                .OrderByDescending("participantesActuales")
                .Limit(10);

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => new TorneoPopularDto
            {
                Nombre = d.GetValue<string>("nombre"),
                Juego = d.GetValue<string>("juego"),
                Inscripciones = d.GetValue<int>("participantesActuales"),
                PremioTotal = d.GetValue<double>("premioTotal"),
                Estado = d.GetValue<string>("estado")
            }).ToList();
        }

        // ENDPOINT 4: Top 20 jugadores destacados globales
        public async Task<List<JugadorDestacadoDto>> GetJugadoresDestacadosAsync()
        {
            var query = _db.Collection("jugadores")
                .OrderByDescending("puntosGlobales")
                .Limit(20);

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => new JugadorDestacadoDto
            {
                Nombre = d.GetValue<string>("nombre"),
                PuntosGlobales = d.GetValue<int>("puntosGlobales"),
                TorneosGanados = d.GetValue<int>("torneoGanados")
            }).ToList();
        }

        // ENDPOINT 5: Mi desempeño personal en un juego
        public async Task<MiDesempenoDto?> GetMiDesempenoAsync(string juegoId, string userId)
        {
            var query = _db.Collection("clasificaciones")
                .WhereEqualTo("JugadorId", userId)
                .WhereEqualTo("JuegoId", juegoId);

            var snapshot = await query.GetSnapshotAsync();
            var doc = snapshot.Documents.FirstOrDefault();

            if (doc == null) return null;

            var c = doc.ConvertTo<Clasificacion>();
            return new MiDesempenoDto
            {
                Posicion = c.Posicion,
                Nivel = c.NivelJuego,
                RatioVictoria = c.RatioVictoria,
                MedallasOro = c.MedallasOro,
                RachaActual = c.Racha,
                Logros = c.Logros
            };
        }

        // ENDPOINT 6: Tendencias (solo admin)
        public async Task<TendenciasDto> GetTendenciasAsync()
        {
            var juegosPopulares = await _db.Collection("juegos")
                .OrderByDescending("torneoActivos")
                .Limit(5)
                .GetSnapshotAsync();

            return new TendenciasDto
            {
                JuegosMasPopulares = juegosPopulares.Documents.Select(d => d.GetValue<string>("titulo")).ToList(),
                HoraPicoActividad = "20:00 - 23:00",
                TotalTorneosActivos = 42
            };
        }
    }
}
