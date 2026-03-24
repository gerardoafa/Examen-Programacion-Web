using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;

namespace Examen_Progra_Web.API.Services;

public class JuegosService : IJuegosService
{
    private readonly FirestoreDb _db;

    public JuegosService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<Juego> CrearJuego(Juego juego)
    {
        if (string.IsNullOrWhiteSpace(juego.Titulo)) throw new ArgumentException("El título es requerido");
        if (juego.Descripcion?.Length < 20) throw new ArgumentException("La descripción debe tener al menos 20 caracteres");

        var juegosRef = _db.Collection("juegos");
        var query = await juegosRef.WhereEqualTo("Titulo", juego.Titulo).GetSnapshotAsync();
        
        if (query.Count > 0) throw new InvalidOperationException("El título del juego ya existe");

        juego.Id = Guid.NewGuid().ToString();
        juego.JugadoresActivos = 0;
        juego.TorneoActivos = 0;
        juego.Estado = "disponible";
        juego.FechaAgregado = Timestamp.FromDateTime(DateTime.UtcNow);

        await juegosRef.Document(juego.Id).SetAsync(juego);
        return juego;
    }

    public async Task<List<Juego>> GetJuegosDisponibles(string? genero, string? plataforma)
    {
        Query query = _db.Collection("juegos").WhereEqualTo("Estado", "disponible");

        if (!string.IsNullOrEmpty(genero)) query = query.WhereEqualTo("Genero", genero);
        if (!string.IsNullOrEmpty(plataforma)) query = query.WhereArrayContains("Plataformas", plataforma);

        var snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Juego>()).ToList();
    }

    public async Task<Juego?> GetEstadisticasJuego(string id)
    {
        var doc = await _db.Collection("juegos").Document(id).GetSnapshotAsync();
        return doc.Exists ? doc.ConvertTo<Juego>() : null;
    }

    public async Task<bool> ActualizarJuego(string id, string? descripcion, double? puntuacion, string? estado)
    {
        var docRef = _db.Collection("juegos").Document(id);
        var doc = await docRef.GetSnapshotAsync();
        if (!doc.Exists) return false;

        var updates = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(descripcion)) updates["Descripcion"] = descripcion;
        if (puntuacion.HasValue) updates["PuntuacionPromedio"] = puntuacion.Value;
        if (!string.IsNullOrEmpty(estado)) updates["Estado"] = estado;

        if (updates.Count > 0) await docRef.UpdateAsync(updates);
        return true;
    }
}
