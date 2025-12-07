using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Listado de usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var model = new List<UsuarioViewModel>();

            foreach (var u in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(u);
                model.Add(new UsuarioViewModel
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Email = u.Email,
                    Roles = roles
                });
            }

            return View(model);
        }

        // Crear usuario
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }

            if (model.Roles != null)
            {
                await _userManager.AddToRolesAsync(user, model.Roles);
            }

            return RedirectToAction(nameof(Index));
        }

        // Editar usuario
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUsuarioViewModel
            {
                Id = user.Id,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // --------------------------------------
            // VALIDACIÓN DE CORREO DUPLICADO
            // --------------------------------------
            var emailExistente = await _userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null && emailExistente.Id != model.Id)
            {
                ModelState.AddModelError("Email", "El correo ingresado ya está en uso.");
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }
            // --------------------------------------

            user.NombreCompleto = model.NombreCompleto;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (model.Roles != null)
                await _userManager.AddToRolesAsync(user, model.Roles);

            return RedirectToAction(nameof(Index));
        }

        // Eliminar usuario
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(new UsuarioViewModel
            {
                Id = user.Id,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // ----------------------------
        // MODALES PARA AJAX
        // ----------------------------

        // GET: Create Modal
        public IActionResult CreateModal()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return PartialView("_CreateModalUsuario", new CreateUsuarioViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateModal(CreateUsuarioViewModel model)
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            if (!ModelState.IsValid)
                return PartialView("_CreateModalUsuario", model);

            // ------------------------------------------------
            // VALIDACIÓN DE CONTRASEÑA (SOLO AÑADIDO)
            // ------------------------------------------------
            var password = model.Password;

            bool tieneMayuscula = password.Any(char.IsUpper);
            bool tieneNumero = password.Any(char.IsDigit);
            bool tieneSimbolo = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!tieneMayuscula || !tieneNumero || !tieneSimbolo)
            {
                ModelState.AddModelError("Password",
                    "La contraseña debe incluir al menos: Una mayúscula, un número y un símbolo especial.");

                return PartialView("_CreateModalUsuario", model);
            }
            // ------------------------------------------------


            // ----------------------------
            // VALIDACIÓN DE CORREO DUPLICADO
            // ----------------------------
            var emailExistente = await _userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null)
            {
                ModelState.AddModelError("Email", "El correo ingresado ya está en uso.");
                return PartialView("_CreateModalUsuario", model);
            }
            // ----------------------------

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                return PartialView("_CreateModalUsuario", model);
            }

            if (model.Roles != null)
                await _userManager.AddToRolesAsync(user, model.Roles);

            return Json(new { success = true });
        }


        // GET: Edit Modal
        public async Task<IActionResult> EditModal(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUsuarioViewModel
            {
                Id = user.Id,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return PartialView("_EditModalUsuario", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditModal(EditUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return PartialView("_EditModalUsuario", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // --------------------------------------
            // VALIDACIÓN DE CORREO DUPLICADO (MODAL)
            // --------------------------------------
            var emailExistente = await _userManager.FindByEmailAsync(model.Email);
            if (emailExistente != null && emailExistente.Id != model.Id)
            {
                ModelState.AddModelError("Email", "El correo ingresado ya está en uso.");
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return PartialView("_EditModalUsuario", model);
            }
            // --------------------------------------

            user.NombreCompleto = model.NombreCompleto;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return PartialView("_EditModalUsuario", model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (model.Roles != null)
                await _userManager.AddToRolesAsync(user, model.Roles);

            return Json(new { success = true });
        }

        // GET: Delete Modal
        public async Task<IActionResult> DeleteModal(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UsuarioViewModel
            {
                Id = user.Id,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return PartialView("_DeleteModalUsuario", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModalConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return Json(new { success = true });
        }
    }

    // ViewModels
    public class UsuarioViewModel
    {
        public string Id { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Email { get; set; } = "";
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class CreateUsuarioViewModel
    {
        public string NombreCompleto { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public IList<string>? Roles { get; set; }
    }

    public class EditUsuarioViewModel
    {
        public string Id { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Email { get; set; } = "";
        public IList<string>? Roles { get; set; }
    }
}
