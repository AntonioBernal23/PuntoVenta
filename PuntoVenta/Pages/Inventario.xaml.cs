using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using PuntoVenta.Models;

namespace PuntoVenta.Pages
{
    public partial class Inventario : ContentPage
    {
        // Variables para los Entrys
        private Entry NombreProductoEntry, PrecioProductoEntry, CantidadProductoEntry, DescripcionProductoEntry;
        private List<Picker> ProveedorProductoPickers; // Lista de Pickers

        // Conexion a la base de datos
        string _connectionString = Conexion.ConnectionString;

        public Inventario()
        {
            InitializeComponent();
            InicializarEntrys();
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
            }

            // Registro de mensajes para verificación
            Console.WriteLine($"NombreProductoEntry is null: {NombreProductoEntry == null}");
            Console.WriteLine($"PrecioProductoEntry is null: {PrecioProductoEntry == null}");
            Console.WriteLine($"CantidadProductoEntry is null: {CantidadProductoEntry == null}");
            Console.WriteLine($"DescripcionProductoEntry is null: {DescripcionProductoEntry == null}");
            foreach (var picker in ProveedorProductoPickers)
            {
                Console.WriteLine($"Picker is null: {picker == null}");
            }
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
                // Limpiar los pickers antes de agregar los proveedores
                foreach (var picker in ProveedorProductoPickers)
                {
                    picker.Items.Clear();
                }

                // Agregar los proveedores a los Pickers
                foreach (var proveedor in proveedores)
                {
                    foreach (var picker in ProveedorProductoPickers)
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
                                if (!string.IsNullOrEmpty(proveedor))
                                {
                                    proveedores.Add(proveedor);
                                }
                                else
                                {
                                    proveedores.Add("Proveedor desconocido");
                                }
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
        private void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            // Obtener los valores de los campos de texto
            string nombre = NombreProductoEntry?.Text?.Trim();
            string precioTexto = PrecioProductoEntry?.Text?.Trim();
            string cantidadTexto = CantidadProductoEntry?.Text?.Trim();
            string descripcion = DescripcionProductoEntry?.Text?.Trim();
            string proveedor = ObtenerProveedorSeleccionado()?.Trim();

            // Verifica si alguno de los campos es nulo o vacío
            if (string.IsNullOrEmpty(nombre) ||
                string.IsNullOrEmpty(precioTexto) ||
                string.IsNullOrEmpty(cantidadTexto) ||
                string.IsNullOrEmpty(descripcion) ||
                string.IsNullOrEmpty(proveedor))
            {
                DisplayAlert("Error", "No puede haber ningún campo vacío.", "OK");
                return;
            }

            // Verificar si la cantidad es un número entero
            if (!int.TryParse(cantidadTexto, out int cantidad))
            {
                DisplayAlert("Error", "El campo 'Cantidad' debe ser un número entero.", "OK");
                return;
            }

            // Verificar si el precio es un número decimal (que incluye enteros)
            if (!decimal.TryParse(precioTexto, out decimal precio))
            {
                DisplayAlert("Error", "El campo 'Precio' debe ser un número válido (entero o decimal).", "OK");
                return;
            }

            // Crear una instancia de Producto
            Producto nuevoProducto = new Producto(nombre, precio, cantidad, descripcion, proveedor);

            // Insertar el producto en la base de datos
            InsertarProductoAsync(nuevoProducto);
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al agregar el producto: {ex.Message}", "OK");
            }
        }

        // Método para limpiar campos
        private void LimpiarCampos()
        {
            // Limpiar Entrys
            NombreProductoEntry.Text = string.Empty;
            PrecioProductoEntry.Text = string.Empty;
            CantidadProductoEntry.Text = string.Empty;
            DescripcionProductoEntry.Text = string.Empty;

            // Limpiar Pickers
            foreach (var picker in ProveedorProductoPickers)
            {
                picker.SelectedItem = null;
            }
        }

        // Obtener el proveedor seleccionado, ya sea del Picker para PC o para Movil
        private string ObtenerProveedorSeleccionado()
        {
            // Recorremos los Pickers en la lista y tomamos el seleccionado
            foreach (var picker in ProveedorProductoPickers)
            {
                var proveedorSeleccionado = picker.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(proveedorSeleccionado))
                {
                    return proveedorSeleccionado;
                }
            }
            return null;
        }

        // Evento al hacer clic en el botón "Ver Inventario"
        private async void OnVerInventarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.VerInventario());
        }
    }
}
