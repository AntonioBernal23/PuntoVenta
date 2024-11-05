namespace PuntoVenta.Pages;

public partial class Inventario : ContentPage
{
    // Variables para cada Entry que se usar�n en ambas plataformas
    private Entry NombreProductoEntry, PrecioProductoEntry, CantidadProductoEntry, DescripcionProductoEntry;

    public Inventario()
	{
		InitializeComponent();

		InicializarEntrys();
	}

    private void InicializarEntrys()
    {
        // Asigna cada Entry usando FindByName y un "cast" expl�cito a Entry
        NombreProductoEntry = (Entry)FindByName("NombreProductoEntry");
        PrecioProductoEntry = (Entry)FindByName("PrecioProductoEntry");
        CantidadProductoEntry = (Entry)FindByName("CantidadProductoEntry");
        DescripcionProductoEntry = (Entry)FindByName("DescripcionProductoEntry");
    }

    private void OnAgregarProductoClicked(object sender, EventArgs e)
    {
        // Usa los entries en una l�gica compartida sin duplicaci�n
        string nombre = NombreProductoEntry?.Text;
        string precio = PrecioProductoEntry?.Text;
        string cantidad = CantidadProductoEntry?.Text;
        string descripcion = DescripcionProductoEntry?.Text;

        // Procesa los datos de los entries (ejemplo de salida)
        DisplayAlert("Producto Agregado", $"Nombre: {nombre}\nPrecio: {precio}\nCantidad: {cantidad}\nDescripci�n: {descripcion}", "OK");
    }

    private async void OnVerInventarioClicked(object sender, EventArgs e)
	{

	}
}