// CajuAjuda.Desktop/ViewModels/LoginViewModel.cs

using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.Views; // Adicione para ter acesso a MainPage
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // <<-- ESTE USING ESTAVA FALTANDO
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private string _email;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private string _senha;

        [ObservableProperty]
        private bool _isBusy;

        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Senha) && !IsBusy;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync()
        {
            IsBusy = true;
            try
            {
                var loginRequest = new LoginRequest { Email = this.Email, Senha = this.Senha };
                var response = await _authService.LoginAsync(loginRequest);

                if (!string.IsNullOrWhiteSpace(response.Token))
                {
                    await SecureStorage.Default.SetAsync("jwt_token", response.Token);
                    await Shell.Current.GoToAsync($"{nameof(MainPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro de Login", "A API não retornou um token.", "Tentar Novamente");
                }
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro de Login", ex.Message, "Tentar Novamente");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}