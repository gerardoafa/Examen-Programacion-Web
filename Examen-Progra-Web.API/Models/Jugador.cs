using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Models;

[FirestoreData]
public class Jugador
{
    [FirestoreProperty]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Nombre { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Apellido { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Correo { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Contrasena { get; set; } = string.Empty;

    [FirestoreProperty]
    public string NombreUsuario { get; set; } = string.Empty;

    [FirestoreProperty]
    public int Edad { get; set; }

    [FirestoreProperty]
    public string Pais { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Rol { get; set; } = "jugador";

    [FirestoreProperty]
    public bool Activo { get; set; } = true;

    [FirestoreProperty]
    public int PuntosGlobales { get; set; }

    [FirestoreProperty]
    public int TorneosGanados { get; set; }

    [FirestoreProperty]
    public Timestamp FechaRegistro { get; set; }

    [FirestoreProperty]
    public bool Conectado { get; set; }

    [FirestoreProperty]
    public Timestamp UltimaConexion { get; set; }
}
