using Dotnet_RPG.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_RPG.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepo;

    // Inject IAuthRepository
    public AuthController(IAuthRepository authRepo)
    {
        _authRepo = authRepo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    // The username and password could be sent via the URL but it is advised to hide this
    // information in the body.
    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
        // The idea behind the user parameter is that more information could be added to the user upon registration,
        // like an email address, birth, favorite color, and so on.
        // That's why we that object is used here.
        // That's totally preferential and the necessary methods could be rewritten to just send a username
        // and password without any additional information, of course.
        var response = await _authRepo.Register(
            new User { Username = request.Username }, request.Password
        );
        // Check if the success member of the response is false.
        // If so a bad request (400) is returned.
        // Otherwise sucess (200) will be returned.
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
    {
        var response = await _authRepo.Login(request.Username, request.Password);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}