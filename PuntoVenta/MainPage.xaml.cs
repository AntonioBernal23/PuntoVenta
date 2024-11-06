using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVenta
{
    public partial class MainPage : ContentPage
    {
        private string _connectionString = "Server=35.226.94.228;Database=PuntoVenta;Uid=root;Pwd=1234;";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string user = "user1";
            string pass = "pass";

            if (EntryUser.Text == user && EntryPassword.Text == pass)
            {
                await Navigation.PushAsync(new Pages.Inicio());
            }
            else
            {
                DisplayAlert("Error", "Usuario o contraseña incorrectos", "Intentar de nuevo");

                EntryUser.Text = "";
                EntryPassword.Text = "";
            }

        }

        //Para agregar un usuario administrador a la bd
        /*
        private async void OnAgregar(object sender, EventArgs e)
        {
            var nombre = NombreEntry.Text;
            var usuario = UsuarioEntry.Text;
            var contraseña = ContraseñaEntry.Text;

            if(string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                await DisplayAlert("Error", "Llena todos los campos", "ok");
            }

            var contraseñaEncriptada = EncriptarContraseña(contraseña);

            await AgregarAdministradorAsync(nombre, usuario, contraseñaEncriptada);
        }

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
        } */

    }

}
