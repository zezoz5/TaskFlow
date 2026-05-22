using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs.Auth;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
    }
}