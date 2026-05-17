using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data.Config;

public class CategoriaConfig : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> b)
    {
        b.HasKey(c => c.Id);
        b.HasIndex(c => c.Slug).IsUnique();
        b.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        b.Property(c => c.Slug).IsRequired().HasMaxLength(100);
    }
}
