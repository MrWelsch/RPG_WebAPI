namespace Dotnet_RPG.Models;

public class Weapon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    // One - To - One Relationship
    public Character? Character { get; set; }
    // Character can exist without Weapon but Weapon
    // CANNOT exist without assigned Character
    public int CharacterId { get; set; }
}