using Microsoft.EntityFrameworkCore;
using Practica3Modelo.Models;

namespace Practica3Modelo.Data;

public class ArtesaniasDbContext : DbContext
{
    public ArtesaniasDbContext(DbContextOptions<ArtesaniasDbContext> options) : base(options) { }

    public DbSet<ClienteModel> Clientes => Set<ClienteModel>();
    public DbSet<ProductoModel> Productos => Set<ProductoModel>();
    public DbSet<PedidoModel> Pedidos => Set<PedidoModel>();
    public DbSet<DetallePedidoModel> DetallesPedido => Set<DetallePedidoModel>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<ProductoModel>().Property(p => p.Precio).HasPrecision(10, 2);
        model.Entity<PedidoModel>().Property(p => p.Total).HasPrecision(10, 2);
        model.Entity<DetallePedidoModel>().Property(d => d.PrecioUnitario).HasPrecision(10, 2);

        model.Entity<DetallePedidoModel>()
            .HasOne(d => d.Pedido).WithMany(p => p.Detalles)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        model.Entity<DetallePedidoModel>()
            .HasOne(d => d.Producto).WithMany(p => p.Detalles)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
