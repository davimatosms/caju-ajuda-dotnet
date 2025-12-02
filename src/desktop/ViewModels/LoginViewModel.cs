using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Services;
using System;

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string email = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string senha = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] Tentando login: {Email}");
                var loginResponse = await _authService.LoginAsync(Email, Senha);
                
                if (loginResponse != null && !string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGIN] ✅ Login bem-sucedido!");
                    await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);
                    if (MauiProgram.Services != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = MauiProgram.Services.GetService<AppShell>();
                        });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGIN] ❌ Falha no login");
                    await DisplaySafeAlert("Erro de Login", 
                        "Email ou senha inválidos, ou sua conta ainda não foi verificada.\n\n" +
                        "Verifique se você está usando as credenciais corretas:\n" +
                        "• Admin: admin@cajuajuda.com / Admin@2025\n" +
                        "• Técnico: tecnico@cajuajuda.com / Tecnico@2025");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] ❌ Erro HTTP: {ex.Message}");
                await DisplaySafeAlert("Erro de Conexão", 
                    $"Não foi possível conectar ao servidor.\n\n" +
                    $"Erro: {ex.Message}\n\n" +
                    $"Verifique sua conexão com a internet.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOGIN] ❌ Erro geral: {ex.Message}");
                await DisplaySafeAlert("Erro", $"Falha inesperada: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Senha) && !IsBusy;

        private async Task DisplaySafeAlert(string title, string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            }
        }
    }
}