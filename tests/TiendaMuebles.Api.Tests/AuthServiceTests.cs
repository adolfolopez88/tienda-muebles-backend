using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TiendaMuebles.Application.Auth;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Infrastructure.Auth;
using TiendaMuebles.Infrastructure.Data;
using Xunit;

namespace TiendaMuebles.Api.Tests;

public class AuthServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private AuthService CreateAuthService(AppDbContext db)
    {
        var jwtConfig = Options.Create(new JwtConfig
        {
            Secret = "this-is-a-test-secret-key-that-is-long-enough!!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        });
        var hasher = new PasswordHasher();
        return new AuthService(db, jwtConfig, hasher);
    }

    [Fact]
    public async Task RegisterAsync_CreatesUser_ReturnsTokens()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        var request = new RegisterRequest("test@example.com", "Password123!");

        var response = await service.RegisterAsync(request);

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(user);
        Assert.Equal(TiendaMuebles.Domain.Enums.UserRole.Customer, user.Role);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_Throws()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        await service.RegisterAsync(new RegisterRequest("dup@example.com", "Password123!"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync(new RegisterRequest("dup@example.com", "Password123!")));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokens()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        await service.RegisterAsync(new RegisterRequest("login@example.com", "Password123!"));

        var response = await service.LoginAsync(new LoginRequest("login@example.com", "Password123!"));

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_Throws()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        await service.RegisterAsync(new RegisterRequest("login@example.com", "Password123!"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(new LoginRequest("login@example.com", "wrong")));
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        var initial = await service.RegisterAsync(new RegisterRequest("refresh@example.com", "Password123!"));

        var response = await service.RefreshTokenAsync(new RefreshRequest(initial.RefreshToken));

        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.RefreshToken);
        Assert.NotEqual(initial.RefreshToken, response.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_Throws()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.RefreshTokenAsync(new RefreshRequest("invalid-token")));
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUser()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        var initial = await service.RegisterAsync(new RegisterRequest("me@example.com", "Password123!"));
        var user = await db.Users.FirstAsync(u => u.Email == "me@example.com");

        var result = await service.GetCurrentUserAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("me@example.com", result.Email);
    }

    [Fact]
    public async Task PasswordIsHashed_NotPlainText()
    {
        var db = CreateDbContext();
        var service = CreateAuthService(db);
        await service.RegisterAsync(new RegisterRequest("hash@example.com", "Password123!"));

        var user = await db.Users.FirstAsync(u => u.Email == "hash@example.com");
        Assert.NotEqual("Password123!", user.PasswordHash);
        Assert.StartsWith("$2", user.PasswordHash);
    }
}
