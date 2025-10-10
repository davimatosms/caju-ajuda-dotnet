using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Services;
using System;
using System.Diagnostics; // Adicionar esta referência

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _email = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _senha = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

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

                    // LINHA DE DEBUG: Vamos ver se o token foi salvo e qual é o seu valor.
                    Debug.WriteLine($"[LoginViewModel] Token foi SALVO com sucesso. Valor: {loginResponse.Token.Substring(0, 20)}...");

                    Application.Current.MainPage = MauiProgram.Services.GetService<AppShell>();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Email ou senha inválidos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha na comunicação: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Senha) && !IsBusy;
    }
}