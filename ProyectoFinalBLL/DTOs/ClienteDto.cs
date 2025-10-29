using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalBLL.DTOs
{
    public class ClienteDto
    {
        public int IdCliente { get; set; }

        [Required, StringLength(50)]
        public string Identificacion { get; set; } = "";

        [Required, StringLength(150)]
        public string Nombre { get; set; } = "";

        [Required, StringLength(150)]
        public string Apellido { get; set; } = "";

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Direccion { get; set; }
    }
}
