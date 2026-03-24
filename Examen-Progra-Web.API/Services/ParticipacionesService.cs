using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;

namespace Examen_Progra_Web.API.Services;

public class ParticipacionesService : IParticipacionesService
{
    private readonly FirestoreDb _db;

    public ParticipacionesService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task InscribirJugador(string torneoId, string jugadorId, bool haPagado)
    {
        var torneoRef = _db.Collection("torneos").Document(torneoId);
        var torneoSnap = await torneoRef.GetSnapshotAsync();
        var torneo = torneoSnap.ConvertTo<Torneo>();

        if (torneo.Estado != "próximo") throw new InvalidOperationException("El torneo ya no acepta inscripciones");
        if (torneo.ParticipantesActuales >= torneo.MaxParticipantes) throw new InvalidOperationException("Torneo lleno");
        if (torneo.PrecioInscripcion > 0 && !haPagado) throw new InvalidOperationException("Se requiere confirmación de pago");

        var queryExiste = await _db.Collection("participaciones")
            .WhereEqualTo("TorneoId", torneoId)
            .WhereEqualTo("JugadorId", jugadorId)
            .GetSnapshotAsync();

        if (queryExiste.Count > 0) throw new InvalidOperationException("Ya estás inscrito en este torneo");

        var participacion = new Participacion
        {
            Id = Guid.NewGuid().ToString(),
            JugadorId = jugadorId,
            TorneoId = torneoId,
            Estado = "inscrito",
            Victorias = 0,
            Derrotas = 0,
            PuntosObtenidos = 0,
            PartidasJugadas = 0,
            FechaInscripcion = Timestamp.FromDateTime(DateTime.UtcNow),
            Pagado = haPagado
        };

        await _db.Collection("participaciones").Document(participacion.Id).SetAsync(participacion);
        await torneoRef.UpdateAsync("ParticipantesActuales", FieldValue.Increment(1));
    }

    public async Task<List<Participacion>> GetParticipantesTorneo(string torneoId)
    {
        var query = _db.Collection("participaciones").WhereEqualTo("TorneoId", torneoId);
        var snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Participacion>()).ToList();
    }

    public async Task<bool> ActualizarResultado(string participacionId, bool victoria, int puntosPartida)
    {
        var docRef = _db.Collection("participaciones").Document(participacionId);
        var doc = await docRef.GetSnapshotAsync();
        if (!doc.Exists) return false;

        var updates = new Dictionary<string, object>
        {
            { "PartidasJugadas", FieldValue.Increment(1) },
            { "PuntosObtenidos", FieldValue.Increment(puntosPartida) }
        };

        if (victoria)
            updates["Victorias"] = FieldValue.Increment(1);
        else
            updates["Derrotas"] = FieldValue.Increment(1);

        await docRef.UpdateAsync(updates);
        return true;
    }

    public async Task<List<Participacion>> GetMisTorneos(string jugadorId)
    {
        var query = _db.Collection("participaciones").WhereEqualTo("JugadorId", jugadorId);
        var snapshot = await query.GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<Participacion>()).ToList();
    }

    public async Task<bool> AbandonarTorneo(string participacionId, string jugadorId)
    {
        var docRef = _db.Collection("participaciones").Document(participacionId);
        var doc = await docRef.GetSnapshotAsync();
        if (!doc.Exists) return false;

        var participacion = doc.ConvertTo<Participacion>();
        if (participacion.JugadorId != jugadorId) return false;

        var torneoRef = _db.Collection("torneos").Document(participacion.TorneoId);
        var torneoSnap = await torneoRef.GetSnapshotAsync();
        var torneo = torneoSnap.ConvertTo<Torneo>();

        if (torneo.Estado != "próximo") return false;

        await docRef.UpdateAsync(new Dictionary<string, object> { { "Estado", "abandonado" } });
        await torneoRef.UpdateAsync("ParticipantesActuales", FieldValue.Increment(-1));
        return true;
    }
}
