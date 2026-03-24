using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace TorneosAPI.Controllers
{
   
   
        [ApiController]
        [Route("api/auth")]
        public class AuthController : ControllerBase
        {
            private readonly FirebaseService _firebase;
            private readonly JwtService _jwt;

            public AuthController(FirebaseService firebase, JwtService jwt)
            {
                _firebase = firebase;
                _jwt = jwt;
            }

            [HttpPost("registro")]
            public async Task<IActionResult> Registro([FromBody] RegistroDTO dto)
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido) ||
                    string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena) ||
                    string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Pais))
                    return BadRequest(new { mensaje = "Todos los campos son obligatorios" });

                if (dto.Edad < 1)
                    return BadRequest(new { mensaje = "La edad debe ser mayor a 0" });

                var db = _firebase.GetDb();

                // Verificar correo unico
                var correoQuery = await db.Collection("jugadores")
                    .WhereEqualTo("correo", dto.Correo).GetSnapshotAsync();
                if (correoQuery.Count > 0)
                    return Conflict(new { mensaje = "El correo ya está registrado" });

                // Verificar nombreUsuario unico
                var userQuery = await db.Collection("jugadores")
                    .WhereEqualTo("nombreUsuario", dto.NombreUsuario).GetSnapshotAsync();
                if (userQuery.Count > 0)
                    return Conflict(new { mensaje = "El nombreUsuario ya está en uso" });

                var ahora = Timestamp.FromDateTime(DateTime.UtcNow);

                var jugador = new Jugador
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Correo = dto.Correo,
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                    NombreUsuario = dto.NombreUsuario,
                    Edad = dto.Edad,
                    Pais = dto.Pais,
                    Rol = "jugador",
                    Activo = true,
                    PuntosGlobales = 0,
                    TorneoGanados = 0,
                    Conectado = false,
                    FechaRegistro = ahora,
                    UltimaConexion = ahora
                };

                var docRef = await db.Collection("jugadores").AddAsync(jugador);

                return Ok(new { mensaje = "Jugador registrado exitosamente", id = docRef.Id });
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginDTO dto)
            {
                if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
                    return BadRequest(new { mensaje = "Correo y contraseña son obligatorios" });

                var db = _firebase.GetDb();

                var query = await db.Collection("jugadores")
                    .WhereEqualTo("correo", dto.Correo).GetSnapshotAsync();

                if (query.Count == 0)
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });

                var doc = query.Documents[0];
                var jugador = doc.ConvertTo<Jugador>();
                jugador.Id = doc.Id;

                if (!jugador.Activo)
                    return Unauthorized(new { mensaje = "La cuenta está inactiva" });

                if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, jugador.Contrasena))
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });

                // Actualizar conectado y ultimaConexion
                await doc.Reference.UpdateAsync(new Dictionary<string, object>
            {
                { "conectado", true },
                { "ultimaConexion", Timestamp.FromDateTime(DateTime.UtcNow) }
            });

                var token = _jwt.GenerarToken(jugador.Id!, jugador.Correo, jugador.Rol);

                return Ok(new
                {
                    mensaje = "Login exitoso",
                    token,
                    jugador = new
                    {
                        id = jugador.Id,
                        nombre = jugador.Nombre,
                        correo = jugador.Correo,
                        rol = jugador.Rol
                    }
                });
            }
        }
    }
}