using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;

namespace PuntoVenta.Pages
{
    public partial class VerInventario : ContentPage
    {
        private string _connectionString = "Server=LocalHost;Database=PuntoVenta;Uid=root;Pwd=1234;";

        public VerInventario()
        {
            InitializeComponent();
        }
    }
}
