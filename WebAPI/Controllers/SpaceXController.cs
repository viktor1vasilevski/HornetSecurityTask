using Main.Enums;
using Main.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceXController : ControllerBase
    {
        private readonly ISpaceXService _spaceXService;

        public SpaceXController(ISpaceXService spaceXService)
        {
            _spaceXService = spaceXService;
        }


        [HttpGet("latest")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetLatestLaunch()
        {
            var response = await _spaceXService.GetLatestLaunchDataAsync();

            if(!response.Success && response.NotificationType == NotificationType.ServerError)
                return StatusCode(500, response);
            return Ok(response);
        }

        [HttpGet("launches/{type}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetLaunchList([FromRoute] string type)
        {
            var response = await _spaceXService.GetListLaunchDataAsync(type);

            if (!response.Success && response.NotificationType == NotificationType.ServerError)
                return StatusCode(500, response);
            return Ok(response);
        }
    }
}
