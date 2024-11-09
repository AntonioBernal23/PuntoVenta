using MySql.Data.MySqlClient;
using PuntoVenta.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVenta
{
    public partial class MainPage : ContentPage
    {
        string _connectionString = Conexion.ConnectionString;

        public MainPage()
        {
            InitializeComponent();
        }

        public static bool IsEmpleado { get; private set; }

        // Evento de clic en el login de empleados
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var usuario = EntryUser.Text;
            var contraseña = EntryPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                await DisplayAlert("Error", "Por favor, ingresa un nombre de usuario y una contraseña.", "OK");
                return;
            }

            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            // Verificar las credenciales de los empleados
            var esValido = await VerificarCredencialesEmpleadosAsync(usuario, contraseñaEncriptada);

            if (esValido)
            {
                IsEmpleado = true; // Marcar que el usuario es un empelado
                EntryUser.Text = "";
                EntryPassword.Text = "";
                await Navigation.PushAsync(new Pages.Inicio()); // Redirige a la página de inicio
            }
            else
            {
                await DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
            }
        }

        // Evento de clic en el login de administradores
        private async void OnLoginAdminClicked(object sender, EventArgs e)
        {
            var usuario = EntryUser.Text;
            var contraseña = EntryPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                await DisplayAlert("Error", "Por favor, ingresa un nombre de usuario y una contraseña.", "OK");
                return;
            }

            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            // Verificar las credenciales de los administradores
            var esValido = await VerificarCredencialesAsync(usuario, contraseñaEncriptada);

            if (esValido)
            {
                IsEmpleado = false; // Marcar que el usuario es un administrador
                await Navigation.PushAsync(new Pages.Inicio()); // Redirige a la página de inicio
            }
            else
            {
                await DisplayAlert("Error", "Usuario o contraseña incorrectos", "OK");
            }
        }

        // Método para verificar las credenciales del administrador
        private async Task<bool> VerificarCredencialesAsync(string usuario, string contraseñaEncriptada)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM administradores WHERE usuario = @usuario AND contraseña = @contraseña";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", contraseñaEncriptada);

                        // Ejecutar la consulta y verificar si el usuario existe con la contraseña correcta
                        var result = await command.ExecuteScalarAsync();

                        // Si el resultado es mayor a 0, las credenciales son correctas
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al verificar las credenciales: {ex.Message}", "OK");
                return false;
            }
        }

        // Método para verificar las credenciales de los empleados
        private async Task<bool> VerificarCredencialesEmpleadosAsync(string usuario, string contraseñaEncriptada)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) FROM empleados WHERE usuario = @usuario AND contraseña = @contraseña";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", contraseñaEncriptada);

                        // Ejecutar la consulta y verificar si el usuario existe con la contraseña correcta
                        var result = await command.ExecuteScalarAsync();

                        // Si el resultado es mayor a 0, las credenciales son correctas
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al verificar las credenciales: {ex.Message}", "OK");
                return false;
            }
        }

        // Método para encriptar la contraseña con SHA256
        private string EncriptarContraseña(string contraseña)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Método para agregar un usuario administrador (comentado)
        /*
        private async void OnAgregar(object sender, EventArgs e)
        {
            var nombre = NombreEntry.Text;
            var usuario = UsuarioEntry.Text;
            var contraseña = ContraseñaEntry.Text;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                await DisplayAlert("Error", "Llena todos los campos", "ok");
            }

            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            await AgregarAdministradorAsync(nombre, usuario, contraseñaEncriptada);
        }
        

        // Método para agregar un administrador a la base de datos (comentado)
        private async Task AgregarAdministradorAsync(string nombre, string usuario, string contraseña)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO administradores (nombre, usuario, contraseña) VALUES (@nombre, @usuario, @contraseña)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", contraseña);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await DisplayAlert("Exito", "Administrador agregado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al agregar al administrador: {ex.Message}", "OK");
            }
        }
        */
    }
}
