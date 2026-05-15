using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TiendaMuebles.Application.Auth;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Domain.Entities;
using TiendaMuebles.Infrastructure.Data;

namespace TiendaMuebles.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtConfig _jwtConfig;
    private readonly PasswordHasher _hasher;

    public AuthService(AppDbContext db, IOptions<JwtConfig> jwtOptions, PasswordHasher hasher)
    {
        _db = db;
        _jwtConfig = jwtOptions.Value;
        _hasher = hasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email ya registrado");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = Domain.Enums.UserRole.Customer
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new UnauthorizedAccessException("Credenciales invalidas");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales invalidas");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request)
    {
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken)
            ?? throw new UnauthorizedAccessException("Refresh token invalido");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token expirado o revocado");

        storedToken.RevokedAt = DateTime.UtcNow;
        return await GenerateTokensAsync(storedToken.User);
    }

    public async Task<UserResponse?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        return user is null ? null : new UserResponse(user.Id, user.Email, user.Role.ToString());
    }

    private async Task<AuthResponse> GenerateTokensAsync(User user)
    {
        var accessToken = GenerateJwt(user);
        var refreshToken = GenerateRefreshToken(user.Id);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshToken.Token);
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays)
        };
    }
}
