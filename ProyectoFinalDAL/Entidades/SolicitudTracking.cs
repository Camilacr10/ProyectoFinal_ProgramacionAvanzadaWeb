using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Entidades
{
    public class SolicitudTracking
    {
        public int Id { get; set; }
        public int IdSolicitud { get; set; }
        public string Estado { get; set; } = "";
        public string Accion { get; set; } = "";
        public string? Comentario { get; set; }
        public string UsuarioId { get; set; } = ""; // Id del ApplicationUser que hizo el cambio
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Navegación
        public Solicitud? Solicitud { get; set; }

    }
}