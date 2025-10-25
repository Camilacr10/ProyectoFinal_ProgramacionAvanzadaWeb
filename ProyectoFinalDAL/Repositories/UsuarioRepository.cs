using Microsoft.AspNetCore.Identity;
using ProyectoFinalDAL.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Repositories
{
    public class UsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Obtener todos los usuarios
        public List<ApplicationUser> GetAll()
        {
            return _userManager.Users.OrderBy(u => u.NombreCompleto).ToList();
        }

        // Obtener usuario por Id
        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        // Obtener usuario por correo
        public async Task<ApplicationUser?> GetByCorreoAsync(string correo)
        {
            return await _userManager.FindByEmailAsync(correo);
        }

        // Crear usuario con contraseña y rol
        public async Task<IdentityResult> AddAsync(ApplicationUser usuario, string password, string rol)
        {
            var result = await _userManager.CreateAsync(usuario, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(rol))
                {
                    await _roleManager.CreateAsync(new IdentityRole(rol));
                }
                await _userManager.AddToRoleAsync(usuario, rol);
            }
            return result;
        }

        // Actualizar usuario
        public async Task<IdentityResult> UpdateAsync(ApplicationUser usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        // Eliminar usuario
        public async Task<IdentityResult> DeleteAsync(ApplicationUser usuario)
        {
            return await _userManager.DeleteAsync(usuario);
        }

        // Obtener roles de un usuario
        public async Task<IList<string>> GetRolesAsync(ApplicationUser usuario)
        {
            return await _userManager.GetRolesAsync(usuario);
        }

        // Asignar un rol a un usuario
        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser usuario, string rol)
        {
            if (!await _roleManager.RoleExistsAsync(rol))
            {
                await _roleManager.CreateAsync(new IdentityRole(rol));
            }
            return await _userManager.AddToRoleAsync(usuario, rol);
        }

        // Remover un rol de un usuario
        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser usuario, string rol)
        {
            return await _userManager.RemoveFromRoleAsync(usuario, rol);
        }
    }
}
