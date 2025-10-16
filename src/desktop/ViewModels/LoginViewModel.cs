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
                var loginResponse = await _authService.LoginAsync(Email, Senha);
                if (loginResponse != null && !string.IsNullOrWhiteSpace(loginResponse.Token))
                {
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
                    await DisplaySafeAlert("Erro", "Email ou senha inválidos.");
                }
            }
            catch (Exception ex)
            {
                await DisplaySafeAlert("Erro", $"Falha na comunicação: {ex.Message}");
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