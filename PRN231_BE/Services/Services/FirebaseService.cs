using DataAccess.Models.Notifications;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;

namespace Services.Services;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseMessaging _messaging;

    public FirebaseService(IConfiguration configuration)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var firebaseConfig = configuration.GetSection("Firebase").Get<FirebaseConfig>();
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(firebaseConfig.CredentialsPath)
            });
        }
        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data = null)
    {
        var message = new Message()
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = data
        };

        try
        {
            await _messaging.SendAsync(message);
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error sending notification: {ex.Message}");
            throw;
        }
    }
}