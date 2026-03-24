namespace Examen_Progra_Web.API.Services;

using Google.Cloud.Firestore;




    public class FirebaseService
    {
        private readonly FirestoreDb _db;

        public FirebaseService(IConfiguration configuration)
        {
            string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-credentials.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            string projectId = configuration["Firebase:ProjectId"]!;
            _db = FirestoreDb.Create(projectId);
        }

        public FirestoreDb GetDb() => _db;
    }
