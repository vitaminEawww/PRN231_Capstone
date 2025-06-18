namespace Services.Interfaces;

public interface IFirebaseService
{
    Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data = null);
}