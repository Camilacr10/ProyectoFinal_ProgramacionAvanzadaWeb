using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        public ClienteService(IClienteRepository repo) => _repo = repo;

        public async Task<List<ClienteDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(MapToDto).ToList();

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e is null ? null : MapToDto(e);
        }

        public async Task<int> CreateAsync(ClienteDto dto)
        {
            var dup = await _repo.GetByIdentificacionAsync(dto.Identificacion);
            if (dup != null) throw new InvalidOperationException("La identificación ya existe.");

            var entity = MapToEntity(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity.IdCliente;
        }

        public async Task UpdateAsync(ClienteDto dto)
        {
            var current = await _repo.GetByIdAsync(dto.IdCliente)
                ?? throw new KeyNotFoundException("Cliente no encontrado.");

            if (!string.Equals(current.Identificacion, dto.Identificacion, StringComparison.OrdinalIgnoreCase))
            {
                var dup = await _repo.GetByIdentificacionAsync(dto.Identificacion);
                if (dup != null) throw new InvalidOperationException("La identificación ya existe.");
            }

            current.Identificacion = dto.Identificacion;
            current.Nombre = dto.Nombre;
            current.Apellido = dto.Apellido;
            current.Telefono = dto.Telefono;
            current.Email = dto.Email;
            current.Direccion = dto.Direccion;

            await _repo.UpdateAsync(current);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        private static ClienteDto MapToDto(Cliente e) => new()
        {
            IdCliente = e.IdCliente,
            Identificacion = e.Identificacion,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            Telefono = e.Telefono,
            Email = e.Email,
            Direccion = e.Direccion
        };

        private static Cliente MapToEntity(ClienteDto d) => new()
        {
            IdCliente = d.IdCliente,
            Identificacion = d.Identificacion,
            Nombre = d.Nombre,
            Apellido = d.Apellido,
            Telefono = d.Telefono,
            Email = d.Email,
            Direccion = d.Direccion
        };
    }
}
