using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registra todas as páginas que podem ser navegadas a partir do Shell
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(DetalheChamadoPage), typeof(DetalheChamadoPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert(
                "Sair",
                "Deseja realmente sair do sistema?",
                "Sim",
                "Não");

            if (confirmLogout)
            {
                // Remove o token
                SecureStorage.Default.Remove("auth_token");

                // Navega para a tela de login
                Application.Current!.MainPage = MauiProgram.Services!.GetService<LoginPage>();
            }
        }
    }
}
