using DealershipBackEnd.Models;
using DealershipBackEnd.DTOs;
using System.Threading.Tasks;

namespace DealershipBackEnd.Interfaces
{
    public interface IUserAuthInterface
    {
        /// <summary>
        /// Registers a new user with username, email, and password
        /// </summary>
        /// <param name="registerDto">Register data transfer object</param>
        /// <returns>Result indicating success or failure</returns>
        Task<AuthResult> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful
        /// </summary>
        /// <param name="loginDto">Login data transfer object</param>
        /// <returns>JWT token and related info</returns>
        Task<AuthResult> LoginAsync(LoginDto loginDto);
    }
}
