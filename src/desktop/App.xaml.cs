using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#pragma warning disable CA1416
            var token = SecureStorage.GetAsync("auth_token").Result;
#pragma warning restore CA1416

            if (!string.IsNullOrEmpty(token))
            {
                MainPage = MauiProgram.Services!.GetService<AppShell>();
            }
            else
            {
                MainPage = MauiProgram.Services!.GetService<LoginPage>();
            }
        }
    }
}