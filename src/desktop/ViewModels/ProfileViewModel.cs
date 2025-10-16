using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly UsuarioService _usuarioService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SavePasswordCommand))]
        private string _senhaAtual = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SavePasswordCommand))]
        private string _novaSenha = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SavePasswordCommand))]
        private string _confirmarNovaSenha = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public ProfileViewModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SavePasswordAsync()
        {
            if (NovaSenha != ConfirmarNovaSenha)
            {
                await DisplaySafeAlert("Erro", "A nova senha e a confirmação não correspondem.");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new SenhaUpdateDto
                {
                    SenhaAtual = SenhaAtual,
                    NovaSenha = NovaSenha
                };

                await _usuarioService.UpdateSenhaAsync(dto);
                await DisplaySafeAlert("Sucesso", "Sua senha foi alterada com sucesso!");

                // Limpa os campos após o sucesso
                SenhaAtual = string.Empty;
                NovaSenha = string.Empty;
                ConfirmarNovaSenha = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplaySafeAlert("Erro", $"Não foi possível alterar a senha: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(SenhaAtual) &&
                   !string.IsNullOrWhiteSpace(NovaSenha) &&
                   !string.IsNullOrWhiteSpace(ConfirmarNovaSenha) &&
                   !IsBusy;
        }

        private async Task DisplaySafeAlert(string title, string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            }
        }
    }
}