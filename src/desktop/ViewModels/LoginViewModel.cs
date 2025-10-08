using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Services;

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

                if (loginResponse is not null && !string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    await SecureStorage.Default.SetAsync("auth_token", loginResponse.Token);

                    // ======================================================
                    //               A CORREÇÃO PRINCIPAL ESTÁ AQUI
                    // ======================================================
                    // Substitui a página de Login (que é a página atual)
                    // pela estrutura principal do aplicativo (o AppShell).
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
#pragma warning disable CA1416
                    await Shell.Current.DisplayAlert("Erro", "Email ou senha inválidos.", "OK");
#pragma warning restore CA1416
                }
            }
            catch (System.Exception ex)
            {
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Erro", $"Falha na comunicação: {ex.Message}", "OK");
#pragma warning restore CA1416
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Senha) && !IsBusy;
        }
    }
}