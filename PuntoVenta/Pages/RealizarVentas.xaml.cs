using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using PuntoVenta.Models;
using System.Data;
using System.Text;

namespace PuntoVenta.Pages
{
    public partial class RealizarVentas : ContentPage
    {
        // Cadena de conexión a base de datos
        public readonly string _connectionString = Conexion.ConnectionString;

        // Lista de productos vendidos (Para mostrar en ListView)
        public ObservableCollection<ProductoVenta> ProductosEnVenta { get; set; }

        public RealizarVentas()
        {
            InitializeComponent();
            ProductosEnVenta = new ObservableCollection<ProductoVenta>();
            ProductosListView.ItemsSource = ProductosEnVenta;
        }

        // Método para obtener los productos de la base de datos
        private async Task<List<Producto>> ObtenerProductosAsync()
        {
            var productos = new List<Producto>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT nombre, precio, cantidad, descripcion, proveedor FROM inventario WHERE cantidad > 0"; // Filtrar productos con cantidad > 0

                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var producto = new Producto(
                                reader.GetString("nombre"),
                                reader.GetDecimal("precio"),
                                reader.GetInt32("cantidad"),
                                reader.GetString("descripcion"),
                                reader.GetString("proveedor")
                            );

                            productos.Add(producto);
                        }
                    }
                }
            }

            return productos;
        }

        // Método para cargar los productos en el Picker
        private async Task CargarProductos()
        {
            // Verificar si el Picker está inicializado
            if (ProductosPickerPc == null)
            {
                return;
            }

            var productos = await ObtenerProductosAsync();

            if (productos.Count > 0)
            {
                // Limpiar los elementos anteriores y agregar los nuevos productos
                ProductosPickerPc.Items.Clear();

                foreach (var producto in productos)
                {
                    ProductosPickerPc.Items.Add(producto.Nombre); // Agregar el nombre de los productos al Picker
                }
            }
            else
            {
                Console.WriteLine("No hay productos disponibles.");
            }
        }

        // Método que se ejecuta cuando se presiona el botón para agregar el producto
        private async void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            if (ProductosPickerPc.SelectedItem == null)
            {
                await DisplayAlert("Error", "Por favor selecciona un producto.", "OK");
                return;
            }

            var nombreProductoSeleccionado = ProductosPickerPc.SelectedItem.ToString();
            var productos = await ObtenerProductosAsync();
            var productoSeleccionado = productos.FirstOrDefault(p => p.Nombre == nombreProductoSeleccionado);

            if (productoSeleccionado != null)
            {
                var productoEnVenta = new ProductoVenta(
                    productoSeleccionado.Nombre,
                    productoSeleccionado.Precio,
                    1, // Cantidad inicial es 1
                    productoSeleccionado.Descripcion,
                    productoSeleccionado.Proveedor
                );

                // Verificar si el producto ya está en el ListView
                var productoExistente = ProductosEnVenta.FirstOrDefault(p => p.Nombre == productoEnVenta.Nombre);
                if (productoExistente != null)
                {
                    // Si el producto ya existe, solo aumentamos la cantidad
                    productoExistente.Cantidad++;
                    productoExistente.Subtotal = productoExistente.Cantidad * productoExistente.Precio;
                }
                else
                {
                    // Si el producto no existe, lo agregamos al ListView
                    ProductosEnVenta.Add(productoEnVenta);
                }

                // Limpiar el Picker
                ProductosPickerPc.SelectedItem = null;

                // Actualizar el total
                ActualizarTotal();

                // Actualizar la UI
                ProductosListView.ItemsSource = null;
                ProductosListView.ItemsSource = ProductosEnVenta;
            }
        }

        // Método para aumentar la cantidad de un producto
        private async void OnAumentarCantidadClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var productoVenta = (ProductoVenta)button.BindingContext;

            // Obtener el producto actualizado desde la base de datos para verificar la cantidad disponible
            var productos = await ObtenerProductosAsync();
            var productoBD = productos.FirstOrDefault(p => p.Nombre == productoVenta.Nombre);

            if (productoBD != null)
            {
                // Verificar si la cantidad en el carrito es menor que la cantidad disponible en el inventario
                if (productoVenta.Cantidad < productoBD.Cantidad)
                {
                    productoVenta.Cantidad++;
                    productoVenta.Subtotal = productoVenta.Cantidad * productoVenta.Precio;

                    // Actualizar el total
                    ActualizarTotal();

                    // Actualizar la UI
                    ProductosListView.ItemsSource = null;
                    ProductosListView.ItemsSource = ProductosEnVenta;
                }
                else
                {
                    DisplayAlert("Error", "No puedes agregar más cantidad de este producto. Stock insuficiente.", "OK");
                }
            }
        }

        // Método para disminuir la cantidad de un producto
        private void OnDisminuirCantidadClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var productoVenta = (ProductoVenta)button.BindingContext;

            // Verificar si la cantidad es mayor a 1 (para no quitar más cantidad de la que existe)
            if (productoVenta.Cantidad > 1)
            {
                productoVenta.Cantidad--;
                productoVenta.Subtotal = productoVenta.Cantidad * productoVenta.Precio;

                // Actualizar el total
                ActualizarTotal();

                // Actualizar la UI
                ProductosListView.ItemsSource = null;
                ProductosListView.ItemsSource = ProductosEnVenta;
            }
        }

        private async void OnRealizarVentaClicked(object sender, EventArgs e)
        {
            // Mostrar cuadro de entrada para datos del cliente
            var nombreCliente = await DisplayPromptAsync("Datos del Cliente", "Ingrese su nombre completo:", initialValue: "", placeholder: "Nombre Completo");
            var apellidosCliente = await DisplayPromptAsync("Datos del Cliente", "Ingrese sus apellidos:", initialValue: "", placeholder: "Apellidos");
            var celularCliente = await DisplayPromptAsync("Datos del Cliente", "Ingrese su celular:", initialValue: "", placeholder: "Celular");

            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(nombreCliente) || string.IsNullOrWhiteSpace(apellidosCliente) || string.IsNullOrWhiteSpace(celularCliente))
            {
                await DisplayAlert("Error", "Por favor ingrese todos los datos del cliente.", "OK");
                return;
            }

            // Insertar al cliente en la base de datos
            int clienteId = await InsertarClienteEnBaseDeDatos(nombreCliente, apellidosCliente, celularCliente);

            // Generar el ticket de la venta
            var totalVenta = ProductosEnVenta.Sum(p => p.Subtotal); // Calcular el total de la venta

            var ticket = new StringBuilder();
            ticket.AppendLine($"Ticket de Venta");
            ticket.AppendLine($"Nombre Cliente: {nombreCliente} {apellidosCliente}");
            ticket.AppendLine($"Celular Cliente: {celularCliente}");
            ticket.AppendLine($"Empleado: {MainPage.NombreUsuario}");
            ticket.AppendLine("Productos:");
            foreach (var producto in ProductosEnVenta)
            {
                ticket.AppendLine($"{producto.Nombre} - Cantidad: {producto.Cantidad} - Subtotal: {producto.Subtotal:C}");
            }
            ticket.AppendLine($"Total: {totalVenta:C}");

            // Mostrar el ticket al usuario
            await DisplayAlert("Ticket de Venta", ticket.ToString(), "OK");

            // Insertar la venta en la base de datos
            int ventaId = await InsertarVentaEnBaseDeDatos(clienteId, totalVenta, MainPage.NombreUsuario);

            // Guardar el ticket en un archivo .txt
            await InsertarDetalleVenta(ventaId, clienteId);

            // Limpiar la lista de productos después de la venta
            ProductosEnVenta.Clear();
            ProductosListView.ItemsSource = ProductosEnVenta; // Actualizar el ListView
        }

        // Método para insertar cliente en la base de datos
        private async Task<int> InsertarClienteEnBaseDeDatos(string nombre, string apellidos, string celular)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO clientes (Nombre, Apellidos, Celular) VALUES (@Nombre, @Apellidos, @Celular);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@Celular", celular);

                await cmd.ExecuteNonQueryAsync();

                // Obtener el ID del cliente insertado
                query = "SELECT LAST_INSERT_ID();";
                cmd = new MySqlCommand(query, connection);
                var clienteId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                return clienteId;
            }
        }

        // Método para insertar la venta en la base de datos
        private async Task<int> InsertarVentaEnBaseDeDatos(int clienteId, decimal totalVenta, string empleado)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO ventas (ClienteID, Total, Empleado) VALUES (@ClienteID, @Total, @Empleado);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cmd.Parameters.AddWithValue("@Total", totalVenta);
                cmd.Parameters.AddWithValue("@Empleado", empleado);

                await cmd.ExecuteNonQueryAsync();

                // Obtener el ID de la venta insertada
                var queryVentaId = "SELECT LAST_INSERT_ID();";
                cmd = new MySqlCommand(queryVentaId, connection);
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        // Método para insertar los detalles de la venta en la base de datos
        private async Task InsertarDetalleVenta(int ventaId, int clienteId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var producto in ProductosEnVenta)
                {
                    // Obtener el ID del producto desde la base de datos
                    var queryProductoId = "SELECT idProducto FROM inventario WHERE nombre = @Nombre;";
                    var cmdProductoId = new MySqlCommand(queryProductoId, connection);
                    cmdProductoId.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    var productoId = Convert.ToInt32(await cmdProductoId.ExecuteScalarAsync());

                    // Insertar detalle de la venta
                    var queryDetalle = @"INSERT INTO detalleventas 
                                 (VentaID, ClienteID, ProductoID, Cantidad, Subtotal) 
                                 VALUES (@VentaID, @ClienteID, @ProductoID, @Cantidad, @Subtotal);";
                    var cmdDetalle = new MySqlCommand(queryDetalle, connection);
                    cmdDetalle.Parameters.AddWithValue("@VentaID", ventaId);
                    cmdDetalle.Parameters.AddWithValue("@ClienteID", clienteId);
                    cmdDetalle.Parameters.AddWithValue("@ProductoID", productoId);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", producto.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@Subtotal", producto.Subtotal);

                    await cmdDetalle.ExecuteNonQueryAsync();

                    // Actualizar el stock en la tabla inventario
                    var queryActualizarStock = "UPDATE inventario SET cantidad = cantidad - @Cantidad WHERE idProducto = @ProductoID;";
                    var cmdActualizarStock = new MySqlCommand(queryActualizarStock, connection);
                    cmdActualizarStock.Parameters.AddWithValue("@Cantidad", producto.Cantidad);
                    cmdActualizarStock.Parameters.AddWithValue("@ProductoID", productoId);

                    await cmdActualizarStock.ExecuteNonQueryAsync();
                }
            }
        }


        // Método para actualizar el total de la venta
        private void ActualizarTotal()
        {
            decimal total = ProductosEnVenta.Sum(p => p.Subtotal);
            TotalLabel.Text = $"Total: {total:C}"; // Actualiza el texto del label con el total formateado
        }

        // Cargar los productos cuando la página aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarProductos(); // Cargar los productos al aparecer la página
        }
    }

    // Clase ProductoVenta que hereda de Producto
    public class ProductoVenta : Producto
    {
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public int StockDisponible { get; set; }

        // Constructor de ProductoVenta
        public ProductoVenta(string nombre, decimal precio, int cantidad, string descripcion, string proveedor)
            : base(nombre, precio, cantidad, descripcion, proveedor)
        {
            Cantidad = cantidad;
            Subtotal = precio * cantidad;
            StockDisponible = cantidad; // El stock disponible es el mismo que la cantidad inicial
        }
    }
}