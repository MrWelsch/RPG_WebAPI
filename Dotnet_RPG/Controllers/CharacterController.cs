using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_RPG.Controllers
{
    // Make use of authentication by using the `Authorize` attribute
    // It can be removed here and added to specific methods only
    [Authorize]
    // Enables API specific features such as Attribute routing and
    // automated HTTP 400 responses if something is wrong with the Model
    [ApiController]
    // API specific routing
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        // Create character service
        private readonly ICharacterService _characterService;

        // Inject the character service into the controller
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        
        // [HttpGet(Name = "GetKnight")] property is not necessary
        // because the web api supports naming conventions and if the name of
        // the method starts with Get, e. g. GetKnight, GetCleric ...,
        // the api assumes that the used HTTP method is also get.
        // Swagger need them though so we define them as follows
        // The routing is done either via 
        // [HttpGet("GetAll")]
        // or by additionally using the Route property
        // [HttpGet]
        // [Route("GetAll")]
        // [AllowAnonymous]
        [HttpGet("GetAll")]
        // Enables us to send specific HTTP status requests back to the client
        // together with the actual data that was requested
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
            // // HTTP 400 response if something is wrong
            // return BadRequest(knight);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetById(int id)
        {
            // Using Linw this returns the first character where the id
            // of it equals the given id
            return Ok(await _characterService.GetCharacterById(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCharacter"></param>
        /// <returns></returns>
        // The Data / JSON-Object is sent via the body of this request
        // not via the URL like in the GetById method
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatedCharacter"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            // Add correct error code (e.g. 404 instead of 200)
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteById(int id)
        {
            var response = await _characterService.DeleteCharacterById(id);
            if (response.Data is null)
                return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCharacterSkill"></param>
        /// <returns></returns>
        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            return Ok(await _characterService.AddCharacterSkill(newCharacterSkill));
        }
    }
}