using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using PuntoVenta.Models;
using System.Collections.ObjectModel;

namespace PuntoVenta.Pages
{
    public partial class Inventario : ContentPage
    {
        // Variables para los Entrys
        private Entry NombreProductoEntry, PrecioProductoEntry, CantidadProductoEntry, DescripcionProductoEntry;
        private List<Picker> ProveedorProductoPickers;
        private ListView productosListView;

        // Conexión a la base de datos
        private readonly string _connectionString = Conexion.ConnectionString;
        private ObservableCollection<Producto> _productos = new ObservableCollection<Producto>();

        // Producto seleccionado para editar o eliminar
        private Producto productoSeleccionado;

        public Inventario()
        {
            InitializeComponent();
            InicializarEntrys();
        }

        // Método para obtener el elemento seleccionado del ListView
        private void ProductosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null && e.SelectedItem is Producto producto)
            {
                productoSeleccionado = producto; // Guardamos el producto seleccionado

                NombreProductoEntry.Text = producto.Nombre;
                PrecioProductoEntry.Text = producto.Precio.ToString();
                CantidadProductoEntry.Text = producto.Cantidad.ToString();
                DescripcionProductoEntry.Text = producto.Descripcion;

                foreach (var picker in ProveedorProductoPickers)
                {
                    if (productoSeleccionado.Proveedor != null)
                    {
                        picker.SelectedItem = productoSeleccionado.Proveedor;
                    }
                }

                // Visibilidad de botones
                AgregarProductoBtnPc.IsVisible = false;
                ActualizarInformacionBtnPc.IsVisible = true;
                EliminarProductoBtnPc.IsVisible = true;
            }
        }

        // Inicializa los Entrys y los Pickers
        private async Task InicializarEntrys()
        {
            await Task.Delay(100);

            // Asignación condicional basada en la plataforma
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                NombreProductoEntry = (Entry)FindByName("NombreProductoEntryMovil");
                PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntryMovil");
                CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntryMovil");
                DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntryMovil");
                ProveedorProductoPickers = new List<Picker>
                {
                    (Picker)FindByName("ProveedorProductoPickerMovil")
                };
                productosListView = (ListView)FindByName("productosListViewMovil");
            }
            else
            {
                NombreProductoEntry = (Entry)FindByName("NombreProductoEntryPc");
                PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntryPc");
                CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntryPc");
                DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntryPc");
                ProveedorProductoPickers = new List<Picker>
                {
                    (Picker)FindByName("ProveedorProductoPickerPc")
                };
                productosListView = (ListView)FindByName("productosListViewPc");

                productosListView.ItemSelected += ProductosListView_ItemSelected;

                var productos = await ObtenerProductosAsync();
                foreach (var producto in productos)
                {
                    _productos.Add(producto);
                }

                productosListView.ItemsSource = _productos;
            }
        }

        // Método para obtener productos desde la base de datos
        private async Task<List<Producto>> ObtenerProductosAsync()
        {
            List<Producto> productos = new List<Producto>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT nombre, precio, cantidad, descripcion, proveedor FROM inventario;";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var producto = new Producto(
                                reader["nombre"].ToString(),
                                Convert.ToDecimal(reader["precio"]),
                                Convert.ToInt32(reader["cantidad"]),
                                reader["descripcion"].ToString(),
                                reader["proveedor"].ToString()
                            );

                            productos.Add(producto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al obtener el producto: {ex.Message}", "OK");
                }
            }
            return productos;
        }

        // Evento OnAppearing: Carga los proveedores cuando la página aparece
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarProveedores();
        }

        // Cargar proveedores desde la base de datos
        private async Task CargarProveedores()
        {
            var proveedores = await ObtenerProveedoresAsync();
            if (proveedores != null && proveedores.Count > 0)
            {
                foreach (var picker in ProveedorProductoPickers)
                {
                    picker.Items.Clear();
                    foreach (var proveedor in proveedores)
                    {
                        picker.Items.Add(proveedor);
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "No se encontraron proveedores.", "OK");
            }
        }

        // Obtener proveedores desde la base de datos
        private async Task<List<string>> ObtenerProveedoresAsync()
        {
            var proveedores = new List<string>();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT nombre FROM proveedores";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var proveedor = reader.GetString(0);
                                proveedores.Add(!string.IsNullOrEmpty(proveedor) ? proveedor : "Proveedor desconocido");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar proveedores: {ex.Message}", "OK");
            }
            return proveedores;
        }

        // Evento al hacer clic en el botón "Agregar Producto"
        private async void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            string nombre = NombreProductoEntry?.Text?.Trim();
            string precioTexto = PrecioProductoEntry?.Text?.Trim();
            string cantidadTexto = CantidadProductoEntry?.Text?.Trim();
            string descripcion = DescripcionProductoEntry?.Text?.Trim();
            string proveedor = ObtenerProveedorSeleccionado()?.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(precioTexto) ||
                string.IsNullOrEmpty(cantidadTexto) || string.IsNullOrEmpty(descripcion) || string.IsNullOrEmpty(proveedor))
            {
                await DisplayAlert("Error", "No puede haber ningún campo vacío.", "OK");
                return;
            }

            if (!int.TryParse(cantidadTexto, out int cantidad) || !decimal.TryParse(precioTexto, out decimal precio))
            {
                await DisplayAlert("Error", "Verifique que la cantidad sea un número entero y el precio un número válido.", "OK");
                return;
            }

            Producto nuevoProducto = new Producto(nombre, precio, cantidad, descripcion, proveedor);
            await InsertarProductoAsync(nuevoProducto);
            ObtenerProductosAsync();
        }

        // Método para insertar producto en la base de datos
        private async Task InsertarProductoAsync(Producto producto)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO inventario (nombre, precio, cantidad, descripcion, proveedor) VALUES (@nombre, @precio, @cantidad, @descripcion, @proveedor)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@precio", producto.Precio);
                        command.Parameters.AddWithValue("@cantidad", producto.Cantidad);
                        command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@proveedor", producto.Proveedor);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await DisplayAlert("Éxito", "Producto agregado correctamente.", "OK");
                LimpiarCampos();
                await ActualizarListaProductosAsync();
                _productos.Add(producto);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al agregar el producto: {ex.Message}", "OK");
            }
        }

        // Método para actualizar producto
        private async void OnActualizarInformacionClicked(object sender, EventArgs e)
        {
            if (productoSeleccionado == null)
            {
                await DisplayAlert("Error", "Seleccione un producto para actualizar.", "OK");
                return;
            }

            string nombre = NombreProductoEntry?.Text?.Trim();
            string precioTexto = PrecioProductoEntry?.Text?.Trim();
            string cantidadTexto = CantidadProductoEntry?.Text?.Trim();
            string descripcion = DescripcionProductoEntry?.Text?.Trim();
            string proveedor = ObtenerProveedorSeleccionado()?.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(precioTexto) ||
                string.IsNullOrEmpty(cantidadTexto) || string.IsNullOrEmpty(descripcion) || string.IsNullOrEmpty(proveedor))
            {
                await DisplayAlert("Error", "No puede haber ningún campo vacío.", "OK");
                return;
            }

            if (!int.TryParse(cantidadTexto, out int cantidad) || !decimal.TryParse(precioTexto, out decimal precio))
            {
                await DisplayAlert("Error", "Verifique que la cantidad sea un número entero y el precio un número válido.", "OK");
                return;
            }

            productoSeleccionado.Nombre = nombre;
            productoSeleccionado.Precio = precio;
            productoSeleccionado.Cantidad = cantidad;
            productoSeleccionado.Descripcion = descripcion;
            productoSeleccionado.Proveedor = proveedor;

            await ActualizarProductoAsync(productoSeleccionado);
        }

        // Método para actualizar la base de datos
        private async Task ActualizarProductoAsync(Producto producto)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE inventario SET nombre = @nombre, precio = @precio, cantidad = @cantidad, descripcion = @descripcion, proveedor = @proveedor WHERE nombre = @nombre";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@precio", producto.Precio);
                        command.Parameters.AddWithValue("@cantidad", producto.Cantidad);
                        command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@proveedor", producto.Proveedor);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await DisplayAlert("Éxito", "Producto actualizado correctamente.", "OK");
                LimpiarCampos();
                await ActualizarListaProductosAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al actualizar el producto: {ex.Message}", "OK");
            }
        }

        // Método para eliminar producto
        private async void OnEliminarProductoClicked(object sender, EventArgs e)
        {
            if (productoSeleccionado == null)
            {
                await DisplayAlert("Error", "Seleccione un producto para eliminar.", "OK");
                return;
            }

            bool confirmacion = await DisplayAlert("Confirmación", $"¿Está seguro de eliminar el producto '{productoSeleccionado.Nombre}'?", "Sí", "No");

            if (confirmacion)
            {
                await EliminarProductoAsync(productoSeleccionado);
            }
        }

        // Método para eliminar producto de la base de datos
        private async Task EliminarProductoAsync(Producto producto)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM inventario WHERE nombre = @nombre";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", producto.Nombre);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await DisplayAlert("Éxito", "Producto eliminado correctamente.", "OK");
                await ActualizarListaProductosAsync();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar el producto: {ex.Message}", "OK");
            }
        }

        // Limpiar los campos después de una acción
        private void LimpiarCampos()
        {
            NombreProductoEntry.Text = string.Empty;
            PrecioProductoEntry.Text = string.Empty;
            CantidadProductoEntry.Text = string.Empty;
            DescripcionProductoEntry.Text = string.Empty;

            AgregarProductoBtnPc.IsVisible = true;
            ActualizarInformacionBtnPc.IsVisible = false;
            EliminarProductoBtnPc.IsVisible = false;
        }

        // Método para obtener proveedor seleccionado
        private string ObtenerProveedorSeleccionado()
        {
            foreach (var picker in ProveedorProductoPickers)
            {
                if (picker.SelectedItem != null)
                {
                    return picker.SelectedItem.ToString();
                }
            }
            return string.Empty;
        }

        private async Task ActualizarListaProductosAsync()
        {
            try
            {
                // Obtener los productos actualizados desde la base de datos
                var productos = await ObtenerProductosAsync();

                // Limpiar la colección actual
                _productos.Clear();

                // Agregar los productos obtenidos a la colección
                foreach (var producto in productos)
                {
                    _productos.Add(producto);
                }

                // Volver a asignar la colección al ListView (esto también actualiza la UI)
                productosListView.ItemsSource = null;
                productosListView.ItemsSource = _productos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al actualizar la lista de productos: {ex.Message}", "OK");
            }
        }

    }
}
