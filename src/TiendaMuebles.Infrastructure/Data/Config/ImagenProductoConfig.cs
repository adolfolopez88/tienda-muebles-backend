using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data.Config;

public class ImagenProductoConfig : IEntityTypeConfiguration<ImagenProducto>
{
    public void Configure(EntityTypeBuilder<ImagenProducto> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.Url).IsRequired().HasMaxLength(500);
    }
}
