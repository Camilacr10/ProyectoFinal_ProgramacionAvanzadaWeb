using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoFinalBLL.DTOs;

namespace ProyectoFinal.Controllers
{
    public IActionResult CreateModal()
    {
        ViewBag.Roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Administrador", Text = "Administrador" },
        new SelectListItem { Value = "Analista", Text = "Analista" },
        new SelectListItem { Value = "Gestor", Text = "Gestor" },
        new SelectListItem { Value = "ServicioAlCliente", Text = "Servicio al Cliente" }
    };

        return PartialView("_CreateModal", new UsuarioViewModel());
    }

}
