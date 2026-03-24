using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Models;

[FirestoreData]
public class Juego
{
    [FirestoreProperty]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Titulo { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Desarrollador { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Genero { get; set; } = string.Empty;

    [FirestoreProperty]
    public List<string> Plataformas { get; set; } = new();

    [FirestoreProperty]
    public Timestamp FechaLanzamiento { get; set; }

    [FirestoreProperty]
    public string Descripcion { get; set; } = string.Empty;

    [FirestoreProperty]
    public int JugadoresActivos { get; set; }

    [FirestoreProperty]
    public int TorneoActivos { get; set; }

    [FirestoreProperty]
    public string Estado { get; set; } = "disponible";

    [FirestoreProperty]
    public double PuntuacionPromedio { get; set; }

    [FirestoreProperty]
    public Timestamp FechaAgregado { get; set; }
}
