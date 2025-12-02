using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.ViewModels;
using CajuAjuda.Desktop.Views;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace CajuAjuda.Desktop
{
    public static class MauiProgram
    {
        public static IServiceProvider? Services { get; private set; }

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

            // --- CONFIGURAÇÃO DA INJEÇÃO DE DEPENDÊNCIA ---
            // Backend no Azure
            string baseAddress = "https://cajuajuda-backend-engaa9gfezfndcgd.eastus2-01.azurewebsites.net";

            // Registra o AuthorizationMessageHandler
            builder.Services.AddTransient<AuthorizationMessageHandler>();

            // Configura o HttpClientFactory para os serviços
            builder.Services.AddHttpClient<AuthService>(client =>
                client.BaseAddress = new Uri(baseAddress));

            builder.Services.AddHttpClient<ChamadoService>(client =>
                client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

            builder.Services.AddHttpClient<UsuarioService>(client =>
                client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

            // Registra as Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetalheChamadoPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddSingleton<AppShell>();

            // Registra os ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<DetalheChamadoViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            var app = builder.Build();
            Services = app.Services;

            return app;
        }
    }
}
