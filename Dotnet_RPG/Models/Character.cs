using Microsoft.AspNetCore.Authentication;

namespace Dotnet_RPG.Models;

public class Character
{
    // Properties
    public int Id {get; set; }
    public string Name { get; set; } = "Frodo";
    public int HitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Defense { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    // Enum Property
    public RpgClass Class { get; set; } = RpgClass.Knight;
    // User Relationship (One)
    public User? User { get; set; }
    // Seeding Purposes
    // Character needs to be assigned to an existing User
    public int UserId {get; set; }
    // One - To - One Relationship
    public Weapon? Weapon { get; set; }
    // Many-To-Many Relationship
    public List<Skill>? Skills { get; set; }
}