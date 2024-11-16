using MySql.Data.MySqlClient;
using PuntoVenta.Models;
using System.Windows.Input;

namespace PuntoVenta.Pages;

public partial class HistorialVentas : ContentPage
{
    public readonly string _connectionString = Conexion.ConnectionString;

    public ICommand EliminarVentaCommand { get; private set; }

    public HistorialVentas()
    {
        InitializeComponent();

        // Inicializamos el comando
        EliminarVentaCommand = new Command<int>(async (ventaId) => await EliminarVentaAsync(ventaId));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        VentasListView.ItemsSource = await ObtenerVentasAgrupadasAsync();
    }

    private async Task<List<VentaAgrupada>> ObtenerVentasAgrupadasAsync()
    {
        var ventasAgrupadas = new List<VentaAgrupada>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = @"
            SELECT 
                dv.VentaID AS VentaID,
                dv.ClienteID AS ClienteID,
                GROUP_CONCAT(CONCAT(p.nombre, ' (Cantidad: ', dv.Cantidad, ')') SEPARATOR ', ') AS Productos,
                SUM(dv.Subtotal) AS Total
            FROM 
                detalleVentas dv
            JOIN 
                inventario p ON dv.ProductoID = p.idProducto
            GROUP BY 
                dv.VentaID, dv.ClienteID;
        ";

            using (var cmd = new MySqlCommand(query, connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var venta = new VentaAgrupada
                        {
                            VentaID = reader.GetInt32(0),       // Índice 0: dv.VentaID
                            ClienteID = reader.GetInt32(1),    // Índice 1: dv.ClienteID
                            Productos = reader.GetString(2),   // Índice 2: GROUP_CONCAT(...)
                            Total = reader.GetDecimal(3)       // Índice 3: SUM(dv.Subtotal)
                        };

                        ventasAgrupadas.Add(venta);
                    }
                }
            }
        }

        return ventasAgrupadas;
    }

    private async void OnEliminarVentaClicked(object sender, EventArgs e)
    {
        var confirmacion = await DisplayAlert("Confirmar", "¿Estás seguro de eliminar este ticket?", "Sí", "No");

        if (confirmacion)
        {
            var button = sender as Button;
            if (button != null && button.CommandParameter is int ventaId)
            {
                // Llamar a la función de eliminación pasando el VentaID
                await EliminarVentaAsync(ventaId);
            }
        }
    }


    //eliminar ticket
    private async Task EliminarVentaAsync(int ventaId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Transacción para eliminar el ticket
            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    // Eliminar las entradas de detalleVentas
                    string deleteDetalleQuery = "DELETE FROM detalleVentas WHERE VentaID = @VentaID";
                    using (var cmd = new MySqlCommand(deleteDetalleQuery, connection, (MySqlTransaction)transaction))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", ventaId);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Eliminar la venta principal (si es necesario)
                    string deleteVentaQuery = "DELETE FROM ventas WHERE VentaID = @VentaID";
                    using (var cmd = new MySqlCommand(deleteVentaQuery, connection, (MySqlTransaction)transaction))
                    {
                        cmd.Parameters.AddWithValue("@VentaID", ventaId);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Commit de la transacción
                    await transaction.CommitAsync();

                    // Actualizar la lista
                    VentasListView.ItemsSource = await ObtenerVentasAgrupadasAsync();
                }
                catch (Exception)
                {
                    // Si algo falla, hacemos rollback
                    await transaction.RollbackAsync();
                    await DisplayAlert("Error", "Hubo un error al eliminar la venta", "OK");
                }
            }
        }
    }

    public class VentaAgrupada
    {
        public int VentaID { get; set; }
        public int ClienteID { get; set; }
        public string Productos { get; set; } // Lista de productos como texto
        public decimal Total { get; set; }   // Total de la venta
    }
}
