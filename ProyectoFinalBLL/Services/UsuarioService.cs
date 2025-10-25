using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(u => MapToDto(u)).ToList();
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario is null ? null : MapToDto(usuario);
        }

        public async Task<UsuarioDto?> GetByCorreoAsync(string correo)
        {
            var usuario = await _usuarioRepository.GetByCorreoAsync(correo);
            return usuario is null ? null : MapToDto(usuario);
        }

        public async Task CreateAsync(UsuarioDto dto)
        {
            var exist = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
            if (exist != null) throw new InvalidOperationException("El correo ya existe.");

            var entity = MapToEntity(dto);
            await _usuarioRepository.AddAsync(entity);
            await _usuarioRepository.SaveChangesAsync();

            dto.IdUsuario = entity.IdUsuario;
        }

        public async Task UpdateAsync(UsuarioDto dto)
        {
            var entity = await _usuarioRepository.GetByIdAsync(dto.IdUsuario);
            if (entity is null) throw new KeyNotFoundException();

            if (entity.Correo != dto.Correo)
            {
                var exist = await _usuarioRepository.GetByCorreoAsync(dto.Correo);
                if (exist != null) throw new InvalidOperationException("El correo ya existe.");
            }

            entity.NombreCompleto = dto.NombreCompleto;
            entity.Correo = dto.Correo;
            entity.Clave = dto.Clave;
            entity.Rol = dto.Rol;

            await _usuarioRepository.UpdateAsync(entity);
            await _usuarioRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _usuarioRepository.DeleteAsync(id);
            await _usuarioRepository.SaveChangesAsync();
        }

        private UsuarioDto MapToDto(Usuario u) => new UsuarioDto
        {
            IdUsuario = u.IdUsuario,
            NombreCompleto = u.NombreCompleto,
            Correo = u.Correo,
            Clave = u.Clave,
            Rol = u.Rol
        };

        private Usuario MapToEntity(UsuarioDto dto) => new Usuario
        {
            IdUsuario = dto.IdUsuario,
            NombreCompleto = dto.NombreCompleto,
            Correo = dto.Correo,
            Clave = dto.Clave,
            Rol = dto.Rol
        };
    }
}
