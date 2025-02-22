using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AuthenticationApi.Infrastructure.Firebase
{
    public class FirebaseAuthService
    {
        private readonly FirebaseApp _firebaseApp;

        public FirebaseAuthService(IConfiguration configuration)
        {
            var firebaseConfigPath = configuration["Firebase:CredentialPath"];
            if (string.IsNullOrEmpty(firebaseConfigPath))
            {
                throw new ArgumentNullException(nameof(firebaseConfigPath), "Firebase credential path is missing.");
            }

            if (!File.Exists(firebaseConfigPath))
            {
                throw new FileNotFoundException($"Firebase credential file not found at: {firebaseConfigPath}");
            }

            _firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(firebaseConfigPath)
            });
        }

        public async Task<FirebaseToken> VerifyOAuthToken(string idToken)
        {
            var auth = FirebaseAuth.GetAuth(_firebaseApp);
            return await auth.VerifyIdTokenAsync(idToken);
        }
    }
}
