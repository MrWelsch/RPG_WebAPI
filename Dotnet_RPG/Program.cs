// Make the Models available for the whole project by using the global keyword
global using Dotnet_RPG.Models;
global using Dotnet_RPG.Services.CharacterService;
global using Dotnet_RPG.Dtos.Character;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using Dotnet_RPG.Data;
global using Dotnet_RPG.Services.WeaponService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Db
builder.Services.AddDbContext<DataContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{   
    // Add Security Definition
    // In essence, enable an option in the swagger UI to enter the bearer token.
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        // Raw Literals since C# 11
        // With that the three curved quotation marks here, you do not
        // have to escape actual quotation marks in this string.
        Description = """Standard Authorization header using the Bearer Scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    }); 
    // Add Filter
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Tell the Web API that the CharacterService Class has to be used
// whenever a Controller wants to inject the ICharacterService.
// In Essence the CharacterService is registered here.
// This is great, because whenever we want to change the implementation
// class, e. g. change CharacterService to another Service entirely,
// we can just change the line below. (Dependency Injection)
builder.Services.AddScoped<ICharacterService, CharacterService>();
// Other Methods:
// Transient:
// Provides a new instance to every Controller and every Service
// even in the same request.
// builder.Services.AddTransient<ICharacterService, CharacterService>();
// Singleton:
// Creates only one instance that is used for every request.
// builder.Services.AddSingleton<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // This thing is the token from our app settings JSON file.
        // So a new symmetric security key that gets the encoded app settings token value.
        IssuerSigningKey = new SymmetricSecurityKey
            // Null forgiving operator ! has to be used here.
            (System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        // Not needed here thus false
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IWeaponService, WeaponService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Add middleware components to the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// // Migrate Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();

// Make it public for integration tests
public partial class Program { }