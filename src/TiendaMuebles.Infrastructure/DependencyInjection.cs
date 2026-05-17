using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.Auth;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Infrastructure.Auth;
using TiendaMuebles.Infrastructure.Data;
using TiendaMuebles.Infrastructure.Services;

namespace TiendaMuebles.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
        services.AddScoped<PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IPedidoService, PedidoService>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();
        services.AddScoped<IPaymentService, SimulatedPaymentService>();
        services.AddScoped<IEmailService, LoggingEmailService>();

        return services;
    }
}
