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

    public async Task<Jugador?> ActualizarPerfil(string jugadorId, ActualizarPerfilDto perfilDto)
    {
        try
        {
            var docRef = _db.Collection("jugadores").Document(jugadorId);
            var doc = await docRef.GetSnapshotAsync();

            if (!doc.Exists)
            {
                return null;
            }

            var updates = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(perfilDto.Nombre))
            {
                updates["Nombre"] = perfilDto.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(perfilDto.Apellido))
            {
                updates["Apellido"] = perfilDto.Apellido;
            }

            if (perfilDto.Edad > 0)
            {
                updates["Edad"] = perfilDto.Edad;
            }

            if (!string.IsNullOrWhiteSpace(perfilDto.Pais))
            {
                updates["Pais"] = perfilDto.Pais;
            }

            if (updates.Count > 0)
            {
                await docRef.UpdateAsync(updates);
            }

            var updatedDoc = await docRef.GetSnapshotAsync();
            return updatedDoc.ConvertTo<Jugador>();
        }
        catch
        {
            return null;
        }
    }
}
