using Dotnet_RPG.Dtos.Weapon;
using Dotnet_RPG.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;

namespace Dotnet_RPG.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class WeaponController : ControllerBase
{
    private readonly IWeaponService _weaponService;

    // Constructor injecting the `IWeaponService`
    public WeaponController(IWeaponService weaponService)
    {
        _weaponService = weaponService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newWeapon"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddWeapon(AddWeaponDto newWeapon)
    {
        return Ok(await _weaponService.AddWeapon((newWeapon)));
    }
}