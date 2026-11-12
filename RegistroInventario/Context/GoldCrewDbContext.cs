using GoldCrew.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static GoldCrew.Data.Models.Usuario;

namespace GoldCrew.Data;
public class GoldCrewDbContext : DbContext
{
    public GoldCrewDbContext(DbContextOptions<GoldCrewDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ReporteIncidente> ReportesIncidente => Set<ReporteIncidente>();
    public DbSet<InformeBaja> InformesBaja => Set<InformeBaja>();
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
    public DbSet<OrdenCompraDetalle> OrdenesCompraDetalle => Set<OrdenCompraDetalle>();
    public DbSet<NotaDespacho> NotasDespacho => Set<NotaDespacho>();
    public DbSet<NotaDespachoDetalle> NotasDespachoDetalle => Set<NotaDespachoDetalle>();
    public DbSet<MovimientoInventario> Movimientos => Set<MovimientoInventario>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Usuario>().HasIndex(x => x.NombreUsuario).IsUnique();
        mb.Entity<Categoria>().HasIndex(x => x.Nombre).IsUnique();
        mb.Entity<Proveedor>().HasIndex(x => x.Rnc).IsUnique();
        mb.Entity<Ubicacion>().HasIndex(x => x.Codigo).IsUnique();
        mb.Entity<Producto>().HasIndex(x => x.Codigo).IsUnique();
        mb.Entity<OrdenCompra>().HasIndex(x => x.Numero).IsUnique();
        mb.Entity<NotaDespacho>().HasIndex(x => x.Numero).IsUnique();

        mb.Entity<Producto>().Property(x => x.PrecioCompra).HasPrecision(18, 2);
        mb.Entity<Producto>().Property(x => x.PrecioVenta).HasPrecision(18, 2);
        mb.Entity<OrdenCompraDetalle>().Property(x => x.PrecioUnitario).HasPrecision(18, 2);

        mb.Entity<Usuario>().Property(x => x.Rol).HasConversion<string>().HasMaxLength(20);
        mb.Entity<Producto>().Property(x => x.UnidadMedida).HasConversion<string>().HasMaxLength(20);
        mb.Entity<OrdenCompra>().Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
        mb.Entity<InformeBaja>().Property(x => x.Motivo).HasConversion<string>().HasMaxLength(20);
        mb.Entity<MovimientoInventario>().Property(x => x.Tipo).HasConversion<string>().HasMaxLength(20);

        mb.Entity<OrdenCompra>().HasMany(x => x.Detalles).WithOne(d => d.OrdenCompra)
            .HasForeignKey(d => d.OrdenCompraId).OnDelete(DeleteBehavior.Cascade);
        mb.Entity<NotaDespacho>().HasMany(x => x.Detalles).WithOne(d => d.NotaDespacho)
            .HasForeignKey(d => d.NotaDespachoId).OnDelete(DeleteBehavior.Cascade);

        foreach (var fk in mb.Model.GetEntityTypes()
                    .SelectMany(e => e.GetForeignKeys())
                    .Where(f => !f.IsOwnership && f.DeleteBehavior == DeleteBehavior.ClientSetNull
                                || f.DeleteBehavior == DeleteBehavior.Cascade
                                    && f.PrincipalEntityType.ClrType != typeof(OrdenCompra)
                                    && f.PrincipalEntityType.ClrType != typeof(NotaDespacho)))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}