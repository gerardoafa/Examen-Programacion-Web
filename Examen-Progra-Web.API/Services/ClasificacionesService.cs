using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using System.Security.Claims;

namespace Examen_Progra_Web.API.Services
{
    public class ClasificacionesService
    {
        private readonly FirestoreDb _db;

        public ClasificacionesService(FirestoreDb db)
        {
            _db = db;
        }

        // ENDPOINT 1: Ranking global por juego (paginado)
        public async Task<RankingResponseDto> GetRankingPorJuegoAsync(string juegoId, int page = 1, int pageSize = 50)
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
                var c = d.ConvertTo<Clasificacion>();
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

            return new RankingResponseDto
            {
                PaginaActual = page,
                TotalPaginas = 10,
                TotalRegistros = ranking.Count,
                Ranking = ranking
            };
        }

        // ENDPOINT 2: Posición personal del jugador autenticado
        public async Task<MiDesempenoDto?> GetPosicionPersonalAsync(string juegoId, string userId)
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
    }
}
