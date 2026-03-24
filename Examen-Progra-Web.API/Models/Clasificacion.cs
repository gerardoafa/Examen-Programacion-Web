using Google.Cloud.Firestore;

namespace Examen_Progra_Web.API.Models
{
    [FirestoreData]
    public class Clasificacion
    {
        [FirestoreProperty]
        public string JugadorId { get; set; } = string.Empty;

        [FirestoreProperty]
        public string JuegoId { get; set; } = string.Empty;

        [FirestoreProperty]
        public int Posicion { get; set; }

        [FirestoreProperty]
        public int PuntosJuego { get; set; }

        [FirestoreProperty]
        public int NivelJuego { get; set; }

        [FirestoreProperty]
        public int TorneoGanados { get; set; }

        [FirestoreProperty]
        public int TotalPartidas { get; set; }

        [FirestoreProperty]
        public double RatioVictoria { get; set; }   // 0-100

        [FirestoreProperty]
        public int Racha { get; set; }

        [FirestoreProperty]
        public int RachaMaxima { get; set; }

        [FirestoreProperty]
        public int MedallasOro { get; set; }

        [FirestoreProperty]
        public int MedallasPlata { get; set; }

        [FirestoreProperty]
        public int MedallasBronce { get; set; }

        [FirestoreProperty]
        public Timestamp FechaActualizacion { get; set; }

        [FirestoreProperty]
        public string EstiloJuego { get; set; } = string.Empty;

        [FirestoreProperty]
        public List<string> Logros { get; set; } = new();
    }
}
