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
        //public static string ConnectionString { get; } = "Server=databasepoe.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=puntoventa;Uid=admin;Pwd=POE$2024;";
        public static string ConnectionString { get; } = "Server=LocalHost;Database=puntoventa;Uid=root;Pwd=1234;";
    }
}
