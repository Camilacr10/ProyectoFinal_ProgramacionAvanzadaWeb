using System.Linq; // para char.IsDigit
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
        {
            var entidades = await _repo.GetAllAsync();
            return entidades.Select(MapToDtoLimpio).ToList();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e is null ? null : MapToDtoLimpio(e);
        }

        public async Task<int> CreateAsync(ClienteDto dto)
        {
           
            Preprocesar(dto);

            var ident = NormalizeKey(dto.Identificacion);
            var dup = await _repo.GetByIdentificacionAsync(ident);
            if (dup != null) throw new InvalidOperationException("La identificación ya existe.");

            var entity = MapToEntity(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity.IdCliente;
        }

        public async Task UpdateAsync(ClienteDto dto)
        {
   
            Preprocesar(dto);

            var current = await _repo.GetByIdAsync(dto.IdCliente)
                ?? throw new KeyNotFoundException("Cliente no encontrado.");

        
            var identActual = NormalizeKey(current.Identificacion);
            var identNueva = NormalizeKey(dto.Identificacion);

            if (!string.Equals(identActual, identNueva, StringComparison.Ordinal))
            {
                var dup = await _repo.GetByIdentificacionAsync(identNueva);
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

        // ============================
        //  Mapeo manual
        // ============================

        private static ClienteDto MapToDtoLimpio(Cliente e) => new()
        {
            IdCliente = e.IdCliente,
            Identificacion = TrimOrEmpty(e.Identificacion),
            Nombre = TrimOrEmpty(e.Nombre),
            Apellido = TrimOrEmpty(e.Apellido),
            Telefono = NormalizarTelefono(e.Telefono),
            Email = TrimOrNull(e.Email),
            Direccion = TrimOrNull(e.Direccion)
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

        // ============================
        //  Reglas "custom"
        // ============================

        private static void Preprocesar(ClienteDto dto)
        {
            dto.Identificacion = TrimOrEmpty(dto.Identificacion);
            dto.Nombre = TrimOrEmpty(dto.Nombre);
            dto.Apellido = TrimOrEmpty(dto.Apellido);
            dto.Email = TrimOrNull(dto.Email);
            dto.Direccion = TrimOrNull(dto.Direccion);
            dto.Telefono = NormalizarTelefono(dto.Telefono);
        }

       
        private static string NormalizeKey(string? s) =>
            (s ?? string.Empty).Trim().ToUpperInvariant();

        private static string TrimOrEmpty(string? s) =>
            string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();

        private static string? TrimOrNull(string? s) =>
            string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        
        private static string? NormalizarTelefono(string? tel)
        {
            if (string.IsNullOrWhiteSpace(tel)) return null;
            var digits = new string(tel.Where(char.IsDigit).ToArray());
            if (digits.Length == 8)
                return $"{digits[..4]}-{digits[4..]}";
            return digits;
        }
    }
}
