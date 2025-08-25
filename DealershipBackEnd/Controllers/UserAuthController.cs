using DealershipBackEnd.DTOs;
using DealershipBackEnd.Interfaces;
using DealershipBackEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace DealershipBackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthInterface _authService;

        public UserAuthController(IUserAuthInterface authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// POST: /api/UserAuth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = await _authService.RegisterAsync(registerDto);

            if (!authResult.Success)
                return BadRequest(authResult.Errors);

            return Ok(new { token = authResult.Token });
        }

        /// <summary>
        /// Logs in a user
        /// POST: /api/UserAuth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResult = await _authService.LoginAsync(loginDto);

            if (!authResult.Success)
                return Unauthorized(authResult.Errors);

            return Ok(new { token = authResult.Token });
        }
    }
}
