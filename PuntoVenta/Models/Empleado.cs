using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVenta.Models
{
    class Empleado
    {
        public int id { get; set; }
        public required string nombre { get; set; }
        public required string apellidos { get; set; }
        public required string usuario { get; set; }
        public string contraseña { get; set; }
    }
}