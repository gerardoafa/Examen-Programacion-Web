using Google.Cloud.Firestore;
using Examen_Progra_Web.API.Models;

namespace Examen_Progra_Web.API.Services;

public class TorneosService
{
    private readonly FirestoreDb _db;

    public TorneosService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<Torneo> CrearTorneo(Torneo torneo)
    {
        [cite_start]if (torneo.FechaInicio <= DateTime.UtcNow) throw new ArgumentException("La fecha de inicio debe ser posterior a hoy"); [cite: 121]
        [cite_start]if (torneo.MaxParticipantes <= 2) throw new ArgumentException("Máximo de participantes debe ser mayor a 2"); [cite: 121]

        torneo.Id = Guid.NewGuid().ToString();
        [cite_start]torneo.Estado = "próximo"; [cite: 121]
        [cite_start]torneo.ParticipantesActuales = 0; [cite: 121]
        [cite_start]torneo.ReglasModificadas = false; [cite: 121]
        torneo.FechaCreacion = Timestamp.FromDateTime(DateTime.UtcNow);

        await _db.Collection("torneos").Document(torneo.Id).SetAsync(torneo);
        return torneo;
    }

    public async Task CambiarEstado(string torneoId, string nuevoEstado)
    {
        var docRef = _db.Collection("torneos").Document(torneoId);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists) throw new KeyNotFoundException("Torneo no encontrado");

        [cite_start]// Lógica de transición válida [cite: 129]
        await docRef.UpdateAsync("Estado", nuevoEstado);

        if (nuevoEstado == "finalizado")
        {
            [cite_start]// Aquí se dispararía la lógica de posiciones automáticas [cite: 130]
        }
    }
}