using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Data
{
    public class SgcDbContext : DbContext
    {
        public SgcDbContext(DbContextOptions<SgcDbContext> options) : base(options) { }
        public DbSet<Cliente> Clientes => Set<Cliente>();

        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasKey(c => c.IdCliente);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Identificacion)
                .IsUnique();

            modelBuilder.Entity<Cliente>(e =>
            {
                e.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
                e.Property(p => p.Identificacion).HasMaxLength(50).IsRequired();
                e.Property(p => p.Telefono).HasMaxLength(20);
                e.Property(p => p.Email).HasMaxLength(100);
                e.Property(p => p.Direccion).HasMaxLength(255);
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Solicitud>(e =>
            {
                e.HasKey(s => s.IdSolicitud);
                e.Property(s => s.Monto).HasPrecision(18, 2).IsRequired();
                e.Property(s => s.Estado).HasMaxLength(50).IsRequired();
                e.Property(s => s.DocumentoPath).HasMaxLength(255);
                e.HasOne(s => s.Cliente)
                .WithMany()
                .HasForeignKey(s => s.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);       
            });
        }
    }
}
