using CajuAjuda.Desktop.Views;
using System.Diagnostics;

namespace CajuAjuda.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                // Sempre abre na tela de login
                if (MauiProgram.Services == null)
                {
                    Debug.WriteLine("ERRO: MauiProgram.Services é nulo!");
                    throw new InvalidOperationException("MauiProgram.Services não foi inicializado");
                }

                var loginPage = MauiProgram.Services.GetService<LoginPage>();
                if (loginPage == null)
                {
                    Debug.WriteLine("ERRO: Não foi possível obter LoginPage do container de DI");
                    throw new InvalidOperationException("LoginPage não está registrada no container de DI");
                }

                MainPage = loginPage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO ao inicializar App: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Mostra página de erro simples
                MainPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        Padding = 20,
                        Spacing = 10,
                        Children =
                        {
                            new Label { Text = "Erro ao inicializar aplicativo", FontSize = 20, FontAttributes = FontAttributes.Bold },
                            new Label { Text = ex.Message, TextColor = Colors.Red },
                            new Label { Text = ex.StackTrace, FontSize = 10 }
                        }
                    }
                };
            }
        }
    }
}
