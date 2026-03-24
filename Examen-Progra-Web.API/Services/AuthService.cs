using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;

namespace Examen_Progra_Web.API.Services;

public class AuthService : IAuthService
{
    private readonly FirestoreDb _db;
    private readonly IConfiguration _configuration;

    public AuthService(FirestoreDb db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<Jugador> Register(RegisterDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Correo) ||
            string.IsNullOrWhiteSpace(registerDto.Contrasena))
        {
            throw new ArgumentNullException("Correo y contraseña son requeridos");
        }

        if (registerDto.Contrasena.Length < 6)
        {
            throw new ArgumentNullException("La contraseña debe tener al menos 6 caracteres");
        }

        if (string.IsNullOrWhiteSpace(registerDto.NombreUsuario))
        {
            throw new ArgumentNullException("Nombre de usuario es requerido");
        }

        var jugadoresRef = _db.Collection("jugadores");

        var correoQuery = await jugadoresRef
            .WhereEqualTo("Correo", registerDto.Correo)
            .GetSnapshotAsync();

        if (correoQuery.Count > 0)
        {
            throw new InvalidOperationException("El correo ya está registrado");
        }

        var usuarioQuery = await jugadoresRef
            .WhereEqualTo("NombreUsuario", registerDto.NombreUsuario)
            .GetSnapshotAsync();

        if (usuarioQuery.Count > 0)
        {
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Contrasena);

        var nuevoJugador = new Jugador
        {
            Id = Guid.NewGuid().ToString(),
            Nombre = registerDto.Nombre,
            Apellido = registerDto.Apellido,
            Correo = registerDto.Correo,
            Contrasena = hashedPassword,
            NombreUsuario = registerDto.NombreUsuario,
            Edad = registerDto.Edad,
            Pais = registerDto.Pais,
            Rol = "jugador",
            Activo = true,
            PuntosGlobales = 0,
            TorneosGanados = 0,
            FechaRegistro = Timestamp.FromDateTime(DateTime.UtcNow),
            Conectado = false,
            UltimaConexion = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        await jugadoresRef.Document(nuevoJugador.Id).SetAsync(nuevoJugador);
        return nuevoJugador;
    }

    public async Task<(Jugador jugador, string token)> Login(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Correo) ||
            string.IsNullOrWhiteSpace(loginDto.Contrasena))
        {
            throw new ArgumentNullException("Correo y contraseña son requeridos");
        }

        var jugadoresRef = _db.Collection("jugadores");
        var query = await jugadoresRef
            .WhereEqualTo("Correo", loginDto.Correo)
            .GetSnapshotAsync();

        if (query.Count == 0)
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        var jugadorDoc = query.Documents[0];
        var jugador = jugadorDoc.ConvertTo<Jugador>();

        if (!jugador.Activo)
        {
            throw new InvalidOperationException("La cuenta está desactivada");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, jugador.Contrasena))
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        await jugadoresRef.Document(jugador.Id).UpdateAsync(new Dictionary<string, object>
        {
            { "Conectado", true },
            { "UltimaConexion", Timestamp.FromDateTime(DateTime.UtcNow) }
        });

        var token = GenerateJwtToken(jugador);
        return (jugador, token);
    }

    public async Task<bool> ValidateToken(string token)
    {
        try
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey)) return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Jugador?> GetJugadorById(string jugadorId)
    {
        try
        {
            var doc = await _db.Collection("jugadores").Document(jugadorId).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Jugador>() : null;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateJwtToken(Jugador jugador)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("JWT SecretKey no configurado");

        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, jugador.Id),
                new Claim(ClaimTypes.Email, jugador.Correo),
                new Claim(ClaimTypes.Name, $"{jugador.Nombre} {jugador.Apellido}"),
                new Claim(ClaimTypes.Role, jugador.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}