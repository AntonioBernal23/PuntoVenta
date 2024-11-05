namespace PuntoVenta.Pages;

public partial class Inicio : ContentPage
{
	public Inicio()
	{
		InitializeComponent();
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