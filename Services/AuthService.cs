using Google.Cloud.Firestore;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using BCrypt.Net;
using Examen_Progra_Web.API.Services; // ← para usar FirebaseService y JwtService

namespace Examen_Progra_Web.API.Services.Interface
{
    public class AuthService
    {
        private readonly FirebaseService _firebase;
        private readonly JwtService _jwt;

        public AuthService(FirebaseService firebase, JwtService jwt)
        {
            _firebase = firebase;
            _jwt = jwt;
        }

        // ==================== REGISTRO ====================
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var db = _firebase.GetDb();

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena) ||
                string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Pais))
            {
                return new AuthResponseDto { Success = false, Message = "Todos los campos son obligatorios" };
            }
            if (dto.Edad < 1)
                return new AuthResponseDto { Success = false, Message = "La edad debe ser mayor a 0" };

            // Verificar correo único
            var correoQuery = await db.Collection("jugadores")
                .WhereEqualTo("correo", dto.Correo).GetSnapshotAsync();
            if (correoQuery.Count > 0)
                return new AuthResponseDto { Success = false, Message = "El correo ya está registrado" };

            // Verificar nombreUsuario único
            var userQuery = await db.Collection("jugadores")
                .WhereEqualTo("nombreUsuario", dto.NombreUsuario).GetSnapshotAsync();
            if (userQuery.Count > 0)
                return new AuthResponseDto { Success = false, Message = "El nombreUsuario ya está en uso" };

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
            var jugadorId = docRef.Id;

            // Generar token
            var token = _jwt.GenerarToken(jugadorId, dto.Correo, "jugador");

            return new AuthResponseDto
            {
                Success = true,
                Message = "Jugador registrado exitosamente",
                Token = token,
                Jugador = new JugadorDto
                {
                    Id = jugadorId,
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    NombreUsuario = dto.NombreUsuario,
                    Correo = dto.Correo,
                    Rol = "jugador",
                    PuntosGlobales = 0,
                    TorneosGanados = 0
                }
            };
        }

        // ==================== LOGIN ====================
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var db = _firebase.GetDb();

            if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
                return new AuthResponseDto { Success = false, Message = "Correo y contraseña son obligatorios" };

            var query = await db.Collection("jugadores")
                .WhereEqualTo("correo", dto.Correo).GetSnapshotAsync();

            if (query.Count == 0)
                return new AuthResponseDto { Success = false, Message = "Credenciales inválidas" };

            var doc = query.Documents[0];
            var jugador = doc.ConvertTo<Jugador>();
            jugador.Id = doc.Id;

            if (!jugador.Activo)
                return new AuthResponseDto { Success = false, Message = "La cuenta está inactiva" };

            if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, jugador.Contrasena))
                return new AuthResponseDto { Success = false, Message = "Credenciales inválidas" };

            // Actualizar estado
            await doc.Reference.UpdateAsync(new Dictionary<string, object>
            {
                { "conectado", true },
                { "ultimaConexion", Timestamp.FromDateTime(DateTime.UtcNow) }
            });

            var token = _jwt.GenerarToken(jugador.Id!, jugador.Correo, jugador.Rol);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                Jugador = new JugadorDto
                {
                    Id = jugador.Id,
                    Nombre = jugador.Nombre,
                    Apellido = jugador.Apellido,
                    NombreUsuario = jugador.NombreUsuario,
                    Correo = jugador.Correo,
                    Rol = jugador.Rol,
                    PuntosGlobales = jugador.PuntosGlobales,
                    TorneosGanados = jugador.TorneosGanados
                }
            };
        }
    }
}
