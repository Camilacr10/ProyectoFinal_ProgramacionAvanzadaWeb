using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalBLL.DTOs
{
    public class TrackingDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Solicitud es obligatoria.")]
        public int IdSolicitud { get; set; }

        [Required(ErrorMessage = "Estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "Estado no debe exceder 50 caracteres.")]
        public string Estado { get; set; } = "";

        [Required(ErrorMessage = "Acción es obligatoria.")]
        [StringLength(100, ErrorMessage = "Acción no debe exceder 100 caracteres.")]
        public string Accion { get; set; } = "";

        [StringLength(500, ErrorMessage = "Comentario no debe exceder 500 caracteres.")]
        public string? Comentario { get; set; }

        [Required(ErrorMessage = "Usuario es obligatorio.")]
        [StringLength(450, ErrorMessage = "UsuarioId no debe exceder 450 caracteres.")]
        public string UsuarioId { get; set; } = "";

        public DateTime Fecha { get; set; }
    }
}