// CajuAjuda.Desktop/MauiProgram.cs

using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.ViewModels;
using CajuAjuda.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Registrando Serviços
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ChamadoService>();

            // Registrando Views (Páginas)
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetalheChamadoPage>();

            // Registrando ViewModels
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<DetalheChamadoViewModel>();


            return builder.Build();
        }
    }
}