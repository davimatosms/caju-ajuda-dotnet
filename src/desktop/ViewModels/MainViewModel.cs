using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ChamadoService _chamadoService;
        // A dependência do AuthService foi removida, não é mais necessária aqui.

        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private ObservableCollection<Chamado> _chamados = new();

        public MainViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        [RelayCommand]
        private async Task LoadChamadosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var chamadosList = await _chamadoService.GetChamadosAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Chamados.Clear();
                    foreach (var chamado in chamadosList)
                    {
                        Chamados.Add(chamado);
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplaySafeAlert("Erro", $"Não foi possível carregar os chamados: {ex.Message}");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Chamado chamado)
        {
            if (chamado == null) return;
            await Shell.Current.GoToAsync($"{nameof(DetalheChamadoPage)}?ChamadoId={chamado.Id}");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await DisplaySafeAlert("Confirmar Logout", "Você tem certeza?", "Sim", "Não");
            if (confirm)
            {
                SecureStorage.Default.Remove("auth_token");
                // A chamada para _authService.ClearAuthToken() foi REMOVIDA.
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        private async Task<bool> DisplaySafeAlert(string title, string message, string accept, string cancel)
        {
            if (Application.Current?.MainPage != null)
            {
#pragma warning disable CA1416
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
#pragma warning restore CA1416
            }
            return false;
        }
        private async Task DisplaySafeAlert(string title, string message)
        {
            if (Application.Current?.MainPage != null)
            {
#pragma warning disable CA1416
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
#pragma warning restore CA1416
            }
        }
    }
}