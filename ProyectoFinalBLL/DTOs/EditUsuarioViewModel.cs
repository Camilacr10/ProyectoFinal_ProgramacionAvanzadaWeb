namespace ProyectoFinalBLL.DTOs
{
    public class EditUsuarioViewModel
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string? NuevaClave { get; set; } // Opcional
    }
}
