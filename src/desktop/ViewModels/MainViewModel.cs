// CajuAjuda.Desktop/ViewModels/MainViewModel.cs

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using Microsoft.Maui.Controls;
using CajuAjuda.Desktop.Views; 

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ChamadoService _chamadoService;

        [ObservableProperty]
        private ObservableCollection<Chamado> _chamados;

        [ObservableProperty]
        private bool _isBusy;

        public MainViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
            _chamados = new ObservableCollection<Chamado>();
            Task.Run(LoadChamadosAsync);
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
            catch (System.Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os chamados: {ex.Message}", "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar Logout", "Você tem certeza de que deseja sair?", "Sim", "Não");
            if (confirm)
            {
                SecureStorage.Default.Remove("jwt_token");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}