using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalDAL.Entidades
{
    public class Cliente
    {
        public int IdCliente { get; set; }            
        public string Identificacion { get; set; } = ""; 
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string? Telefono { get; set; }           
        public string? Email { get; set; }              
        public string? Direccion { get; set; }

        // Navegación
        public List<Solicitud> Solicitudes { get; set; } = new();

    }
}
