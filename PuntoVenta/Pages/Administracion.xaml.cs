using PuntoVenta.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PuntoVenta.Pages
{
    public partial class Administracion : ContentPage
    {
        private Entry NombreEntry, ApellidosEntry, UsuarioEntry, ContraseñaEntry;
        private ListView empleadosListView;

        string _connectionString = Conexion.ConnectionString;
        ObservableCollection<Empleado> _empleados;

        //Defino propiedades de los botones
        public Button AgregarEmpleadoBtn { get; private set; }
        public Button ActualizarInformacionBtn { get; private set; }
        public Button EliminarEmpleadoBtn { get; private set; }
        public Button CancelarBtn { get; private set; }

        public Administracion()
        {
            InitializeComponent();
            _empleados = new ObservableCollection<Empleado>();
            InicializarEntrys();
        }

        //Metodo para obtener elemento seleccionado del listview
        private void EmpleadosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null && e.SelectedItem is Empleado empleadoSeleccionado)
            {
                NombreEntry.Text = empleadoSeleccionado.nombre;
                ApellidosEntry.Text = empleadoSeleccionado.apellidos;
                UsuarioEntry.Text = empleadoSeleccionado.usuario;
                ContraseñaEntry.Text = empleadoSeleccionado.contraseña;

                AgregarEmpleadoBtn.IsVisible = false;
                ActualizarInformacionBtn.IsVisible = true;
                EliminarEmpleadoBtn.IsVisible = true;
                CancelarBtn.IsVisible = true;
            }
        }

        //Metodo para deseleccionar elemento de listview
        private void OnCancelarClicked(object sender, EventArgs e)
        {
            // Deseleccionar el elemento del ListView
            empleadosListView.SelectedItem = null;

            // Limpiar los campos de entrada
            NombreEntry.Text = string.Empty;
            ApellidosEntry.Text = string.Empty;
            UsuarioEntry.Text = string.Empty;
            ContraseñaEntry.Text = string.Empty;

            // Restaurar visibilidad de los botones
            AgregarEmpleadoBtn.IsVisible = true;
            ActualizarInformacionBtn.IsVisible = false;
            EliminarEmpleadoBtn.IsVisible = false;
            CancelarBtn.IsVisible = false;

        }

        // Método para inicializar Entry
        private async Task InicializarEntrys()
        {
            await Task.Delay(100);

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                NombreEntry = (Entry)FindByName("NombreEntryMovil");
                ApellidosEntry = (Entry)FindByName("ApellidosEntryMovil");
                UsuarioEntry = (Entry)FindByName("UsuarioEntryMovil");
                ContraseñaEntry = (Entry)FindByName("ContraseñaEntryMovil");
                empleadosListView = (ListView)FindByName("empleadosListViewMovil");
                AgregarEmpleadoBtn = (Button)FindByName("AgregarEmpleadoBtnMovil");
                ActualizarInformacionBtn = (Button)FindByName("ActualizarInformacionBtnMovil");
                EliminarEmpleadoBtn = (Button)FindByName("EliminarEmpleadoBtnMovil");
                CancelarBtn = (Button)FindByName("CancelarBtn");
            }
            else
            {
                NombreEntry = (Entry)FindByName("NombreEntryPc");
                ApellidosEntry = (Entry)FindByName("ApellidosEntryPc");
                UsuarioEntry = (Entry)FindByName("UsuarioEntryPc");
                ContraseñaEntry = (Entry)FindByName("ContraseñaEntryPc");
                empleadosListView = (ListView)FindByName("empleadosListViewPc");
                AgregarEmpleadoBtn = (Button)FindByName("AgregarEmpleadoBtnPc");
                ActualizarInformacionBtn = (Button)FindByName("ActualizarInformacionBtnPc");
                EliminarEmpleadoBtn = (Button)FindByName("EliminarEmpleadoBtnPc");
                CancelarBtn = (Button)FindByName("CancelarBtnPc");
            }

            empleadosListView.ItemSelected += EmpleadosListView_ItemSelected;

            var empleados = await ObtenerEmpleadosAsync();
            foreach (var entry in empleados)
            {
                _empleados.Add(entry);
            }

            empleadosListView.ItemsSource = _empleados;
        }

        // Método para obtener empleados desde la base de datos
        private async Task<List<Empleado>> ObtenerEmpleadosAsync()
        {
            List<Empleado> empleados = new List<Empleado>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT nombre, apellidos, usuario FROM empleados";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var empleado = new Empleado
                            {
                                nombre = reader["nombre"].ToString(),
                                apellidos = reader["apellidos"].ToString(),
                                usuario = reader["usuario"].ToString()
                            };

                            empleados.Add(empleado);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener empleados: {ex.Message}");
                }
            }
            return empleados;
        }

        //Metodo para encriptar contraseña
        private string EncriptarContraseña(string contraseña)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                var builder = new StringBuilder();
                foreach(var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Método para agregar un nuevo empleado
        private async void OnAgregarEmpleadoClicked(object sender, EventArgs e)
        {
            var nombre = NombreEntry.Text;
            var apellidos = ApellidosEntry.Text;
            var usuario = UsuarioEntry.Text;
            var contraseña = ContraseñaEntry.Text;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                await DisplayAlert("Error", "No puede haber ningún campo vacío.", "OK");
            }

            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            await AgregarEmpleadoAsync(nombre, apellidos, usuario, contraseñaEncriptada);
        }

        //Metodo para agregar empleado a la base de datos
        private async Task AgregarEmpleadoAsync(string nombre, string apellidos, string usuario, string contraseña)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO empleados (nombre, apellidos, usuario, contraseña) VALUES (@nombre, @apellidos, @usuario, @contraseña)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@apellidos", apellidos);
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", contraseña);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Recargar la lista de empleados
                _empleados.Clear();
                var empleados = await ObtenerEmpleadosAsync();
                foreach (var empleado in empleados)
                {
                    _empleados.Add(empleado);
                }

                empleadosListView.ItemsSource = _empleados;

                await DisplayAlert("Exito", "Empleado agregado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al agregar al empleado: {ex.Message}", "OK");
            }
        }

        // Método para actualizar la información de un empleado
        private async void OnActualizarInformacionClicked(object sender, EventArgs e)
        {
            // Obtener los datos del Entry
            var nombre = NombreEntry.Text;
            var apellidos = ApellidosEntry.Text;
            var usuario = UsuarioEntry.Text;
            var contraseña = ContraseñaEntry.Text;

            // Validar que no haya campos vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                await DisplayAlert("Error", "Debes llenar todos los campos", "OK");
                return;
            }

            // Encriptar la nueva contraseña
            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            // Obtener el empleado seleccionado
            if (empleadosListView.SelectedItem != null && empleadosListView.SelectedItem is Empleado empleadoSeleccionado)
            {
                // Actualizar el empleado en la base de datos
                await ActualizarEmpleadoAsync(empleadoSeleccionado, nombre, apellidos, usuario, contraseñaEncriptada);

                // Recargar la lista de empleados
                _empleados.Clear();
                var empleados = await ObtenerEmpleadosAsync();
                foreach (var empleado in empleados)
                {
                    _empleados.Add(empleado);
                }

                empleadosListView.ItemsSource = _empleados;

                //Deseleccionar elemento del listview
                empleadosListView.SelectedItem = null;

                // Limpiar los campos de entrada
                NombreEntry.Text = string.Empty;
                ApellidosEntry.Text = string.Empty;
                UsuarioEntry.Text = string.Empty;
                ContraseñaEntry.Text = string.Empty;

                // Ocultar el botón de actualización y mostrar el de agregar
                AgregarEmpleadoBtn.IsVisible = true;
                ActualizarInformacionBtn.IsVisible = false;
                EliminarEmpleadoBtn.IsVisible = false;
                CancelarBtn.IsVisible = false;

                // Mostrar mensaje de éxito
                await DisplayAlert("Éxito", "Empleado actualizado correctamente", "OK");
            }
        }

        // Método para actualizar el empleado en la base de datos
        private async Task ActualizarEmpleadoAsync(Empleado empleado, string nombre, string apellidos, string usuario, string contraseña)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE empleados SET nombre = @nombre, apellidos = @apellidos, usuario = @usuario, contraseña = @contraseña WHERE usuario = @usuarioOriginal";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@apellidos", apellidos);
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", contraseña);
                        command.Parameters.AddWithValue("@usuarioOriginal", empleado.usuario); // Usamos el usuario original como criterio para actualizar
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al actualizar al empleado: {ex.Message}", "OK");
            }
        }


        // Método para eliminar un empleado
        private async void OnEliminarEmpleadoClicked(object sender, EventArgs e)
        {
            // Obtener el empleado seleccionado
            if (empleadosListView.SelectedItem != null && empleadosListView.SelectedItem is Empleado empleadoSeleccionado)
            {
                // Confirmar con el usuario antes de eliminar
                var confirmacion = await DisplayAlert("Confirmar", "¿Estás seguro de eliminar a este empleado?", "Sí", "No");

                if (confirmacion)
                {
                    // Eliminar el empleado de la base de datos
                    await EliminarEmpleadoAsync(empleadoSeleccionado);

                    // Recargar la lista de empleados
                    _empleados.Clear();
                    var empleados = await ObtenerEmpleadosAsync();
                    foreach (var empleado in empleados)
                    {
                        _empleados.Add(empleado);
                    }

                    empleadosListView.ItemsSource = _empleados;

                    // Deseleccionar el elemento del ListView
                    empleadosListView.SelectedItem = null;

                    // Limpiar los campos de entrada
                    NombreEntry.Text = string.Empty;
                    ApellidosEntry.Text = string.Empty;
                    UsuarioEntry.Text = string.Empty;
                    ContraseñaEntry.Text = string.Empty;

                    // Ocultar el botón de eliminación y mostrar el de agregar
                    AgregarEmpleadoBtn.IsVisible = true;
                    ActualizarInformacionBtn.IsVisible = false;
                    EliminarEmpleadoBtn.IsVisible = false;
                    CancelarBtn.IsVisible = false;

                    // Mostrar mensaje de éxito
                    await DisplayAlert("Éxito", "Empleado eliminado correctamente", "OK");
                }
            }
            else
            {
                // Si no hay un empleado seleccionado, mostrar un mensaje de error
                await DisplayAlert("Error", "Debes seleccionar un empleado para eliminar", "OK");
            }
        }

        // Método para eliminar el empleado de la base de datos
        private async Task EliminarEmpleadoAsync(Empleado empleado)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM empleados WHERE usuario = @usuario";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@usuario", empleado.usuario); // Usamos el usuario como criterio para eliminar
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar al empleado: {ex.Message}", "OK");
            }
        }
    }
}
