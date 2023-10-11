namespace Dotnet_RPG.Data;

// Save Changes to Database
public class DataContext : DbContext
{
    // Constructor
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    // Data Seeding
    // To seed entities that have relationships proper FK has to be seeded.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Generate Password Hash
        var password = "1234";
        byte[] passwordHash;
        byte[] passwordSalt;
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User {Id = 1, Username = "Nico", PasswordHash = passwordHash, PasswordSalt = passwordSalt},
            new User {Id = 2,  Username = "Giu", PasswordHash = passwordHash, PasswordSalt = passwordSalt}
        );
        // Seed Characters
        modelBuilder.Entity<Character>().HasData(
            new Character 
                {
                    Id = 1, 
                    Name = "Frodo", 
                    HitPoints = 50, 
                    Strength = 60,
                    Defense = 20,
                    Intelligence = 10,
                    // TODO: Can this be solved in a better way?
                    UserId = 1,
                },
                new Character 
                {
                    Id = 2, 
                    Name = "Sam", 
                    HitPoints = 80, 
                    Strength = 10,
                    Defense = 40,
                    Intelligence = 60,
                    UserId = 1,
                },
                new Character 
                {
                    Id = 3, 
                    Name = "Sauron", 
                    HitPoints = 200, 
                    Strength = 100,
                    Defense = 100,
                    Intelligence = 100,
                    UserId = 2,
                }
        );
        // Seed Skills
        modelBuilder.Entity<Skill>().HasData(
            new Skill{Id = 1, Name = "Fireball", Damage = 30},
            new Skill{Id = 2, Name = "Frenzy", Damage = 20},
            new Skill{Id = 3, Name = "Blizzard", Damage = 50}
        );
        // Seed Weapons
        modelBuilder.Entity<Weapon>().HasData(
            new Weapon{Id = 1, Name = "Excalibur", Damage = 200, CharacterId = 1},
            new Weapon{Id = 2, Name = "Master Sword", Damage = 150, CharacterId = 2},
            new Weapon{Id = 3, Name = "Ashbringer", Damage = 100, CharacterId = 3}
        );
    }

    // Enables querying and saving RPG Characters
    // The name of the property will be the name of the 
    // corresponding database table.
    // Whenever a table of a specific model should be created in the database
    // a corresponding DBSet has to be specified here.
    // Naming Convention: Pluralize the name of the model.
    public DbSet<Character> Characters { get; set; }
    // // If {get; set; } throws an error:
    // public DbSet<Character> Characters => Set<Character>();
    public DbSet<User> Users { get; set; }
    public DbSet<Weapon> Weapons { get; set; }
    public DbSet<Skill> Skills { get; set; }
}