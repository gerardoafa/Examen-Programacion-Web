using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Models;

[FirestoreData]
public class Torneo
{
    [FirestoreProperty]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Nombre { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Juego { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Organizador { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Descripcion { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Estado { get; set; } = "próximo";

    [FirestoreProperty]
    public string Formato { get; set; } = "individual";

    [FirestoreProperty]
    public int MaxParticipantes { get; set; }

    [FirestoreProperty]
    public int ParticipantesActuales { get; set; }

    [FirestoreProperty]
    public double PrecioInscripcion { get; set; }

    [FirestoreProperty]
    public double PremioTotal { get; set; }

    [FirestoreProperty]
    public Timestamp FechaInicio { get; set; }

    [FirestoreProperty]
    public Timestamp FechaFin { get; set; }

    [FirestoreProperty]
    public Timestamp FechaLimiteInscripcion { get; set; }

    [FirestoreProperty]
    public int MinNivel { get; set; }

    [FirestoreProperty]
    public int MaxNivel { get; set; }

    [FirestoreProperty]
    public bool RequiereEquipo { get; set; }

    [FirestoreProperty]
    public int TamanioEquipo { get; set; }

    [FirestoreProperty]
    public Timestamp FechaCreacion { get; set; }

    [FirestoreProperty]
    public bool ReglasModificadas { get; set; }
}
