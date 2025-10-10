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

#pragma warning disable CA1416
            string baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                               ? "http://10.0.2.2:5205"
                               : "http://localhost:5205";
#pragma warning restore CA1416

            builder.Services.AddTransient<AuthenticationMessageHandler>();

            builder.Services.AddHttpClient<AuthService>(client => client.BaseAddress = new Uri(baseAddress));
            builder.Services.AddHttpClient<ChamadoService>(client => client.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<AuthenticationMessageHandler>();

            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<DetalheChamadoViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetalheChamadoPage>();

            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}