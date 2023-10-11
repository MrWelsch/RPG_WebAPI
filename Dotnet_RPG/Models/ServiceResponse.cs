namespace Dotnet_RPG.Models;

// T is the actual type of the data we want to return
public class ServiceResponse<T>
{
    // The actual Data like the RpgCharacter (T is nullable)
    public T? Data { get; set; }
    // Tell the Frontend if everything went alright
    public bool Success { get; set; }
    // Add a message to explain what happened
    public string Message { get; set; } = string.Empty;
}