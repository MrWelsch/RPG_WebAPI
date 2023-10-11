namespace Dotnet_RPG.Data;

public interface IAuthRepository
{
    Task<ServiceResponse<int>> Register(User user, string password);

    Task<ServiceResponse<string>> Login(string username, string password);
    // Returning a service response here would be a bit over the top because this result
    // is not returned to the client.
    // It is only used within the service itself.
    Task<bool> UserExists(string username);
}