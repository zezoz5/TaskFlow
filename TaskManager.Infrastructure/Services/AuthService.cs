using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Core.DTOs.Auth;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Services
{
    public class AuthService(UserManager<AppUser> user, IConfiguration configuration) : IAuthService
    {
        private readonly UserManager<AppUser> _user = user;
        private readonly IConfiguration _config = configuration;

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if a user with that email already exists
            var existingUser = await _user.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exist");
            }

            // Create a new AppUser from the RegisterDto
            var NewUser = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            // Use UserManager to create the user with the password
            var user = await _user.CreateAsync(NewUser, registerDto.Password);

            // If creation fails, handle the error
            if (!user.Succeeded)
            {
                var errors = string.Join(", ", user.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            // Generate a JWT token
            var token = JwtTokenGeneration(NewUser);

            var expiryDays = int.Parse(_config["Jwt:ExpireDays"]!);
            // Return an AuthResponseDto with the token
            return new AuthResponseDto
            {
                Token = token,
                FullName = NewUser.FullName,
                Email = NewUser.Email!,
                ExpireDate = DateTime.UtcNow.AddDays(expiryDays)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _user.FindByEmailAsync(loginDto.Email) ?? throw new Exception("Email not found");
            if (!await _user.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new Exception("Incorrect password");
            }

            var token = JwtTokenGeneration(user);

            var expiryDays = int.Parse(_config["Jwt:ExpireDays"]!);

            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email!,
                ExpireDate = DateTime.UtcNow.AddDays(expiryDays)
            };
        }

        private string JwtTokenGeneration(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new("FullName", user.FullName),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryDays = int.Parse(_config["Jwt:ExpireDays"]!);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}