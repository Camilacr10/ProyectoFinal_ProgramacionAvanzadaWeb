using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProyectoFinalDAL.Entidades;
using System;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Data.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 🔹 Roles que queremos en el sistema
            string[] roles = { "Administrador", "Analista", "Gestor", "ServicioAlCliente" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"❌ Error al crear rol '{role}': {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }

            // 🔹 Usuario administrador inicial
            string adminEmail = "admin@sgc.com";
            string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NombreCompleto = "Administrador SGC",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Administrador");
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"❌ Error al asignar rol de administrador: {string.Join(", ", roleResult.Errors)}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Error al crear usuario admin: {string.Join(", ", createResult.Errors)}");
                }
            }

            // 🔹 Opcional: imprimir confirmación
            Console.WriteLine("✅ Seed de roles y usuario administrador completado.");
        }
    }
}
