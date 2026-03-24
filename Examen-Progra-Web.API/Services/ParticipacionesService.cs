using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services;

public class ParticipacionesService
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

        [cite_start]if (torneo.Estado != "próximo") throw new InvalidOperationException("El torneo ya no acepta inscripciones"); [cite: 134]
        [cite_start]if (torneo.ParticipantesActuales >= torneo.MaxParticipantes) throw new InvalidOperationException("Torneo lleno"); [cite: 134]
        [cite_start]if (torneo.PrecioInscripcion > 0 && !haPagado) throw new InvalidOperationException("Se requiere confirmación de pago"); [cite: 135]

        var participacion = new Participacion
        {
            Id = Guid.NewGuid().ToString(),
            JugadorId = jugadorId,
            TorneoId = torneoId,
            [cite_start]Estado = "inscrito", [cite: 136]
            Victorias = 0,
            Derrotas = 0,
            PuntosObtenidos = 0,
            FechaInscripcion = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        await _db.Collection("participaciones").Document(participacion.Id).SetAsync(participacion);
        await torneoRef.UpdateAsync("ParticipantesActuales", FieldValue.Increment(1));
    }
}