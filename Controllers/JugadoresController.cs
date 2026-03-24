using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TorneosAPI.Controllers
{
    [ApiController]
    [Route("api/jugadores")]
    [Authorize]
    public class JugadoresController : ControllerBase
    {
        private readonly FirebaseService _firebase;
        private readonly JwtService _jwt;

        public JugadoresController(FirebaseService firebase, JwtService jwt)
        {
            _firebase = firebase;
            _jwt = jwt;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerJugador(string id)
        {
            var db = _firebase.GetDb();
            var doc = await db.Collection("jugadores").Document(id).GetSnapshotAsync();

            if (!doc.Exists)
                return NotFound(new { mensaje = "Jugador no encontrado" });

            var jugador = doc.ConvertTo<Jugador>();

            return Ok(new
            {
                id = doc.Id,
                nombre = jugador.Nombre,
                apellido = jugador.Apellido,
                nombreUsuario = jugador.NombreUsuario,
                correo = jugador.Correo,
                edad = jugador.Edad,
                pais = jugador.Pais,
                rol = jugador.Rol,
                activo = jugador.Activo,
                puntosGlobales = jugador.PuntosGlobales,
                torneoGanados = jugador.TorneoGanados,
                conectado = jugador.Conectado,
                fechaRegistro = jugador.FechaRegistro.ToDateTime()
            });
        }

        [HttpPut("{id}/perfil")]
        public async Task<IActionResult> ActualizarPerfil(string id, [FromBody] ActualizarPerfilDTO dto)
        {
            var jugadorIdToken = _jwt.ObtenerJugadorId(User);
            var rolToken = _jwt.ObtenerRol(User);

            if (jugadorIdToken != id && rolToken != "admin")
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.Pais))
                return BadRequest(new { mensaje = "Nombre, apellido y país son obligatorios" });

            if (dto.Edad < 1)
                return BadRequest(new { mensaje = "La edad debe ser mayor a 0" });

            var db = _firebase.GetDb();
            var doc = await db.Collection("jugadores").Document(id).GetSnapshotAsync();

            if (!doc.Exists)
                return NotFound(new { mensaje = "Jugador no encontrado" });

            await doc.Reference.UpdateAsync(new Dictionary<string, object>
            {
                { "nombre", dto.Nombre },
                { "apellido", dto.Apellido },
                { "edad", dto.Edad },
                { "pais", dto.Pais }
            });

            return Ok(new { mensaje = "Perfil actualizado exitosamente" });
        }
    }
}