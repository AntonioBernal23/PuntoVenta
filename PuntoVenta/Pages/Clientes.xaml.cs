using MySql.Data.MySqlClient;
using PuntoVenta.Models;

namespace PuntoVenta.Pages;

public partial class Clientes : ContentPage
{
    public readonly string _connectionString = Conexion.ConnectionString;

    public Clientes()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ClientesListView.ItemsSource = await ObtenerClientesAsync();
    }

	private async Task<List<ClientesEnBd>> ObtenerClientesAsync()
	{
		var ClientesEnBd = new List<ClientesEnBd>();

		using (var connection = new MySqlConnection(_connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT * FROM clientes;"; 

			using (var cmd = new MySqlCommand(query, connection))
			{
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
                        var cliente = new ClientesEnBd
                        {
                            ClienteId = reader.GetInt32(0),
                            Nombre = reader.GetString(1), // Usa GetString para columnas de texto
                            Apellidos = reader.GetString(2), // Usa GetString para columnas de texto
                            Ceular = reader.GetString(3) // Esto es correcto si la columna es un entero
                        };
                        ClientesEnBd.Add(cliente); // Agregar el cliente a la lista
                    }
				}
			}
		}
		return ClientesEnBd;
	}

    public class ClientesEnBd
	{
		public int ClienteId { get; set; }
		public string Nombre { get; set; }
		public string Apellidos { get; set; }
		public string Ceular { get; set; }

	}
}