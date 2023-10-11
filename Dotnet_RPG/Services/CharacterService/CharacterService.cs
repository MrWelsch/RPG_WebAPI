using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp;

namespace Dotnet_RPG.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Returns the User ID.
    /// </summary>
    /// <returns>A User ID of type int</returns>
    private int GetUserId()
        => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    /// <summary>
    /// Returns all Characters of the currently authorized User.
    /// </summary>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        var dbCharacters = await _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .Where(c => c.User!.Id == GetUserId()).ToListAsync();
        serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        return serviceResponse;
    }

    /// <summary>
    /// Get a Character of the currently authorized User specified by it's ID.
    /// </summary>
    /// <param name="id">The specified Character ID</param>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        var dbCharacter = await _context.Characters
            .Include(c => c.Weapon)
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
        serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        return serviceResponse;
    }

    /// <summary>
    /// Add a Character and assign it to the currently authorized User.
    /// </summary>
    /// <param name="newCharacter">An object of the class AddCharacterDto</param>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        // Assign added character to current User
        character.User = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == GetUserId());
        
        _context.Characters.Add(character);
        // Writes the changes to the database and also creates the character id
        await _context.SaveChangesAsync();
        
        // Select returns an IEnumerable but we want a list, so ToListAsync() is used.
        serviceResponse.Data = 
            await _context.Characters
                // Filter the characters assigned to current User    
                .Where(c => c.User!.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
        return serviceResponse;
    }

    /// <summary>
    /// Update a Character of the currently authenticated User.
    /// </summary>
    /// <param name="updatedCharacter">An object of the class UpdateCharacterDto</param>
    /// <returns>A ServiceResponse containing an object of type GetCharacterDto</returns>
    /// <exception cref="Exception">Character not found</exception>
    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        // Catch non existing id's
        try
        {
            var character = 
                await _context.Characters
                    // Include is used to make the user available in the if statement below
                    // because else DbContext of Entity Framework would only know the UserId
                    // as null.
                    // Short: To access related objects they have to be included first.
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
            // Check for null || Check if Id doesn't equal currently authenticated `User.Id`
            if (character is null || character.User!.Id != GetUserId())
                throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
            
            // // Modify Character with AutoMapper
            // _mapper.Map<Character>(updatedCharacter);
            // // Another way. This implementation needs to be added to Root/AutoMapperProfiles.
            // _mapper.Map(updatedCharacter, character);

            // Modify Character manually
            character.Name = updatedCharacter.Name;
            character.HitPoints = updatedCharacter.HitPoints;
            character.Strength = updatedCharacter.Strength;
            character.Defense = updatedCharacter.Defense;
            character.Intelligence = updatedCharacter.Intelligence;
            character.Class = updatedCharacter.Class;

            // Writes the changes to the database and also creates the character id
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        
        return serviceResponse;
    }

    /// <summary>
    /// Delete a Character of the currently authenticated User.
    /// </summary>
    /// <param name="id">The specified Character ID</param>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    /// <exception cref="Exception">Character not found</exception>
    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        // Catch non existing id's
        try
        {
            var character = 
                await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            // Catch nulled id
            if (character is null)
                throw new Exception($"Character with Id '{id}' not found.");

            _context.Characters.Remove(character);

            // Writes the changes to the database and also creates the character id
            await _context.SaveChangesAsync();
            serviceResponse.Data = 
                await _context.Characters
                    .Where(c => c.User!.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c))
                    .ToListAsync();
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }
        
        return serviceResponse;
    }

    /// <summary>
    /// Add a Skill to a Character of the currently authenticated User.
    /// </summary>
    /// <param name="newCharacterSkill">An object of the class AddCharacterSkillDto</param>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = await _context.Characters
                .Include(c => c.Weapon)
                // If we would need to include additional related data,
                // meaning the skills would have lists of different effects for instance,
                // we can use `.Include().ThenInclude()' to further include this data.
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                                          c.User!.Id == GetUserId());
            if (character is null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            var skill = await _context.Skills
                .FirstOrDefaultAsync((s => s.Id == newCharacterSkill.SkillId));
            
            if (skill is null)
            {
                response.Success = false;
                response.Message = "Skill not found.";
                return response;
            }
            
            character.Skills!.Add(skill);
            await _context.SaveChangesAsync();

            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }
}