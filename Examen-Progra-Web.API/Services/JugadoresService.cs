using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;
using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Services;

public class JugadoresService : IJugadoresService
{
    private readonly FirestoreDb _db;

    public JugadoresService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<Jugador?> GetJugadorPublico(string id)
    {
        try
        {
            var doc = await _db.Collection("jugadores").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ConvertTo<Jugador>() : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdatePerfil(string id, ActualizarPerfilDto perfilDto)
    {
        try
        {
            var docRef = _db.Collection("jugadores").Document(id);
            var doc = await docRef.GetSnapshotAsync();

            if (!doc.Exists) return false;

            var updates = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(perfilDto.Nombre))
                updates["Nombre"] = perfilDto.Nombre;

            if (!string.IsNullOrWhiteSpace(perfilDto.Apellido))
                updates["Apellido"] = perfilDto.Apellido;

            if (perfilDto.Edad > 0)
                updates["Edad"] = perfilDto.Edad;

            if (!string.IsNullOrWhiteSpace(perfilDto.Pais))
                updates["Pais"] = perfilDto.Pais;

            if (updates.Count > 0)
            {
                await docRef.UpdateAsync(updates);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Jugador>> GetRankingGlobal()
    {
        try
        {
            var snapshot = await _db.Collection("jugadores")
                .Limit(100)
                .GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Jugador>())
                .OrderByDescending(j => j.PuntosGlobales)
                .ToList();
        }
        catch
        {
            return new List<Jugador>();
        }
    }
}
