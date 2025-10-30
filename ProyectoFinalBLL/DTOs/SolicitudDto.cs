using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalBLL.DTOs
{
    public class SolicitudDto
    {
        public int IdSolicitud { get; set; }
        [Required]
        public int IdCliente { get; set; }
        [Required, Range(0.01, 10000000, ErrorMessage = "Este monto debe de ser igual o menor a 10, 000, 000")]
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "Registrado";
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? DocumentoPath { get; set; }
        public Cliente? Cliente { get; set; }
        public string Comentarios { get; set; }


    }
}
