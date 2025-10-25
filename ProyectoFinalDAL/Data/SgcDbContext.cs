using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Data
{
    public class SgcDbContext : DbContext
    {
        public SgcDbContext(DbContextOptions<SgcDbContext> options) : base(options) { }

        // 🔹 DbSets de entidades DAL
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
        public DbSet<Usuario> Usuarios => Set<Usuario>(); // Usuario DAL

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Configuración Cliente
            modelBuilder.Entity<Cliente>(e =>
            {
                e.HasKey(c => c.IdCliente);
                e.HasIndex(c => c.Identificacion).IsUnique();
                e.Property(c => c.Nombre).HasMaxLength(150).IsRequired();
                e.Property(c => c.Identificacion).HasMaxLength(50).IsRequired();
                e.Property(c => c.Telefono).HasMaxLength(20);
                e.Property(c => c.Email).HasMaxLength(100);
                e.Property(c => c.Direccion).HasMaxLength(255);
            });

            // 🔹 Configuración Solicitud
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

            // 🔹 Configuración Usuario
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(u => u.IdUsuario);

                e.Property(u => u.IdUsuario)
                 .UseIdentityColumn()
                 .ValueGeneratedOnAdd();

                e.Property(u => u.NombreCompleto).HasMaxLength(150).IsRequired();
                e.Property(u => u.Correo).HasMaxLength(100).IsRequired();
                e.Property(u => u.Clave).HasMaxLength(255).IsRequired();
                e.Property(u => u.Rol).HasMaxLength(50).IsRequired();

                e.ToTable("Usuario");

                // 🔹 Usuario Admin predeterminado
                e.HasData(new Usuario
                {
                    IdUsuario = 1,
                    NombreCompleto = "Administrador",
                    Correo = "admin@sgc.com",
                    Clave = "admin", // ⚠️ Para producción usar hash
                    Rol = "Admin"
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
