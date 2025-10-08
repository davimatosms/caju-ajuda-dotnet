using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Apenas registramos as rotas para navegação interna
            Routing.RegisterRoute(nameof(DetalheChamadoPage), typeof(DetalheChamadoPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }
    }
}