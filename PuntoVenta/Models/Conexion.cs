using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVenta.Models
{
    internal class Conexion
    {
        // Definir la variable de conexión como pública y estática
        public static string ConnectionString { get; } = "Server=10.226.160.104;Database=PuntoVenta;Uid=tony;Pwd=1234;";
    }
}
