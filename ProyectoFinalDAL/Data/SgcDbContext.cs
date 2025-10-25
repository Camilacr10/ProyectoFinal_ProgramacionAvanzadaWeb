using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;

public class SgcDbContext : IdentityDbContext<ApplicationUser>
{
    public SgcDbContext(DbContextOptions<SgcDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuración Cliente
        builder.Entity<Cliente>(e =>
        {
            e.HasKey(c => c.IdCliente);
            e.HasIndex(c => c.Identificacion).IsUnique();
            e.Property(c => c.Nombre).HasMaxLength(150).IsRequired();
            e.Property(c => c.Identificacion).HasMaxLength(50).IsRequired();
        });

        // Configuración Solicitud
        builder.Entity<Solicitud>(e =>
        {
            e.HasKey(s => s.IdSolicitud);
            e.HasOne(s => s.Cliente)
              .WithMany()
              .HasForeignKey(s => s.IdCliente)
              .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
