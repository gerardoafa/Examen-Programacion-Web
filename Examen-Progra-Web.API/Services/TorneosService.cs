using Examen_Progra_Web.API.DTOs;
using Examen_Progra_Web.API.Models;
using Examen_Progra_Web.API.Services.Interface;
using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Services;

public class TorneosService : ITorneosService
{
    private readonly FirestoreDb _db;

    public TorneosService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<Torneo> CrearTorneo(CrearTorneoDto dto, string organizadorId)
    {
        var juegoDoc = await _db.Collection("juegos").Document(dto.JuegoId).GetSnapshotAsync();
        if (!juegoDoc.Exists)
        {
            throw new InvalidOperationException("El juego no existe");
        }

        var nuevoTorneo = new Torneo
        {
            Id = Guid.NewGuid().ToString(),
            Nombre = dto.Nombre,
            Juego = dto.JuegoId,
            Organizador = organizadorId,
            Descripcion = dto.Descripcion,
            Estado = "próximo",
            Formato = dto.Formato,
            MaxParticipantes = dto.MaxParticipantes,
            ParticipantesActuales = 0,
            PrecioInscripcion = dto.PrecioInscripcion,
            PremioTotal = dto.PremioTotal,
            FechaInicio = Timestamp.FromDateTime(dto.FechaInicio.ToUniversalTime()),
            FechaFin = Timestamp.FromDateTime(dto.FechaFin.ToUniversalTime()),
            FechaLimiteInscripcion = Timestamp.FromDateTime(dto.FechaLimiteInscripcion.ToUniversalTime()),
            MinNivel = dto.MinNivel,
            MaxNivel = dto.MaxNivel,
            RequiereEquipo = dto.RequiereEquipo,
            TamanioEquipo = dto.TamanioEquipo,
            FechaCreacion = Timestamp.FromDateTime(DateTime.UtcNow),
            ReglasModificadas = false
        };

        await _db.Collection("torneos").Document(nuevoTorneo.Id).SetAsync(nuevoTorneo);
        return nuevoTorneo;
    }

    public async Task<List<TorneoDto>> ListarTorneos(TorneosFiltradosDto filtros)
    {
        var query = _db.Collection("torneos")
            .WhereEqualTo("Estado", filtros.Estado ?? "próximo");

        var snapshot = await query.GetSnapshotAsync();
        var torneos = new List<TorneoDto>();

        foreach (var doc in snapshot.Documents)
        {
            var torneo = doc.ConvertTo<Torneo>();
            var juegoDoc = await _db.Collection("juegos").Document(torneo.Juego).GetSnapshotAsync();
            var juegoTitulo = juegoDoc.Exists ? juegoDoc.GetValue<string>("Titulo") : "Desconocido";

            var organizadorDoc = await _db.Collection("jugadores").Document(torneo.Organizador).GetSnapshotAsync();
            var organizadorNombre = organizadorDoc.Exists
                ? $"{organizadorDoc.GetValue<string>("Nombre")} {organizadorDoc.GetValue<string>("Apellido")}"
                : "Desconocido";

            if (filtros.PrecioMin.HasValue && torneo.PrecioInscripcion < filtros.PrecioMin.Value) continue;
            if (filtros.PrecioMax.HasValue && torneo.PrecioInscripcion > filtros.PrecioMax.Value) continue;

            torneos.Add(new TorneoDto
            {
                Id = torneo.Id,
                Nombre = torneo.Nombre,
                JuegoId = torneo.Juego,
                JuegoTitulo = juegoTitulo,
                OrganizadorId = torneo.Organizador,
                OrganizadorNombre = organizadorNombre,
                Descripcion = torneo.Descripcion,
                Estado = torneo.Estado,
                Formato = torneo.Formato,
                MaxParticipantes = torneo.MaxParticipantes,
                ParticipantesActuales = torneo.ParticipantesActuales,
                PrecioInscripcion = torneo.PrecioInscripcion,
                PremioTotal = torneo.PremioTotal,
                FechaInicio = torneo.FechaInicio.ToDateTime(),
                FechaFin = torneo.FechaFin.ToDateTime(),
                FechaLimiteInscripcion = torneo.FechaLimiteInscripcion.ToDateTime(),
                MinNivel = torneo.MinNivel,
                MaxNivel = torneo.MaxNivel,
                RequiereEquipo = torneo.RequiereEquipo,
                TamanioEquipo = torneo.TamanioEquipo
            });
        }

        return torneos;
    }

    public async Task<Torneo?> ObtenerTorneoPorId(string torneoId)
    {
        var doc = await _db.Collection("torneos").Document(torneoId).GetSnapshotAsync();
        return doc.Exists ? doc.ConvertTo<Torneo>() : null;
    }

    public async Task<Torneo?> ActualizarTorneo(string torneoId, ActualizarTorneoDto dto, string usuarioId, string rol)
    {
        var docRef = _db.Collection("torneos").Document(torneoId);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists) return null;

        var torneo = doc.ConvertTo<Torneo>();

        if (torneo.Estado != "próximo")
        {
            throw new InvalidOperationException("Solo se pueden editar torneos en estado 'próximo'");
        }

        if (torneo.Organizador != usuarioId && rol != "admin")
        {
            throw new UnauthorizedAccessException("Solo el organizador o un admin puede editar este torneo");
        }

        var updates = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) updates["Nombre"] = dto.Nombre;
        if (!string.IsNullOrWhiteSpace(dto.Descripcion)) updates["Descripcion"] = dto.Descripcion;
        if (dto.MaxParticipantes.HasValue)
        {
            if (dto.MaxParticipantes.Value < torneo.ParticipantesActuales)
            {
                throw new InvalidOperationException("No se puede reducir maxParticipantes por debajo de participantes actuales");
            }
            updates["MaxParticipantes"] = dto.MaxParticipantes.Value;
        }
        if (dto.PrecioInscripcion.HasValue) updates["PrecioInscripcion"] = dto.PrecioInscripcion.Value;
        if (dto.FechaInicio.HasValue) updates["FechaInicio"] = Timestamp.FromDateTime(dto.FechaInicio.Value.ToUniversalTime());
        if (dto.FechaLimiteInscripcion.HasValue) updates["FechaLimiteInscripcion"] = Timestamp.FromDateTime(dto.FechaLimiteInscripcion.Value.ToUniversalTime());
        if (dto.MinNivel.HasValue) updates["MinNivel"] = dto.MinNivel.Value;
        if (dto.MaxNivel.HasValue) updates["MaxNivel"] = dto.MaxNivel.Value;

        if (updates.Count > 0)
        {
            updates["ReglasModificadas"] = true;
            await docRef.UpdateAsync(updates);
        }

        var updatedDoc = await docRef.GetSnapshotAsync();
        return updatedDoc.ConvertTo<Torneo>();
    }

    public async Task<bool> EliminarTorneo(string torneoId, string usuarioId, string rol)
    {
        var docRef = _db.Collection("torneos").Document(torneoId);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists) return false;

        var torneo = doc.ConvertTo<Torneo>();

        if (torneo.Estado != "próximo")
        {
            throw new InvalidOperationException("Solo se pueden cancelar torneos en estado 'próximo'");
        }

        if (torneo.Organizador != usuarioId && rol != "admin")
        {
            throw new UnauthorizedAccessException("Solo el organizador o un admin puede cancelar este torneo");
        }

        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "Estado", "cancelado" }
        });

        return true;
    }

    public async Task<Torneo?> CambiarEstado(string torneoId, string nuevoEstado, string usuarioId, string rol)
    {
        var docRef = _db.Collection("torneos").Document(torneoId);
        var doc = await docRef.GetSnapshotAsync();

        if (!doc.Exists) return null;

        var torneo = doc.ConvertTo<Torneo>();

        if (torneo.Organizador != usuarioId && rol != "admin")
        {
            throw new UnauthorizedAccessException("Solo el organizador o un admin puede cambiar el estado");
        }

        var estadosValidos = new Dictionary<string, string[]>
        {
            { "próximo", new[] { "en progreso" } },
            { "en progreso", new[] { "finalizado" } }
        };

        if (!estadosValidos.ContainsKey(torneo.Estado) || !estadosValidos[torneo.Estado].Contains(nuevoEstado))
        {
            throw new InvalidOperationException($"No se puede cambiar de '{torneo.Estado}' a '{nuevoEstado}'");
        }

        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "Estado", nuevoEstado }
        });

        var updatedDoc = await docRef.GetSnapshotAsync();
        return updatedDoc.ConvertTo<Torneo>();
    }
}
