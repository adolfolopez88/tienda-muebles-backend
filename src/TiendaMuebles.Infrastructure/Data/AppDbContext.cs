using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<ImagenProducto> ImagenProductos => Set<ImagenProducto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

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

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Email = "admin@tiendamuebles.com",
            PasswordHash = "$2a$12$sbwJNiELotlcqbpeF/gVgebq9SIRnIWdZlybAk2Sd6i8wYXqtl.9G",
            Role = TiendaMuebles.Domain.Enums.UserRole.Admin,
            CreatedAt = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
