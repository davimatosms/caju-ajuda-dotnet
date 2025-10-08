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

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private ObservableCollection<Chamado> _chamados = new();

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
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os chamados: {ex.Message}", "OK");
#pragma warning restore CA1416
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Chamado chamado)
        {
            if (chamado == null) return;

#pragma warning disable CA1416
            await Shell.Current.GoToAsync($"{nameof(DetalheChamadoPage)}?ChamadoId={chamado.Id}");
#pragma warning restore CA1416
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
#pragma warning disable CA1416
            bool confirm = await Shell.Current.DisplayAlert("Confirmar Logout", "Você tem certeza que deseja sair?", "Sim", "Não");
            if (confirm)
            {
                SecureStorage.Default.Remove("auth_token");

                // O "//" reinicia a pilha de navegação, mostrando a LoginPage como a única página.
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
#pragma warning restore CA1416
        }
    }
}