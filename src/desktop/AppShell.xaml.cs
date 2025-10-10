using CajuAjuda.Desktop.Views;
using System;

namespace CajuAjuda.Desktop
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Apenas registramos as rotas para navegação interna.
            // A lógica de verificação de token foi removida daqui.
            Routing.RegisterRoute(nameof(DetalheChamadoPage), typeof(DetalheChamadoPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }
    }
}