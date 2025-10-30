using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Repositories;

namespace ProyectoFinalBLL.Services
{
    public class TrackingServicio : ITrackingServicio
    {
        // Repositorio de tracking (acceso a base de datos)
        private readonly ITrackingRepositorio _repo;

        // Mapper para convertir entre entidades y DTOs
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias necesarias
        public TrackingServicio(ITrackingRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // ======================= GUARDAR TRACKING =======================
        // Guarda un registro de seguimiento (tracking) de una solicitud.
        // Se registra la acción realizada, estado, comentario y usuario que la hizo.
        public async Task<CustomResponse<object>> GuardarAsync(int idSolicitud, string estado, string accion, string? comentario, string userId)
        {
            var r = new CustomResponse<object>();

            // Crear nuevo registro de seguimiento
            var item = new ProyectoFinalDAL.Entidades.SolicitudTracking
            {
                IdSolicitud = idSolicitud,
                Estado = estado,
                Accion = accion,
                Comentario = comentario,
                UsuarioId = userId
            };

            // Guardar en base de datos
            var ok = await _repo.AgregarAsync(item);

            // Si algo falla, devolver error
            if (!ok)
            {
                r.EsError = true;
                r.Mensaje = "No se pudo guardar el tracking";
                return r;
            }

            // Si todo sale bien, retorna sin errores
            return r;
        }

        // ======================= LISTAR TRACKING =======================
        // Retorna la lista de acciones (tracking) asociadas a una solicitud específica.
        public async Task<CustomResponse<List<TrackingDto>>> ListarAsync(int idSolicitud)
        {
            var r = new CustomResponse<List<TrackingDto>>();

            // Obtener los registros desde el repositorio
            var list = await _repo.ListarPorSolicitudAsync(idSolicitud);

            // Convertir las entidades a DTOs para enviarlas al frontend
            r.Data = _mapper.Map<List<TrackingDto>>(list);

            return r;
        }
    }
}