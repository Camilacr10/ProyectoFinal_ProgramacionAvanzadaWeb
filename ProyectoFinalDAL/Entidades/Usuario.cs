using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; } // <--- esta es la clave

        [Required, StringLength(150)]
        public string Nombre { get; set; } = "";

        [Required, StringLength(150)]
        public string Apellido { get; set; } = "";

        [Required, EmailAddress, StringLength(100)]
        public string Correo { get; set; } = "";

        [Required, StringLength(50)]
        public string Rol { get; set; } = "";
    }
}