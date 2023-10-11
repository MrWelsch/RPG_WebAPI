using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet_RPG.Data;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    // Added to access appsettings.json file
    private readonly IConfiguration _configuration;

    public AuthRepository(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        var response = new ServiceResponse<int>();
        // If the user already exists, a response with success is false and a
        // message like user already exists should be returned.
        if(await UserExists(user.Username))
        {
            response.Success = false;
            response.Message = "User already exists.";
            return response;
        }
        
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        // store the out byte[] arrays in the user object
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        response.Data = user.Id;
        // This was not implemented in the course,
        // but without it success would be false
        response.Success = true;
        return response;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
        if (user is null)
        {
            response.Success = false;
            // Now regarding the message, always take into account that useful information is already revealed to a
            // potential attacker if you tell specifically that the user has not been found or that the password has
            // been wrong in thes cases, an attacker would know that a particular user exists.
            // It's more critical with email addresses and he or she could try to find the correct password with a
            // brute force attack.
            response.Message = "User not found.";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong Password.";
        }
        else
        {
            response.Data = CreateToken(user);
            // This was not implemented in the course,
            // but without it success would be false
            response.Success = true;
        }

        return response;
    }

    // Public because it may has to be used in the controller later on.
    public async Task<bool> UserExists(string username)
    {
        // Using a ServiceResponse in this method would be over the top.
        if (await _context.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower())))
        {
            return true;
        }
        return false;
    }

    // Out parameters are used to set them then in the Register method in the user object.
    // Those values aren't returned.
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            // This computed hash value will be compared with the password
            // hash in the database byte by byte by using the hash SequenceEqual method.
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        // List of claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        // Symmetric Security Key 
        // Get Token from appsettings.json
        var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
        if (appSettingsToken is null)
            throw new Exception("AppSettings Token is null.");
        // Create Instance of the Symmetric Security Class
        SymmetricSecurityKey key = new SymmetricSecurityKey
            (System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
        // Create new Signing Credentials
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        // Create Security Token Descriptor
        // The Descriptor object gets the information that is used to create the final token.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // Set the subject to a new claims identity with our list of claims.
            Subject = new ClaimsIdentity(claims),
            // Invalidates after 1 Day
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };
        // New JWT Security Token Handler
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        // Create the token
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        // Serialize SecurityToken to a JSON Web Token
        return tokenHandler.WriteToken(token);
    }
}