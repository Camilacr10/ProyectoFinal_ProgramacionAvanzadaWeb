using ProyectoFinalBLL.Dtos;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(MapToDto).ToList();

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(UsuarioDto dto)
        {
            // Verificar duplicado por correo
            var existing = await _repo.GetByCorreoAsync(dto.Correo);
            if (existing != null)
                throw new InvalidOperationException("El correo ya está registrado.");

            var entity = MapToEntity(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return entity.IdUsuario;
        }

        public async Task UpdateAsync(UsuarioDto dto)
        {
            var current = await _repo.GetByIdAsync(dto.IdUsuario)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            // Validar correo duplicado si cambia
            if (!string.Equals(current.Correo, dto.Correo, StringComparison.OrdinalIgnoreCase))
            {
                var dup = await _repo.GetByCorreoAsync(dto.Correo);
                if (dup != null)
                    throw new InvalidOperationException("El correo ya está registrado.");
            }

            current.Nombre = dto.Nombre;
            current.Apellido = dto.Apellido;
            current.Correo = dto.Correo;
            current.Rol = dto.Rol;

            await _repo.UpdateAsync(current);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        private static UsuarioDto MapToDto(Usuario e) => new()
        {
            IdUsuario = e.IdUsuario,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            Correo = e.Correo,
            Rol = e.Rol
        };

        private static Usuario MapToEntity(UsuarioDto d) => new()
        {
            IdUsuario = d.IdUsuario,
            Nombre = d.Nombre,
            Apellido = d.Apellido,
            Correo = d.Correo,
            Rol = d.Rol
        };
    }
}
