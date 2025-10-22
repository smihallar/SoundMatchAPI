using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.AuthModels;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SoundMatchAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<AuthResult> RegisterUserAsync(UserRegisterRequest request)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResult
                    {
                        Succeeded = false,
                        Errors = new List<string> { "User with this email already exists." }
                    };
                }
                var newUser = new User
                {
                    Email = request.Email,
                    UserName = request.Email // When not connected to Spotify, username is the email
                };
                var createUserResult = await userManager.CreateAsync(newUser, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return new AuthResult
                    {
                        Succeeded = false,
                        Errors = createUserResult.Errors.Select(e => e.Description).ToList()
                    };
                }
                await userManager.AddToRoleAsync(newUser, ApiRoles.User);
                return new AuthResult
                {
                    Succeeded = true,
                    UserId = newUser.Id,
                    Errors = null
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<AuthResult> LoginUserAsync(UserLoginRequest request)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
                if (user == null || passwordValid == false)
                {
                    return new AuthResult
                    {
                        Succeeded = false,
                        Errors = new List<string> { "Wrong email or password." },
                    };
                }

                string tokenString = await GenerateToken(user);

                return new AuthResult
                {
                    Succeeded = true,
                    Errors = null,
                    Token = tokenString,
                    UserId = user.Id,
                };
            }
            catch
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new List<string> { "An error occurred during login." },
                };
            }
        }

        private async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id),
            }
            .Union(roleClaims)
            .Union(userClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
