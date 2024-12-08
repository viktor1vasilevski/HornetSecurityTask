using Main.DTOs;
using Main.Enums;
using Main.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserRequest request)
        {
            var response = await _authService.UserRegisterAsync(request);

            if(!response.Success && response.NotificationType == NotificationType.Message)
            {
                return BadRequest(response);
            }
            else if (!response.Success && response.NotificationType == NotificationType.ServerError)
            {
                return StatusCode(500, response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var response = await _authService.UserLoginAsync(request);

            if (!response.Success && response.NotificationType == NotificationType.Message)
            {
                return BadRequest(response);
            }
            else if (!response.Success && response.NotificationType == NotificationType.ServerError)
            {
                return StatusCode(500, response);
            }
            else
            {
                return Ok(response);
            }
        }

    }
}
