using DealershipBackEnd.DTOs;
using DealershipBackEnd.Interfaces;
using DealershipBackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DealershipBackEnd.Repositories
{
    /// <summary>
    /// Repository for user authentication and registration.
    /// Implements IUserAuthInterface for abstraction.
    /// </summary>
    public class UserAuthRepository : IUserAuthInterface
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserAuthRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// Returns AuthResult indicating success/failure and JWT token if successful.
        /// </summary>
        public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Username already exists" }
                };
            }

            // Create new user entity
            var newUser = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            // Create user with password
            var isCreated = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!isCreated.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = isCreated.Errors.Select(e => e.Description)
                };
            }

            // Optional: Add default role
            // await _userManager.AddToRoleAsync(newUser, "User");

            // Generate JWT token for the new user
            var token = GenerateJwtToken(newUser);

            return new AuthResult
            {
                Success = true,
                Token = token
            };
        }

        /// <summary>
        /// Logs in a user by validating username and password.
        /// Returns AuthResult with JWT token if successful.
        /// </summary>
        public async Task<AuthResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Invalid username or password" }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Invalid username or password" }
                };
            }

            // Generate JWT token on successful login
            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Token = token
            };
        }

        /// <summary>
        /// Generates a JWT token for a given user.
        /// The token includes basic claims and expires after configured minutes.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = Convert.ToInt32(jwtSettings["ExpiryMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Claims included in the JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
