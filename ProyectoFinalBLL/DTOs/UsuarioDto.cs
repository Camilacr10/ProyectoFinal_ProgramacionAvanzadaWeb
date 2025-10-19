using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalBLL.Dtos
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }

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

