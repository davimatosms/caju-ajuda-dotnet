using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Sempre abre na tela de login
            MainPage = MauiProgram.Services!.GetService<LoginPage>();
        }
    }
}
