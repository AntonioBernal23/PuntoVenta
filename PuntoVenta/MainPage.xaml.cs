namespace PuntoVenta
{
    public partial class MainPage : ContentPage
    {
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

    }

}
