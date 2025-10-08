using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.ViewModels;
using CajuAjuda.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Desktop
{
    public static class MauiProgram
    {
        // Adicionamos uma propriedade estática para acessar os serviços
    public static IServiceProvider Services { get; private set; } = default!;

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

            string baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                               ? "http://10.0.2.2:5205"
                               : "http://localhost:5205";

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            });

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ChamadoService>();

            // Adicionamos o AppShell como Singleton também
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<DetalheChamadoViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetalheChamadoPage>();

            // Construímos o app e guardamos o provedor de serviços
            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}