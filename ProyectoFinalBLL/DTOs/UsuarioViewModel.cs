using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalBLL.DTOs
{
    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string Role { get; set; } = string.Empty;
    }
}
