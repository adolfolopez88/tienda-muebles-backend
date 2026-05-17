using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data.Config;

public class ProductoConfig : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> b)
    {
        b.HasKey(p => p.Id);
        b.HasIndex(p => p.Slug).IsUnique();
        b.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
        b.Property(p => p.Slug).IsRequired().HasMaxLength(200);
        b.Property(p => p.Precio).HasPrecision(10, 2);

        b.HasOne(p => p.Categoria)
         .WithMany()
         .HasForeignKey(p => p.CategoriaSlug)
         .HasPrincipalKey(c => c.Slug)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Imagenes)
         .WithOne(i => i.Producto)
         .HasForeignKey(i => i.ProductoId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
