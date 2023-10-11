namespace Dotnet_RPG.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    // Many-To-Many Relationship
    public List<Character>? Characters { get; set; }
}