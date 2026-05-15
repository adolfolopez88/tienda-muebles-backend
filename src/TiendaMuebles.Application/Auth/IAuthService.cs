using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshRequest request);
    Task<UserResponse?> GetCurrentUserAsync(Guid userId);
}
