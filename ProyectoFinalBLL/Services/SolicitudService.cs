using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _repo;
        private readonly IMapper _mapper;
        public SolicitudService(ISolicitudRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SolicitudDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(MapToDto).ToList();

        public async Task<SolicitudDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e is null ? null : MapToDto(e);
        }
        public async Task<int> CreateAsync(SolicitudDto dto)
        {
            if (dto.Monto > 10_000_000)
                throw new InvalidOperationException("El monto no se puede superar de más de 10.000.000 colones ");

            if (await _repo.SolicitudConflictiva(dto.IdCliente))
                throw new InvalidOperationException("Este cliente ya posee una solicitud en estado de 'Registrado' o 'Devolución'");

            var entity = MapToEntity(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity.IdSolicitud;
        }

        public async Task UpdateAsync(SolicitudDto dto)
        {
            var current = await _repo.GetByIdAsync(dto.IdSolicitud)
                ?? throw new KeyNotFoundException("La Solicitud no fue encontrada");

            current.Monto = dto.Monto;
            current.Estado = dto.Estado;
            current.DocumentoPath = dto.DocumentoPath;
            await _repo.UpdateAsync(current);
            await _repo.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
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
            DocumentoPath = e.DocumentoPath
        };

        private static Solicitud MapToEntity(SolicitudDto d) => new()
        {
            IdSolicitud = d.IdSolicitud,
            IdCliente = d.IdCliente,
            Monto = d.Monto,
            Estado = d.Estado,
            FechaSolicitud = d.FechaSolicitud,
            DocumentoPath = d.DocumentoPath
        };

        //Lo necesito para el tracking

        public async Task<CustomResponse<List<SolicitudDto>>> ObtenerTodosAsync()
        {
            var r = new CustomResponse<List<SolicitudDto>>();
            var list = await _repo.ObtenerTodosAsync();
            r.Data = list.Select(_mapper.Map<SolicitudDto>).ToList();
            return r;
        }

        public async Task<CustomResponse<SolicitudDto>> ObtenerPorIdAsync(int id)
        {
            var r = new CustomResponse<SolicitudDto>();
            var s = await _repo.ObtenerPorIdAsync(id);
            if (s == null)
            {
                r.EsError = true;
                r.Mensaje = "Solicitud no encontrada";
                return r;
            }
            r.Data = _mapper.Map<SolicitudDto>(s);
            return r;
        }

        public async Task<CustomResponse<SolicitudDto>> ActualizarAsync(SolicitudDto dto)
        {
            var r = new CustomResponse<SolicitudDto>();

            if (dto.Monto <= 0 || dto.Monto > 10_000_000m)
            {
                r.EsError = true;
                r.Mensaje = "El monto debe ser mayor a 0 y no exceder 10.000.000";
                return r;
            }

            var s = await _repo.ObtenerPorIdAsync(dto.IdSolicitud);
            if (s == null)
            {
                r.EsError = true;
                r.Mensaje = "Solicitud no existe";
                return r;
            }

            s.IdCliente = dto.IdCliente;
            s.Monto = dto.Monto;
            s.Estado = dto.Estado;
            s.Comentarios = dto.Comentarios;
            s.DocumentoPath = dto.DocumentoPath;

            if (!await _repo.ActualizarAsync(s))
            {
                r.EsError = true;
                r.Mensaje = "No se pudo actualizar la solicitud";
                return r;
            }

            r.Data = _mapper.Map<SolicitudDto>(s);
            return r;
        }


    }
}
