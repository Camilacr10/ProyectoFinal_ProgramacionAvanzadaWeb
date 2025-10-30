using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Data
{
    // Hereda de IdentityDbContext para incluir las tablas de Identity
    public class SgcDbContext : IdentityDbContext<ApplicationUser>
    {
        public SgcDbContext(DbContextOptions<SgcDbContext> options)
            : base(options)
        {
        }

        // Entidades personalizadas
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<SolicitudTracking> SolicitudTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //  NECESARIO para Identity

            // Configuración Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.IdCliente);
                entity.Property(c => c.Nombre)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(c => c.Identificacion)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(c => c.Identificacion).IsUnique();
            });

            // Configuración Solicitud
            modelBuilder.Entity<Solicitud>(entity =>
            {
                entity.HasKey(s => s.IdSolicitud);

                entity.Property(s => s.Estado)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(s => s.Monto)                     // Agregado
                      .HasColumnType("decimal(18,2)");            // Configura precisión y escala

                entity.HasOne(s => s.Cliente)
                      .WithMany()
                      .HasForeignKey(s => s.IdCliente)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Tracking
            modelBuilder.Entity<SolicitudTracking>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Estado).HasMaxLength(40);
                entity.Property(t => t.Accion).HasMaxLength(80);
                entity.Property(t => t.UsuarioId).HasMaxLength(450);

                entity.HasOne(t => t.Solicitud)
                 .WithMany(t => t.Trackings)
                 .HasForeignKey(t => t.IdSolicitud)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
