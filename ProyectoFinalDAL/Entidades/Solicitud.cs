using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Entidades
{
    public class Solicitud
    {
        public int IdSolicitud {  get; set; }
        public int IdCliente { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "Registrado";
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public string? DocumentoPath { get; set; }

        public Cliente? Cliente { get; set; }

    }
}
