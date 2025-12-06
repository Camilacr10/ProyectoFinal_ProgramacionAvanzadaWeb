using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalBLL.DTOs
{
    public class CreateUsuarioViewModel
    {
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = string.Empty;
    }
}
