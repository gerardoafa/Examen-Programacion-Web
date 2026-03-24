using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Models;

[FirestoreData]
public class Participacion
{
    [FirestoreProperty]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string JugadorId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string TorneoId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string? EquipoId { get; set; }

    [FirestoreProperty]
    public string Estado { get; set; } = "inscrito";

    [FirestoreProperty]
    public int? Posicion { get; set; }

    [FirestoreProperty]
    public int PuntosObtenidos { get; set; }

    [FirestoreProperty]
    public int PartidasJugadas { get; set; }

    [FirestoreProperty]
    public int Victorias { get; set; }

    [FirestoreProperty]
    public int Derrotas { get; set; }

    [FirestoreProperty]
    public Timestamp FechaInscripcion { get; set; }

    [FirestoreProperty]
    public Timestamp? FechaEliminacion { get; set; }

    [FirestoreProperty]
    public EstadisticasPartida Estadisticas { get; set; } = new();

    [FirestoreProperty]
    public int Penalizaciones { get; set; }

    [FirestoreProperty]
    public bool Pagado { get; set; }
}

[FirestoreData]
public class EstadisticasPartida
{
    [FirestoreProperty]
    public int Asesinatos { get; set; }

    [FirestoreProperty]
    public int Muertes { get; set; }

    [FirestoreProperty]
    public int Asistencias { get; set; }

    [FirestoreProperty]
    public double DanoCausado { get; set; }
}
