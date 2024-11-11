namespace PuntoVenta.Pages
{
    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();
            DesactivarBotones();
            lblEncabezado.Text = $"Hola, {MainPage.NombreUsuario}";
        }

        // Método para desactivar botones si es un empleado
        private void DesactivarBotones()
        {
            if (MainPage.IsEmpleado)
            {
#if WINDOWS || MACCATALYST
                InventarioBtnPc.IsEnabled = false;
                ClientesBtnPc.IsEnabled = false;
                AdministracionBtnPc.IsEnabled = false;
#endif
#if ANDROID || IOS
                InventarioBtnMovil.IsEnabled = false;
                ClientesBtnMovil.IsEnabled = false;
                AdministracionBtnMovil.IsEnabled = false;
#endif
            }
        }

        private async void OnVentasClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Ventas());
        }

        private async void OnInventarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Inventario());
        }

        private async void OnClientesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Clientes());
        }

        private async void OnAdministracionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Administracion());
        }
    }
}
