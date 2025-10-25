using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Data;

namespace ProyectoFinal.Controllers
{
    public class AccesoController : Controller
    {
        private readonly SgcDbContext _SgcDbContext;
        public AccesoController(SgcDbContext sgcDbContext)
        {
            _SgcDbContext = sgcDbContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }
    }
}
