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

        // Duas coleções separadas para as abas da interface
        public ObservableCollection<Chamado> MeusChamados { get; } = new();
        public ObservableCollection<Chamado> ChamadosDisponiveis { get; } = new();

        public MainViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        // Comando único para carregar todos os dados necessários para o dashboard
        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                // Limpa as listas antes de carregar os novos dados
                MeusChamados.Clear();
                ChamadosDisponiveis.Clear();

                // Inicia as duas chamadas à API em paralelo para economizar tempo
                var meusChamadosTask = _chamadoService.GetMeusChamadosAsync();
                var disponiveisTask = _chamadoService.GetChamadosDisponiveisAsync();

                // Espera ambas as chamadas terminarem
                await Task.WhenAll(meusChamadosTask, disponiveisTask);

                // Popula a lista de "Meus Chamados"
                foreach (var chamado in meusChamadosTask.Result)
                {
                    MeusChamados.Add(chamado);
                }

                // Popula a lista de "Chamados Disponíveis"
                foreach (var chamado in disponiveisTask.Result)
                {
                    ChamadosDisponiveis.Add(chamado);
                }
            }
            catch (Exception ex)
            {
                await DisplaySafeAlert("Erro", $"Não foi possível carregar os chamados: {ex.Message}");
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

            await Shell.Current.GoToAsync($"{nameof(DetalheChamadoPage)}?ChamadoId={chamado.Id}");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await DisplaySafeAlert("Confirmar Logout", "Você tem certeza?", "Sim", "Não");
            if (confirm)
            {
                SecureStorage.Default.Remove("auth_token");
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