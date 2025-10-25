using Microsoft.AspNetCore.Identity;

namespace ProyectoFinalDAL.Entidades
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
    }
}
