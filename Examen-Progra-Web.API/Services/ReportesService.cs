using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services
{
    public class ReportesService
    {
        private readonly FirestoreDb _db;

        public ReportesService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<List<TorneoPopularDto>> GetTorneosPopularesAsync()
        {
            var snapshot = await _db.Collection("torneos")
                .Limit(50)
                .GetSnapshotAsync();

            var torneos = snapshot.Documents
                .Select(d => new
                {
                    Doc = d,
                    Nombre = d.GetValue<string>("Nombre"),
                    Juego = d.GetValue<string>("Juego"),
                    ParticipantesActuales = d.GetValue<int>("ParticipantesActuales"),
                    PremioTotal = d.GetValue<double>("PremioTotal"),
                    Estado = d.GetValue<string>("Estado"),
                    FechaCreacion = d.GetValue<Timestamp>("FechaCreacion")
                })
                .Where(t => t.FechaCreacion != null && t.FechaCreacion.ToDateTime() >= DateTime.UtcNow.AddDays(-30))
                .OrderByDescending(t => t.ParticipantesActuales)
                .Take(10)
                .Select(t => new TorneoPopularDto
                {
                    Nombre = t.Nombre,
                    Juego = t.Juego,
                    Inscripciones = t.ParticipantesActuales,
                    PremioTotal = t.PremioTotal,
                    Estado = t.Estado
                }).ToList();

            return torneos;
        }

        public async Task<List<JugadorDestacadoDto>> GetJugadoresDestacadosAsync()
        {
            var snapshot = await _db.Collection("jugadores")
                .Limit(100)
                .GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => new JugadorDestacadoDto
                {
                    Nombre = d.GetValue<string>("Nombre") + " " + d.GetValue<string>("Apellido"),
                    PuntosGlobales = d.GetValue<int>("PuntosGlobales"),
                    TorneosGanados = d.GetValue<int>("TorneosGanados")
                })
                .OrderByDescending(j => j.PuntosGlobales)
                .Take(20)
                .ToList();
        }

        public async Task<MiDesempenoDto?> GetMiDesempenoAsync(string juegoId, string userId)
        {
            var snapshot = await _db.Collection("clasificaciones")
                .WhereEqualTo("JugadorId", userId)
                .GetSnapshotAsync();

            var doc = snapshot.Documents.FirstOrDefault(d => d.GetValue<string>("JuegoId") == juegoId);
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

        public async Task<TendenciasDto> GetTendenciasAsync()
        {
            var snapshot = await _db.Collection("juegos")
                .Limit(50)
                .GetSnapshotAsync();

            var juegos = snapshot.Documents
                .Select(d => new
                {
                    Titulo = d.GetValue<string>("Titulo"),
                    TorneoActivos = d.GetValue<int>("TorneoActivos")
                })
                .OrderByDescending(j => j.TorneoActivos)
                .Take(5)
                .Select(j => j.Titulo)
                .ToList();

            return new TendenciasDto
            {
                JuegosMasPopulares = juegos,
                HoraPicoActividad = "20:00 - 23:00",
                TotalTorneosActivos = 0
            };
        }
    }
}
