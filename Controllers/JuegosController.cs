using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace TorneosAPI.Controllers
{
    [ApiController]
    [Route("api/juegos")]
    [Authorize]
    public class JuegosController : ControllerBase
    {
        private readonly FirebaseService _firebase;
        private readonly JwtService _jwt;

        public JuegosController(FirebaseService firebase, JwtService jwt)
        {
            _firebase = firebase;
            _jwt = jwt;
        }

        [HttpPost]
        public async Task<IActionResult> CrearJuego([FromBody] CrearJuegoDTO dto)
        {
            var rol = _jwt.ObtenerRol(User);
            if (rol != "admin")
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                return BadRequest(new { mensaje = "El título es obligatorio" });

            if (string.IsNullOrWhiteSpace(dto.Descripcion) || dto.Descripcion.Length < 20)
                return BadRequest(new { mensaje = "La descripción debe tener mínimo 20 caracteres" });

            var plataformasValidas = new List<string> { "PC", "PS5", "Xbox", "Switch" };
            if (dto.Plataformas == null || dto.Plataformas.Count == 0)
                return BadRequest(new { mensaje = "Debe incluir al menos una plataforma" });

            foreach (var plataforma in dto.Plataformas)
            {
                if (!plataformasValidas.Contains(plataforma))
                    return BadRequest(new { mensaje = $"Plataforma inválida: {plataforma}. Válidas: PC, PS5, Xbox, Switch" });
            }

            var db = _firebase.GetDb();

            // Verificar titulo unico
            var tituloQuery = await db.Collection("juegos")
                .WhereEqualTo("titulo", dto.Titulo).GetSnapshotAsync();
            if (tituloQuery.Count > 0)
                return Conflict(new { mensaje = "Ya existe un juego con ese título" });

            var ahora = Timestamp.FromDateTime(DateTime.UtcNow); // ejemplo

            var juego = new Juego
            {
                Titulo = dto.Titulo,
                Desarrollador = dto.Desarrollador,
                Genero = dto.Genero,
                Plataformas = dto.Plataformas,
                FechaLanzamiento = Timestamp.FromDateTime(dto.FechaLanzamiento.ToUniversalTime()),
                Descripcion = dto.Descripcion,
                JugadoresActivos = 0,
                TorneoActivos = 0,
                Estado = "disponible",
                PuntuacionPromedio = 0,
                FechaAgreg = ahora
            };

            var docRef = await db.Collection("juegos").AddAsync(juego);

            return Ok(new { mensaje = "Juego creado exitosamente", id = docRef.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ListarJuegos(
            [FromQuery] string? genero,
            [FromQuery] string? plataforma,
            [FromQuery] string? desarrollador)
        {
            var db = _firebase.GetDb();

            var query = db.Collection("juegos").WhereEqualTo("estado", "disponible");
            var snapshot = await query.GetSnapshotAsync();

            var juegos = snapshot.Documents.Select(doc =>
            {
                var j = doc.ConvertTo<Juego>();
                j.Id = doc.Id;
                return j;
            }).ToList();

            // Filtros opcionales en memoria
            if (!string.IsNullOrWhiteSpace(genero))
                juegos = juegos.Where(j => j.Genero.ToLower() == genero.ToLower()).ToList();

            if (!string.IsNullOrWhiteSpace(plataforma))
                juegos = juegos.Where(j => j.Plataformas.Any(p =>
                    p.ToLower() == plataforma.ToLower())).ToList();

            if (!string.IsNullOrWhiteSpace(desarrollador))
                juegos = juegos.Where(j => j.Desarrollador.ToLower()
                    .Contains(desarrollador.ToLower())).ToList();

            var resultado = juegos.Select(j => new
            {
                id = j.Id,
                titulo = j.Titulo,
                desarrollador = j.Desarrollador,
                genero = j.Genero,
                plataformas = j.Plataformas,
                estado = j.Estado,
                jugadoresActivos = j.JugadoresActivos,
                torneoActivos = j.TorneoActivos,
                puntuacionPromedio = j.PuntuacionPromedio
            });

            return Ok(resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarJuego(string id, [FromBody] ActualizarJuegoDTO dto)
        {
            var rol = _jwt.ObtenerRol(User);
            if (rol != "admin")
                return Forbid();

            var estadosValidos = new List<string> { "disponible", "mantenimiento", "descontinuado" };
            if (!string.IsNullOrWhiteSpace(dto.Estado) && !estadosValidos.Contains(dto.Estado))
                return BadRequest(new { mensaje = "Estado inválido. Válidos: disponible, mantenimiento, descontinuado" });

            if (!string.IsNullOrWhiteSpace(dto.Descripcion) && dto.Descripcion.Length < 20)
                return BadRequest(new { mensaje = "La descripción debe tener mínimo 20 caracteres" });

            if (dto.PuntuacionPromedio < 0 || dto.PuntuacionPromedio > 5)
                return BadRequest(new { mensaje = "La puntuación promedio debe estar entre 0 y 5" });

            var db = _firebase.GetDb();
            var doc = await db.Collection("juegos").Document(id).GetSnapshotAsync();

            if (!doc.Exists)
                return NotFound(new { mensaje = "Juego no encontrado" });

            var updates = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                updates["descripcion"] = dto.Descripcion;

            if (!string.IsNullOrWhiteSpace(dto.Estado))
                updates["estado"] = dto.Estado;

            if (dto.PuntuacionPromedio > 0)
                updates["puntuacionPromedio"] = dto.PuntuacionPromedio;

            if (updates.Count == 0)
                return BadRequest(new { mensaje = "No se enviaron campos para actualizar" });

            await doc.Reference.UpdateAsync(updates);

            return Ok(new { mensaje = "Juego actualizado exitosamente" });
        }

        [HttpGet("{id}/estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas(string id)
        {
            var db = _firebase.GetDb();
            var doc = await db.Collection("juegos").Document(id).GetSnapshotAsync();

            if (!doc.Exists)
                return NotFound(new { mensaje = "Juego no encontrado" });

            var juego = doc.ConvertTo<Juego>();

            return Ok(new
            {
                id = doc.Id,
                titulo = juego.Titulo,
                jugadoresActivos = juego.JugadoresActivos,
                torneoActivos = juego.TorneoActivos,
                puntuacionPromedio = juego.PuntuacionPromedio,
                estado = juego.Estado
            });
        }
    }
}