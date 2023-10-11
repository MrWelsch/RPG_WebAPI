using System.Security.Claims;
using Dotnet_RPG.Dtos.Weapon;

namespace Dotnet_RPG.Services.WeaponService;

public class WeaponService : IWeaponService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="mapper"></param>
    public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _context = context;                             // Get DB Data
        _httpContextAccessor = httpContextAccessor;     // Get currently authorized User
        _mapper = mapper;
    }
    
    /// <summary>
    /// Add a Weapon and assign it to the currently authorized User.
    /// </summary>
    /// <param name="newWeapon">An object of the class AddWeaponDto</param>
    /// <returns>A ServiceResponse containing a List of type GetCharacterDto</returns>
    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            // Access the characters from the context.
            var character = await _context.Characters
                // Find the first entity with a given character ID    
                .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                                          // Find the correct user so that we know this character
                                          // really belongs to the currently authorized user.
                                          c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                                              .FindFirstValue(ClaimTypes.NameIdentifier)!));
            // Check for null
            if (character is null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }
            // Assign values if character is not null
            var weapon = new Weapon
            {
                Name = newWeapon.Name,
                Damage = newWeapon.Damage,
                Character = character
            };
            // Add object to DB
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            // Set response data
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