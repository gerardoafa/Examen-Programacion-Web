using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services;

public class JuegosService
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
}