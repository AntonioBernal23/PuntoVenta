using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVenta.Models
{
    public class ProductoVenta
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string Proveedor { get; set; }

        // Nueva propiedad ProductoID
        public int ProductoID { get; set; }

        // Propiedad solo lectura para Subtotal, pero con un método para recalcular
        public decimal Subtotal => Cantidad * Precio;

        // Constructor
        public ProductoVenta(string nombre, decimal precio, int cantidad, string descripcion, string proveedor, int productoID)
        {
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
            Descripcion = descripcion;
            Proveedor = proveedor;
            ProductoID = productoID;  // Asigna el ProductoID
        }

        // Método para actualizar la cantidad (recalcular el subtotal al cambiar la cantidad)
        public void ActualizarSubtotal()
        {
            // El Subtotal se recalcula automáticamente con la propiedad 'Subtotal', no es necesario hacer nada aquí.
        }
    }
}
