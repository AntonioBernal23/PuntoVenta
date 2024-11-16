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
        // Declaración de Entry
        public Entry NombreProductoEntry { get; set; } = new Entry();
        public Entry PrecioProductoEntry { get; set; } = new Entry();
        public Entry CantidadProductoEntry { get; set; } = new Entry();
        public Entry DescripcionProductoEntry { get; set; } = new Entry();

        // Declaración de Picker
        public List<Picker> ProveedorProductoPickers;

        // Declaración de ListView
        public ListView productosListView;

        // Declaración de ObservableCollection de Producto
        public ObservableCollection<Producto> _productos = new ObservableCollection<Producto>();
        public Producto productoSeleccionado;

        // Cadena de conexión a base de datos
        public readonly string _connectionString = Conexion.ConnectionString;

        // Declaración de botones
        public Button AgregarProductoBtn { get; private set; }
        public Button ActualizarInformacionBtn { get; private set; }
        public Button EliminarProductoBtn { get; private set; }
        public Button CancelarBtn { get; private set; }

        public Inventario()
        {
            InitializeComponent();
            InicializarEntrys();
        }

        private async Task InicializarEntrys()
        {
            await Task.Delay(100);

            // Inicialización basada en plataforma
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                NombreProductoEntry = (Entry)FindByName("NombreProductoEntryMovil");
                PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntryMovil");
                CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntryMovil");
                DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntryMovil");
                ProveedorProductoPickers = new List<Picker> { (Picker)FindByName("ProveedorProductoPickerMovil") };
                productosListView = (ListView)FindByName("productosListViewMovil");
                AgregarProductoBtn = (Button)FindByName("AgregarProductoBtnMovil");
                ActualizarInformacionBtn = (Button)FindByName("ActualizarInformacionBtnMovil");
                EliminarProductoBtn = (Button)FindByName("EliminarProductoBtnMovil");
                CancelarBtn = (Button)FindByName("CancelarBtnMovil");

                productosListView.ItemSelected += ProductosListView_ItemSelected;
                await ActualizarListaProductosAsync();
            }
            else
            {
                NombreProductoEntry = (Entry)FindByName("NombreProductoEntryPc");
                PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntryPc");
                CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntryPc");
                DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntryPc");
                ProveedorProductoPickers = new List<Picker> { (Picker)FindByName("ProveedorProductoPickerPc") };
                productosListView = (ListView)FindByName("productosListViewPc");
                AgregarProductoBtn = (Button)FindByName("AgregarProductoBtnPc");
                ActualizarInformacionBtn = (Button)FindByName("ActualizarInformacionBtnPc");
                EliminarProductoBtn = (Button)FindByName("EliminarProductoBtnPc");
                CancelarBtn = (Button)FindByName("CancelarBtnPc");

                productosListView.ItemSelected += ProductosListView_ItemSelected;
                await ActualizarListaProductosAsync();
            }
            await Task.Delay(200);
            await CargarProveedores();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Dispatcher.DispatchAsync(async () => await CargarProveedores());
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            // Asegurarnos de que la actualización de la UI se haga en el hilo principal
            Dispatcher.DispatchAsync(() =>
            {
                ActualizarInformacionBtn.IsVisible = false;
                EliminarProductoBtn.IsVisible = false;
                CancelarBtn.IsVisible = false;
                AgregarProductoBtn.IsVisible = true;
            });

            // Limpiar campos de texto
            LimpiarCampos();

            // Desmarcar la selección en la lista
            productosListView.SelectedItem = null;
        }

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
                            productos.Add(new Producto(
                                reader["nombre"].ToString(),
                                Convert.ToDecimal(reader["precio"]),
                                Convert.ToInt32(reader["cantidad"]),
                                reader["descripcion"].ToString(),
                                reader["proveedor"].ToString()
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al obtener los productos: {ex.Message}", "OK");
                }
            }
            return productos;
        }

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
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            proveedores.Add(reader.GetString(0) ?? "Proveedor desconocido");
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

        private async void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            if (ValidarCampos(out Producto nuevoProducto))
            {
                //modificar botones
                ActualizarInformacionBtn.IsVisible = false;
                EliminarProductoBtn.IsVisible = false;
                CancelarBtn.IsVisible = false;
                AgregarProductoBtn.IsVisible = true;

                await EjecutarOperacionProductoAsync(nuevoProducto, "INSERT INTO inventario (nombre, precio, cantidad, descripcion, proveedor) VALUES (@nombre, @precio, @cantidad, @descripcion, @proveedor)", "Producto agregado correctamente.");
                await ActualizarListaProductosAsync();
            }
        }

        private async void OnActualizarInformacionClicked(object sender, EventArgs e)
        {
            if (productoSeleccionado != null && ValidarCampos(out Producto productoActualizado))
            {
                //modificar botones
                ActualizarInformacionBtn.IsVisible = false;
                EliminarProductoBtn.IsVisible = false;
                CancelarBtn.IsVisible = false;
                AgregarProductoBtn.IsVisible = true;

                await EjecutarOperacionProductoAsync(productoActualizado, "UPDATE inventario SET precio=@precio, cantidad=@cantidad, descripcion=@descripcion, proveedor=@proveedor WHERE nombre=@nombre", "Producto actualizado correctamente.");
                await ActualizarListaProductosAsync();
            }
        }

        private async void OnEliminarProductoClicked(object sender, EventArgs e)
        {
            if (productoSeleccionado != null && await DisplayAlert("Confirmación", $"¿Está seguro de eliminar el producto '{productoSeleccionado.Nombre}'?", "Sí", "No"))
            {
                //modificar botones
                ActualizarInformacionBtn.IsVisible = false;
                EliminarProductoBtn.IsVisible = false;
                CancelarBtn.IsVisible = false;
                AgregarProductoBtn.IsVisible = true;

                await EjecutarOperacionProductoAsync(productoSeleccionado, "DELETE FROM inventario WHERE nombre=@nombre", "Producto eliminado correctamente.");
                await ActualizarListaProductosAsync();
            }
        }

        private async Task EjecutarOperacionProductoAsync(Producto producto, string query, string mensajeExito)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
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
                await DisplayAlert("Éxito", mensajeExito, "OK");
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error en la operación: {ex.Message}", "OK");
            }
        }

        private async Task ActualizarListaProductosAsync()
        {
            _productos.Clear();
            var productos = await Task.Run(() => ObtenerProductosAsync());
            await Dispatcher.DispatchAsync(() =>
            {
                productos.ForEach(producto => _productos.Add(producto));
                productosListView.ItemsSource = _productos;
            });
        }

        private async Task CargarProveedores()
        {
            if (ProveedorProductoPickers == null || ProveedorProductoPickers.Count == 0)
            {
                await Task.Delay(100);
                return;
            }

            var proveedores = await Task.Run(() => ObtenerProveedoresAsync());
            if (proveedores.Count > 0)
            {
                await Dispatcher.DispatchAsync(() =>
                {
                    foreach (var picker in ProveedorProductoPickers)
                    {
                        picker.Items.Clear();
                        proveedores.ForEach(proveedor => picker.Items.Add(proveedor));
                    }
                });
            }
            else
            {
                await DisplayAlert("Error", "No se encontraron proveedores.", "OK");
            }
        }

        private void ProductosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Modificar botones
            AgregarProductoBtn.IsVisible = false;
            ActualizarInformacionBtn.IsVisible = true;
            EliminarProductoBtn.IsVisible = true;
            CancelarBtn.IsVisible = true;

            if (e.SelectedItem is Producto producto)
            {
                productoSeleccionado = producto;
                NombreProductoEntry.Text = producto.Nombre;
                PrecioProductoEntry.Text = producto.Precio.ToString();
                CantidadProductoEntry.Text = producto.Cantidad.ToString();
                DescripcionProductoEntry.Text = producto.Descripcion;
                // Selección del proveedor
                if (ProveedorProductoPickers != null && ProveedorProductoPickers.Count > 0)
                {
                    var picker = ProveedorProductoPickers[0];
                    picker.SelectedItem = producto.Proveedor;
                }
            }
        }

        private void LimpiarCampos()
        {
            NombreProductoEntry.Text = string.Empty;
            PrecioProductoEntry.Text = string.Empty;
            CantidadProductoEntry.Text = string.Empty;
            DescripcionProductoEntry.Text = string.Empty;
            foreach (var picker in ProveedorProductoPickers)
            {
                picker.SelectedItem = null;
            }
            productoSeleccionado = null;
        }

        private bool ValidarCampos(out Producto producto)
        {
            producto = null;
            if (string.IsNullOrWhiteSpace(NombreProductoEntry.Text) ||
                string.IsNullOrWhiteSpace(PrecioProductoEntry.Text) ||
                string.IsNullOrWhiteSpace(CantidadProductoEntry.Text) ||
                string.IsNullOrWhiteSpace(DescripcionProductoEntry.Text) ||
                ProveedorProductoPickers[0].SelectedItem == null)
            {
                DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return false;
            }

            decimal precio;
            int cantidad;
            if (!decimal.TryParse(PrecioProductoEntry.Text, out precio) || !int.TryParse(CantidadProductoEntry.Text, out cantidad))
            {
                DisplayAlert("Error", "El precio y la cantidad deben ser números válidos.", "OK");
                return false;
            }

            producto = new Producto(NombreProductoEntry.Text, precio, cantidad, DescripcionProductoEntry.Text, ProveedorProductoPickers[0].SelectedItem.ToString());
            return true;
        }
    }
}
