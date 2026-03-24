using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.DTOs;

namespace Examen_Progra_Web.API.Services;

public class JugadoresService
{
    private readonly FirestoreDb _db;

    public JugadoresService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<Jugador?> GetJugadorPublico(string id)
    {
        var docRef = _db.Collection("jugadores").Document(id);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return null;
        }

        var jugador = snapshot.ConvertTo<Jugador>();
        
        // Limpiamos la contraseña por seguridad antes de retornar
        jugador.Contrasena = string.Empty; 
        
        return jugador;
    }

    public async Task<bool> UpdatePerfil(string id, UpdatePerfilDto perfilDto)
    {
        var docRef = _db.Collection("jugadores").Document(id);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            throw new KeyNotFoundException("El jugador no existe");
        }

        [cite_start]// El examen especifica que no se debe modificar correo ni nombreUsuario 
        var actualizaciones = new Dictionary<string, object>
        {
            { "Nombre", perfilDto.Nombre },
            { "Apellido", perfilDto.Apellido },
            { "Edad", perfilDto.Edad },
            { "Pais", perfilDto.Pais },
            { "UltimaConexion", Timestamp.FromDateTime(DateTime.UtcNow) }
        };

        try
        {
            await docRef.UpdateAsync(actualizaciones);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Jugador>> GetRankingGlobal()
    {
        [cite_start]// Consulta para el Escenario 5: Jugadores con mayor puntaje global [cite: 156]
        var snapshot = await _db.Collection("jugadores")
            .OrderByDescending("PuntosGlobales")
            .Limit(20)
            .GetSnapshotAsync();

        return snapshot.Documents.Select(d => {
            var j = d.ConvertTo<Jugador>();
            j.Contrasena = string.Empty;
            return j;
        }).ToList();
    }
}