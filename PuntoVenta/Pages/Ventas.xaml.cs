namespace PuntoVenta.Pages;

public partial class Ventas : ContentPage
{
	public Ventas()
	{
		InitializeComponent();
		DesactivarBotones();
	}

	private void DesactivarBotones()
	{
		if (MainPage.IsEmpleado)
		{
#if WINDOWS || MACCATALYST
			HistorialVentasBtnPc.IsEnabled = false;
#endif
#if ANDROID || IOS
            HistorialVentasBtnMovil.IsEnabled = false;
#endif
		}
	}

	private async void OnRealizarVentasClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new Pages.RealizarVentas());
    }

	private async void OnHistorialVentasClicked(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new Pages.HistorialVentas());
    }
}