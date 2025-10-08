using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Inicia a página principal assincronamente para evitar bloqueio no construtor.
            // O método ignora o aviso de análise estática sobre "async void" pois aqui
            // queremos disparar o fluxo de inicialização sem bloquear o construtor.
            _ = InitializeMainPageAsync();
        }

        private async System.Threading.Tasks.Task InitializeMainPageAsync()
        {
            try
            {
                var token = string.Empty;
#pragma warning disable CA1416
                token = await SecureStorage.GetAsync("auth_token");
#pragma warning restore CA1416

                if (!string.IsNullOrEmpty(token))
                {
                    MainPage = MauiProgram.Services.GetService<AppShell>();
                }
                else
                {
                    MainPage = MauiProgram.Services.GetService<LoginPage>();
                }
            }
            catch
            {
                // Em caso de falha no SecureStorage, exibimos o login por segurança.
                MainPage = MauiProgram.Services.GetService<LoginPage>();
            }
        }
    }
}