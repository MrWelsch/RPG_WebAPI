namespace Dotnet_RPG.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    // Password as Hash-Value with Salt to create a unique password hash.
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    // RPG Characters Relationship (Many)
    public List<Character>? Characters { get; set; }
}