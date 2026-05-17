using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Infrastructure.Data.Config;

public class PedidoConfig : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> b)
    {
        b.HasKey(p => p.Id);
        b.HasIndex(p => p.Numero).IsUnique();
        b.Property(p => p.Numero).IsRequired().HasMaxLength(20);
        b.Property(p => p.Email).IsRequired().HasMaxLength(256);
        b.Property(p => p.Subtotal).HasPrecision(10, 2);

        b.HasMany(p => p.Items)
         .WithOne(i => i.Pedido)
         .HasForeignKey(i => i.PedidoId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(p => p.DatosEnvio)
         .WithOne(d => d.Pedido)
         .HasForeignKey<DatosEnvio>(d => d.PedidoId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
