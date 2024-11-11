namespace PuntoVenta.Pages { 
    public class Producto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string Descripcion { get; set; }
        public string Proveedor { get; set; }

        // Constructor
        public Producto(string nombre, decimal precio, int cantidad, string descripcion, string proveedor)
        {
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
            Descripcion = descripcion;
            Proveedor = proveedor;
        }
    }
}
