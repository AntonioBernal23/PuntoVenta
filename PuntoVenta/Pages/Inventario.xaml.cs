using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace PuntoVenta.Pages
{
    public partial class Inventario : ContentPage
    {
        // Variables para los Entrys
        private Entry NombreProductoEntry, PrecioProductoEntry, CantidadProductoEntry, DescripcionProductoEntry;
        private List<Picker> ProveedorProductoPickers; // Lista de Pickers

        // Conexion a la base de datos
        private string _connectionString = "Server=35.226.94.228;Database=PuntoVenta;Uid=root;Pwd=1234;";

        public Inventario()
        {
            InitializeComponent();
            InicializarEntrys();
        }

        // Inicializa los Entrys y los Pickers
        private void InicializarEntrys()
        {
            // Inicializando los Entrys
            NombreProductoEntry = (Entry)FindByName("NombreProductoEntry");
            PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntry");
            CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntry");
            DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntry");

            // Inicializando la lista de Pickers
            ProveedorProductoPickers = new List<Picker>
            {
                (Picker)FindByName("ProveedorProductoPickerPc"), // Picker para PC
                (Picker)FindByName("ProveedorProductoPickerMovil") // Picker para móvil
            };
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
            string nombre = NombreProductoEntry?.Text;
            string precio = PrecioProductoEntry?.Text;
            string cantidad = CantidadProductoEntry?.Text;
            string descripcion = DescripcionProductoEntry?.Text;

            // Verificar cuál de los dos Pickers está seleccionado
            string proveedor = ObtenerProveedorSeleccionado();

            // Procesa los datos (ejemplo: mostrando una alerta)
            DisplayAlert("Producto Agregado", $"Nombre: {nombre}\nPrecio: {precio}\nCantidad: {cantidad}\nDescripción: {descripcion}\nProveedor: {proveedor}", "OK");
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
            return "Proveedor no seleccionado";
        }

        // Evento al hacer clic en el botón "Ver Inventario"
        private async void OnVerInventarioClicked(object sender, EventArgs e)
        {

        }
    }
}
