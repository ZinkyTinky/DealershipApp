using DealershipBackEnd.DTOs;
using DealershipBackEnd.Interfaces;
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
        /// Returns 200 OK with JWT token if successful.
        /// Returns 400 BadRequest with errors if registration fails.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return validation errors if model is invalid

            var authResult = await _authService.RegisterAsync(registerDto);

            if (!authResult.Success)
                return BadRequest(authResult.Errors); // Return errors if registration failed

            // Return the JWT token to the client
            return Ok(new { token = authResult.Token });
        }

        /// <summary>
        /// Logs in a user
        /// POST: /api/UserAuth/login
        /// Returns 200 OK with JWT token if credentials are valid.
        /// Returns 401 Unauthorized if login fails.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Return validation errors if model is invalid

            var authResult = await _authService.LoginAsync(loginDto);

            if (!authResult.Success)
                return Unauthorized(authResult.Errors); // Return 401 if login failed

            // Return JWT token on successful login
            return Ok(new { token = authResult.Token });
        }
    }
}
