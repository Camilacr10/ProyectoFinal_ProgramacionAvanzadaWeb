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
        private readonly ITrackingRepositorio _repo;
        private readonly IMapper _mapper;

        public TrackingServicio(ITrackingRepositorio repo, IMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<CustomResponse<object>> GuardarAsync(int idSolicitud, string estado, string accion, string? comentario, string userId)
        {
            var r = new CustomResponse<object>();

            var item = new ProyectoFinalDAL.Entidades.SolicitudTracking
            {
                IdSolicitud = idSolicitud,
                Estado = estado,
                Accion = accion,
                Comentario = comentario,
                UsuarioId = userId
            };

            var ok = await _repo.AgregarAsync(item);
            if (!ok)
            {
                r.EsError = true;
                r.Mensaje = "No se pudo guardar el tracking";
                return r;
            }

            return r;
        }

        public async Task<CustomResponse<List<TrackingDto>>> ListarAsync(int idSolicitud)
        {
            var r = new CustomResponse<List<TrackingDto>>();
            var list = await _repo.ListarPorSolicitudAsync(idSolicitud);
            r.Data = _mapper.Map<List<TrackingDto>>(list);
            return r;
        }
    }
}