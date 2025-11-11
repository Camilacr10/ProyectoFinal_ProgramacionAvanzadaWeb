using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalBLL.DTOs
{
    // Modelo para cambio de estado
    public class CambioEstadoDto
    {
        public int IdSolicitud { get; set; }
        public string NuevoEstado { get; set; } = "";
        public string? Comentario { get; set; }
    }
}