using CajuAjuda.Desktop.Views;
using System;

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
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage)); // Nova rota do Perfil
        }
    }
}