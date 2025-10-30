using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalDAL.Entidades;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoFinalBLL.Mapeos
{
    public class MapeoClases : Profile
    {
        public MapeoClases()
        {
            // Tracking <-> TrackingDto
            CreateMap<SolicitudTracking, TrackingDto>().ReverseMap();

            CreateMap<Cliente, ClienteDto>().ReverseMap();
        }
    }
}