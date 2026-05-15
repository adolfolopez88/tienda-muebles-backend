# API Base + Auth JWT — Plan de Implementación

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Crear backend .NET 10 con Clean Architecture, autenticación JWT + refresh tokens, roles Admin/Customer.

**Architecture:** 4 capas (Api → Application → Domain ← Infrastructure). JWT stateless con refresh tokens en DB. BCrypt para passwords. xUnit para tests.

**Tech Stack:** .NET 10, Entity Framework Core 10, SQL Server, BCrypt.Net-Next, System.IdentityModel.Tokens.Jwt, xUnit, Moq

---

## File Map

| File | Responsibility |
|------|---------------|
| `src/TiendaMuebles.Domain/Entities/User.cs` | Entidad de usuario |
| `src/TiendaMuebles.Domain/Entities/RefreshToken.cs` | Entidad de refresh token |
| `src/TiendaMuebles.Domain/Enums/UserRole.cs` | Enum Admin, Customer |
| `src/TiendaMuebles.Application/Auth/IAuthService.cs` | Contrato de auth |
| `src/TiendaMuebles.Application/DTOs/AuthDTOs.cs` | DTOs de request/response |
| `src/TiendaMuebles.Infrastructure/Data/AppDbContext.cs` | EF Core context |
| `src/TiendaMuebles.Infrastructure/Auth/AuthService.cs` | Implementación JWT |
| `src/TiendaMuebles.Infrastructure/Auth/PasswordHasher.cs` | BCrypt wrapper |
| `src/TiendaMuebles.Infrastructure/Auth/JwtConfig.cs` | Config de JWT |
| `src/TiendaMuebles.Infrastructure/DependencyInjection.cs` | Extensión DI |
| `src/TiendaMuebles.Api/Program.cs` | Entry point, DI, middleware |
| `src/TiendaMuebles.Api/Controllers/HealthController.cs` | Health check |
| `src/TiendaMuebles.Api/Controllers/AuthController.cs` | Endpoints auth |
| `src/TiendaMuebles.Api/appsettings.json` | Configuración |
| `tests/TiendaMuebles.Api.Tests/AuthServiceTests.cs` | Tests unitarios |
| `tests/TiendaMuebles.Api.Tests/AuthControllerTests.cs` | Tests integración |

---

### Task 1: Scaffold de proyecto .NET 10

**Files:**
- Create: `TiendaMuebles.sln`
- Create: `src/TiendaMuebles.Domain/TiendaMuebles.Domain.csproj`
- Create: `src/TiendaMuebles.Application/TiendaMuebles.Application.csproj`
- Create: `src/TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj`
- Create: `src/TiendaMuebles.Api/TiendaMuebles.Api.csproj`
- Create: `tests/TiendaMuebles.Api.Tests/TiendaMuebles.Api.Tests.csproj`

- [ ] **Step 1: Crear solution y proyectos**

Run:
```bash
cd C:/Proyectos/tienda-muebles-backend
dotnet new sln -n TiendaMuebles
dotnet new classlib -n TiendaMuebles.Domain -o src/TiendaMuebles.Domain
dotnet new classlib -n TiendaMuebles.Application -o src/TiendaMuebles.Application
dotnet new classlib -n TiendaMuebles.Infrastructure -o src/TiendaMuebles.Infrastructure
dotnet new webapi -n TiendaMuebles.Api -o src/TiendaMuebles.Api --no-https
dotnet new xunit -n TiendaMuebles.Api.Tests -o tests/TiendaMuebles.Api.Tests
```

- [ ] **Step 2: Agregar proyectos a la solución**

Run:
```bash
dotnet sln add src/TiendaMuebles.Domain/TiendaMuebles.Domain.csproj
dotnet sln add src/TiendaMuebles.Application/TiendaMuebles.Application.csproj
dotnet sln add src/TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj
dotnet sln add src/TiendaMuebles.Api/TiendaMuebles.Api.csproj
dotnet sln add tests/TiendaMuebles.Api.Tests/TiendaMuebles.Api.Tests.csproj
```

- [ ] **Step 3: Configurar referencias entre proyectos**

Run:
```bash
cd src/TiendaMuebles.Application
dotnet add reference ../TiendaMuebles.Domain/TiendaMuebles.Domain.csproj
cd ../TiendaMuebles.Infrastructure
dotnet add reference ../TiendaMuebles.Application/TiendaMuebles.Application.csproj
cd ../TiendaMuebles.Api
dotnet add reference ../TiendaMuebles.Application/TiendaMuebles.Application.csproj
dotnet add reference ../TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj
cd ../../tests/TiendaMuebles.Api.Tests
dotnet add reference ../../src/TiendaMuebles.Api/TiendaMuebles.Api.csproj
dotnet add reference ../../src/TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj
cd ../..
```

- [ ] **Step 4: Instalar paquetes NuGet**

Run:
```bash
cd src/TiendaMuebles.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
cd ../../tests/TiendaMuebles.Api.Tests
dotnet add package Moq
cd ../..
```

- [ ] **Step 5: Verificar build**

Run: `dotnet build`
Expected: Build succeeded. 0 Error(s), 0 Warning(s)

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "feat: scaffold proyecto .NET 10 Clean Architecture"
```

---

### Task 2: Domain — User, RefreshToken, UserRole

**Files:**
- Create: `src/TiendaMuebles.Domain/Enums/UserRole.cs`
- Create: `src/TiendaMuebles.Domain/Entities/User.cs`
- Create: `src/TiendaMuebles.Domain/Entities/RefreshToken.cs`

- [ ] **Step 1: Crear enum UserRole**

Write `src/TiendaMuebles.Domain/Enums/UserRole.cs`:
```csharp
namespace TiendaMuebles.Domain.Enums;

public enum UserRole
{
    Customer = 0,
    Admin = 1
}
```

- [ ] **Step 2: Crear entidad User**

Write `src/TiendaMuebles.Domain/Entities/User.cs`:
```csharp
namespace TiendaMuebles.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
```

- [ ] **Step 3: Crear entidad RefreshToken**

Write `src/TiendaMuebles.Domain/Entities/RefreshToken.cs`:
```csharp
namespace TiendaMuebles.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsExpired && !IsRevoked;
}
```

- [ ] **Step 4: Verificar build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 5: Commit**

```bash
git add src/TiendaMuebles.Domain/
git commit -m "feat: agregar entidades User, RefreshToken y enum UserRole"
```

---

### Task 3: Application — IAuthService y DTOs

**Files:**
- Create: `src/TiendaMuebles.Application/Auth/IAuthService.cs`
- Create: `src/TiendaMuebles.Application/DTOs/AuthDTOs.cs`

- [ ] **Step 1: Crear DTOs**

Write `src/TiendaMuebles.Application/DTOs/AuthDTOs.cs`:
```csharp
namespace TiendaMuebles.Application.DTOs;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken);
public record UserResponse(Guid Id, string Email, string Role);
```

- [ ] **Step 2: Crear IAuthService**

Write `src/TiendaMuebles.Application/Auth/IAuthService.cs`:
```csharp
using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshRequest request);
    Task<UserResponse?> GetCurrentUserAsync(Guid userId);
}
```

- [ ] **Step 3: Verificar build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add src/TiendaMuebles.Application/
git commit -m "feat: agregar IAuthService y DTOs de autenticación"
```

---

### Task 4: Infrastructure — DbContext y configuración JWT

**Files:**
- Create: `src/TiendaMuebles.Infrastructure/Data/AppDbContext.cs`
- Create: `src/TiendaMuebles.Infrastructure/Auth/JwtConfig.cs`
- Modify: `src/TiendaMuebles.Api/appsettings.json`

- [ ] **Step 1: Crear JwtConfig**

Write `src/TiendaMuebles.Infrastructure/Auth/JwtConfig.cs`:
```csharp
namespace TiendaMuebles.Infrastructure.Auth;

public class JwtConfig
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
```

- [ ] **Step 2: Crear AppDbContext**

Write `src/TiendaMuebles.Infrastructure/Data/AppDbContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(rt => rt.Token).IsUnique();
        });
    }
}
```

- [ ] **Step 3: Configurar appsettings.json**

Read `src/TiendaMuebles.Api/appsettings.json`, replace with:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TiendaMuebles;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Secret": "super-secret-key-at-least-32-characters-long!!",
    "Issuer": "TiendaMuebles.Api",
    "Audience": "TiendaMuebles.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

- [ ] **Step 4: Verificar build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "feat: agregar AppDbContext, JwtConfig y configuracion inicial"
```

---

### Task 5: Infrastructure — AuthService, PasswordHasher, DI

**Files:**
- Create: `src/TiendaMuebles.Infrastructure/Auth/PasswordHasher.cs`
- Create: `src/TiendaMuebles.Infrastructure/Auth/AuthService.cs`
- Create: `src/TiendaMuebles.Infrastructure/DependencyInjection.cs`

- [ ] **Step 1: Crear PasswordHasher**

Write `src/TiendaMuebles.Infrastructure/Auth/PasswordHasher.cs`:
```csharp
namespace TiendaMuebles.Infrastructure.Auth;

public class PasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, 12);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

- [ ] **Step 2: Crear AuthService**

Write `src/TiendaMuebles.Infrastructure/Auth/AuthService.cs`:

See complete code block below — AuthService implements IAuthService with:
- RegisterAsync: validates unique email, hashes password, saves user, returns tokens
- LoginAsync: validates credentials, returns tokens
- RefreshTokenAsync: validates refresh token, revokes old, issues new pair
- GetCurrentUserAsync: fetches user by ID
- GenerateTokensAsync: creates JWT + refresh token pair
- GenerateJwt: creates signed JWT with claims (sub, email, role)
- GenerateRefreshToken: creates cryptographically random 64-byte token

- [ ] **Step 2: Crear AuthService**

Write `src/TiendaMuebles.Infrastructure/Auth/AuthService.cs` — implementa IAuthService con RegisterAsync (valida email unico, hashea password, guarda user, retorna tokens), LoginAsync (valida credenciales, retorna tokens), RefreshTokenAsync (valida refresh token, revoca el viejo, emite nuevo par), GetCurrentUserAsync (busca user por ID), GenerateTokensAsync (crea JWT + refresh token pair), GenerateJwt (crea JWT firmado con claims: sub, email, role), GenerateRefreshToken (crea token aleatorio de 64 bytes).

- [ ] **Step 3: Crear DependencyInjection**

Write `src/TiendaMuebles.Infrastructure/DependencyInjection.cs` — metodo AddInfrastructure que registra AppDbContext con SQL Server, configura JwtConfig desde appsettings, y registra PasswordHasher y AuthService como scoped services.

- [ ] **Step 4: Verificar build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 5: Crear migracion inicial**

Run:
```bash
cd src/TiendaMuebles.Api
dotnet ef migrations add InitialCreate --startup-project . --project ../TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj
cd ../..
```

Expected: Build succeeded. Migration files created.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "feat: implementar AuthService, PasswordHasher, DI y migracion inicial"
```

---

### Task 6: Api — Program.cs, HealthController, Swagger

**Files:**
- Create: `src/TiendaMuebles.Api/Controllers/HealthController.cs`
- Modify: `src/TiendaMuebles.Api/Program.cs`

- [ ] **Step 1: Crear HealthController**

Write `src/TiendaMuebles.Api/Controllers/HealthController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
```

- [ ] **Step 2: Configurar Program.cs**

Read and replace `src/TiendaMuebles.Api/Program.cs`:
```csharp
using TiendaMuebles.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
```

- [ ] **Step 3: Verificar build y ejecutar**

Run:
```bash
dotnet build
dotnet run --project src/TiendaMuebles.Api --urls http://localhost:5000
```

Expected: Swagger en http://localhost:5000/swagger, `GET /health` → 200

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat: configurar Program.cs, HealthController y Swagger"
```

---

### Task 7: Api — AuthController

**Files:**
- Create: `src/TiendaMuebles.Api/Controllers/AuthController.cs`

- [ ] **Step 1: Crear AuthController**

Write `src/TiendaMuebles.Api/Controllers/AuthController.cs`:
```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaMuebles.Application.Auth;
using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId);
        return user is null ? NotFound() : Ok(user);
    }
}
```

- [ ] **Step 2: Configurar autenticacion JWT en Program.cs**

Edit `src/TiendaMuebles.Api/Program.cs` — add after `builder.Services.AddInfrastructure(builder.Configuration);`:
```csharp
// Add JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
    System.Text.Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = key
    };
});
```

And after `app.UseSwaggerUI();` add:
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Final Program.cs should be:
```csharp
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TiendaMuebles.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = key
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

- [ ] **Step 3: Verificar build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat: implementar AuthController con register, login, refresh, me"
```

---

### Task 8: Tests — AuthService unit tests

**Files:**
- Create: `tests/TiendaMuebles.Api.Tests/AuthServiceTests.cs`

- [ ] **Step 1: Escribir tests de AuthService**

Write `tests/TiendaMuebles.Api.Tests/AuthServiceTests.cs` — 8 tests con xUnit + Moq + InMemoryDatabase:
1. RegisterAsync_CreatesUser_ReturnsTokens — registra usuario, valida tokens y rol Customer
2. RegisterAsync_DuplicateEmail_Throws — email duplicado lanza InvalidOperationException
3. LoginAsync_ValidCredentials_ReturnsTokens — login con credenciales correctas retorna tokens
4. LoginAsync_InvalidPassword_Throws — password incorrecta lanza UnauthorizedAccessException
5. RefreshTokenAsync_ValidToken_ReturnsNewTokens — refresh valido rota tokens
6. RefreshTokenAsync_InvalidToken_Throws — token invalido lanza UnauthorizedAccessException
7. GetCurrentUserAsync_ReturnsUser — obtener usuario autenticado
8. PasswordIsHashed_NotPlainText — verifica que passwords se hashean con BCrypt

- [ ] **Step 2: Ejecutar tests**

Run: `dotnet test`
Expected: 8 passed, 0 failed

- [ ] **Step 3: Commit**

```bash
git add tests/
git commit -m "test: agregar tests unitarios de AuthService (8 tests)"
```

---

### Task 9: Seed Admin y verificacion de roles

**Files:**
- Modify: `src/TiendaMuebles.Infrastructure/Data/AppDbContext.cs`

- [ ] **Step 1: Agregar seed data de Admin**

Edit `src/TiendaMuebles.Infrastructure/Data/AppDbContext.cs` — add to `OnModelCreating`:
```csharp
modelBuilder.Entity<User>().HasData(new User
{
    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
    Email = "admin@tiendamuebles.com",
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", 12),
    Role = Domain.Enums.UserRole.Admin,
    CreatedAt = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc)
});
```

- [ ] **Step 2: Crear migracion de seed**

Run:
```bash
cd src/TiendaMuebles.Api
dotnet ef migrations add SeedAdminUser --startup-project . --project ../TiendaMuebles.Infrastructure/TiendaMuebles.Infrastructure.csproj
cd ../..
```

- [ ] **Step 3: Verificar build y ejecutar tests**

Run:
```bash
dotnet build
dotnet test
```

Expected: Build succeeded. All tests pass.

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat: seed admin user y migracion de datos iniciales"
```

---

### Task 10: Sincronizar con Vault

- [ ] **Step 1: Verificar archivos en ambas ubicaciones**

Run:
```bash
ls -la docs/specs/2026-05/
ls -la "C:/Proyectos/ObsidianVault/200 🚀 Proyectos/tienda-muebles-backend/docs/specs/2026-05/"
```

Expected: `spec-2026-05-15-api-base-auth.md` exists in both.

- [ ] **Step 2: Actualizar tabla de progreso en la spec**

Edit the spec in BOTH locations — change AC-1 through AC-9 to `[x]` and update progress table status to ✅ with date 2026-05-15.

- [ ] **Step 3: Commit en vault**

```bash
cd "C:/Proyectos/ObsidianVault"
git add "200 🚀 Proyectos/tienda-muebles-backend/"
git commit -m "progress: API base + auth implementados (AC-1 a AC-9 completados)"
```

- [ ] **Step 4: Commit en proyecto**

```bash
cd "C:/Proyectos/tienda-muebles-backend"
git init
git add -A
git commit -m "feat: API base + auth JWT implementados (.NET 10 Clean Architecture)"
```

---

## Dependencies

```
T1 (scaffold) → T2 (domain) → T3 (application) → T4 (dbcontext) → T5 (auth service) → T6 (program) → T7 (auth controller) → T8 (tests) → T9 (seed) → T10 (sync)
```

## Verification Checklist

- [ ] `dotnet build` sin errores
- [ ] `dotnet test` — todos los tests pasan
- [ ] `dotnet run` — Swagger carga en /swagger
- [ ] `GET /health` → 200
- [ ] `POST /api/auth/register` → 200 con JWT + refresh
- [ ] `POST /api/auth/login` → 200 con JWT + refresh
- [ ] `POST /api/auth/refresh` → 200 con nuevo JWT + refresh rotado
- [ ] `GET /api/auth/me` con JWT → 200 con datos del user
- [ ] `GET /api/auth/me` sin JWT → 401
- [ ] Passwords hasheados (BCrypt) en DB
- [ ] Admin seed creado (admin@tiendamuebles.com / Admin123!)
- [ ] Spec sincronizada en vault y proyecto
