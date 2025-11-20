using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;


        //Necesario para el tracking

        private readonly ITrackingServicio _tracking; // Servicio de tracking
        private readonly IHttpContextAccessor _httpContextAccessor; // Acceso al contexto HTTP para obtener el usuario logeado

        public SolicitudService(ISolicitudRepository repo, ITrackingServicio tracking, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _tracking = tracking;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<SolicitudDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(MapToDto).ToList();

        public async Task<SolicitudDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e is null ? null : MapToDto(e);
        }

        // Reglas de negocio
        public async Task<int> CreateAsync(SolicitudDto dto)
        {
            if (dto.Monto > 10_000_000)
                throw new InvalidOperationException("El monto no se puede superar de más de 10.000.000 colones ");

            if (await _repo.SolicitudConflictiva(dto.IdCliente))
                throw new InvalidOperationException("Este cliente ya posee una solicitud en estado de 'Registrado' o 'Devolución'");

            var entity = MapToEntity(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();



            // ================================
            // Registrar el tracking de creación
            // ================================

            try
            {
                // Se obtiene el usuario logueado

                var user = _httpContextAccessor.HttpContext?.User; // Usuario logueado

                // Obtener el identificador del usuario
                var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user?.Identity?.Name
                             ?? "sistema";

                // Guarda el movimiento
                await _tracking.GuardarAsync(
                    entity.IdSolicitud,
                    dto.Estado ?? "Registrado",
                    "Crear",
                    dto.Comentarios,
                    userId
                );
            }
            catch
            {
                // Si el tracking falla no afecta la creación
            }



            return entity.IdSolicitud;
        }


        public async Task<int> CreateAsync(SolicitudDto dto, IFormFile? documento, string webRootPath)
        {
            if (documento != null && documento.Length > 0)
            {
                var uploadsRoot = Path.Combine(webRootPath, "uploads", "solicitudes");
                Directory.CreateDirectory(uploadsRoot);

                var safeName = Path.GetFileName(documento.FileName);
                var fileName = $"{Guid.NewGuid()}_{safeName}";
                var fullPath = Path.Combine(uploadsRoot, fileName);

                using (var stream = File.Create(fullPath))
                {
                    await documento.CopyToAsync(stream);
                }

                dto.DocumentoPath = $"/uploads/solicitudes/{fileName}";
            }

            return await CreateAsync(dto);
        }

        public async Task UpdateAsync(SolicitudDto dto)
        {
            var current = await _repo.GetByIdAsync(dto.IdSolicitud)
                ?? throw new KeyNotFoundException("La Solicitud no fue encontrada");

            current.Monto = dto.Monto;
            current.Estado = dto.Estado;
            current.DocumentoPath = dto.DocumentoPath;
            current.Comentarios = dto.Comentarios;
            await _repo.UpdateAsync(current);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var current = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("La solicitud no existe o ya fue eliminada.");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        public Task<bool> SolicitudConflictiva(int idCliente)
            => _repo.SolicitudConflictiva(idCliente);

        private static SolicitudDto MapToDto(Solicitud e) => new()
        {
            IdSolicitud = e.IdSolicitud,
            IdCliente = e.IdCliente,
            Monto = e.Monto,
            Estado = e.Estado,
            FechaSolicitud = e.FechaSolicitud,
            DocumentoPath = e.DocumentoPath,
            Comentarios = e.Comentarios,
            Cliente = e.Cliente
        };

        private static Solicitud MapToEntity(SolicitudDto d) => new()
        {
            IdSolicitud = d.IdSolicitud,
            IdCliente = d.IdCliente,
            Monto = d.Monto,
            Estado = d.Estado,
            FechaSolicitud = d.FechaSolicitud,
            DocumentoPath = d.DocumentoPath,
            Comentarios = d.Comentarios
        };
    }
}
