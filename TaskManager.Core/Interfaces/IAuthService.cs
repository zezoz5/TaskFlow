
using TaskManager.Core.DTOs.Auth;

namespace TaskManager.Core.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        public Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}