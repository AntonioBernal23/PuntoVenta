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
        private Entry NombreEntry, ApellidosEntry, UsuarioEntry, Contrase�aEntry;
        private ListView empleadosListView;

        string _connectionString = Conexion.ConnectionString;
        ObservableCollection<Empleado> _empleados;

        public Administracion()
        {
            InitializeComponent();
            _empleados = new ObservableCollection<Empleado>();
            InicializarEntrys();
        }

        // M�todo para inicializar Entry
        private async Task InicializarEntrys()
        {
            await Task.Delay(100);

            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                NombreEntry = (Entry)FindByName("NombreEntryMovil");
                ApellidosEntry = (Entry)FindByName("ApellidosEntryMovil");
                UsuarioEntry = (Entry)FindByName("UsuarioEntryMovil");
                Contrase�aEntry = (Entry)FindByName("Contrase�aEntryMovil");
                empleadosListView = (ListView)FindByName("empleadosListViewMovil");
            }
            else
            {
                NombreEntry = (Entry)FindByName("NombreEntryPc");
                ApellidosEntry = (Entry)FindByName("ApellidosEntryPc");
                UsuarioEntry = (Entry)FindByName("UsuarioEntryPc");
                Contrase�aEntry = (Entry)FindByName("Contrase�aEntryPc");
                empleadosListView = (ListView)FindByName("empleadosListViewPc");
            }

            var empleados = await ObtenerEmpleadosAsync();
            foreach (var entry in empleados)
            {
                _empleados.Add(entry);
            }

            empleadosListView.ItemsSource = _empleados;
        }

        // M�todo para obtener empleados desde la base de datos
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

        //Metodo para encriptar contrase�a
        private string EncriptarContrase�a(string contrase�a)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrase�a));
                var builder = new StringBuilder();
                foreach(var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // M�todo para agregar un nuevo empleado
        private async void OnAgregarEmpleadoClicked(object sender, EventArgs e)
        {
            var nombre = NombreEntry.Text;
            var apellidos = ApellidosEntry.Text;
            var usuario = UsuarioEntry.Text;
            var contrase�a = Contrase�aEntry.Text;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrase�a))
            {
                await DisplayAlert("Error", "Debes llenar todos los campos", "OK");
            }

            var contrase�aEncriptada = EncriptarContrase�a(contrase�a);

            await AgregarEmpleadoAsync(nombre, apellidos, usuario, contrase�aEncriptada);
        }

        //Metodo para agregar empleado a la base de datos
        private async Task AgregarEmpleadoAsync(string nombre, string apellidos, string usuario, string contrase�a)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO empleados (nombre, apellidos, usuario, contrase�a) VALUES (@nombre, @apellidos, @usuario, @contrase�a)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@apellidos", apellidos);
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contrase�a", contrase�a);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await DisplayAlert("Exito", "Empleado agregado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al agregar al empleado: {ex.Message}", "OK");
            }
        }       

        // M�todo para actualizar la informaci�n de un empleado
        private async void OnActualizarInformacionClicked(object sender, EventArgs e)
        {
            // L�gica para actualizar la informaci�n de un empleado
        }

        // M�todo para eliminar un empleado
        private async void OnEliminarEmpleadoClicked(object sender, EventArgs e)
        {
            // L�gica para eliminar un empleado
        }
    }
}
