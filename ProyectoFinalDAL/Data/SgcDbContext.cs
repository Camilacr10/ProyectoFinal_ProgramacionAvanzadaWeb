using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Data
{
    public class SgcDbContext : DbContext
    {
        public SgcDbContext(DbContextOptions<SgcDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== Clientes =====
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

            // ===== Usuarios =====
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<Usuario>(e =>
            {
                e.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
                e.Property(p => p.Apellido).HasMaxLength(150).IsRequired();
                e.Property(p => p.Correo).HasMaxLength(100).IsRequired();
                e.Property(p => p.Rol).HasMaxLength(50).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
