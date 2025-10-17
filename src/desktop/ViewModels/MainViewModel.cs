using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using CajuAjuda.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ChamadoService _chamadoService;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private int _selectedTab = 0;

        // Três coleções separadas para as abas da interface
        public ObservableCollection<Chamado> MeusChamados { get; } = new();
        public ObservableCollection<Chamado> ChamadosDisponiveis { get; } = new();
        public ObservableCollection<Chamado> ChamadosFechados { get; } = new();

        public MainViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        [RelayCommand]
        private void SelectTab(int tabIndex)
        {
            SelectedTab = tabIndex;
        }

        /// <summary>
        /// Carrega todos os dados do dashboard (Meus Chamados, Disponíveis e Fechados)
        /// </summary>
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
                ChamadosFechados.Clear();

                // Carrega os chamados do técnico (em andamento)
                var meusChamados = await _chamadoService.GetMeusChamadosAsync();
                if (meusChamados != null)
                {
                    foreach (var chamado in meusChamados)
                    {
                        // Filtra apenas os que estão em andamento
                        if (chamado.Status != "FECHADO" && chamado.Status != "RESOLVIDO" && chamado.Status != "CANCELADO")
                        {
                            MeusChamados.Add(chamado);
                        }
                        else
                        {
                            ChamadosFechados.Add(chamado);
                        }
                    }
                }

                // Carrega os chamados disponíveis (sem técnico atribuído)
                var disponiveis = await _chamadoService.GetChamadosDisponiveisAsync();
                if (disponiveis != null)
                {
                    foreach (var chamado in disponiveis)
                    {
                        ChamadosDisponiveis.Add(chamado);
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Erro de conexão ou autenticação
                if (httpEx.Message.Contains("401") || httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.DisplayAlert("Não autenticado", "Você precisa fazer login primeiro.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro de conexão", $"Não foi possível conectar ao servidor: {httpEx.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar chamados: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Abre a página de detalhes de um chamado específico
        /// </summary>
        [RelayCommand]
        private async Task OpenChamadoAsync(Chamado chamado)
        {
            if (chamado == null) return;

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "ChamadoId", chamado.Id }
                };

                await Shell.Current.GoToAsync(nameof(DetalheChamadoPage), parameters);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao abrir chamado: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Atribui um chamado disponível ao técnico logado
        /// </summary>
        [RelayCommand]
        private async Task AtribuirChamadoAsync(Chamado chamado)
        {
            if (chamado == null || IsBusy) return;

            try
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Assumir Chamado",
                    $"Deseja assumir o chamado '{chamado.Titulo}'?",
                    "Sim",
                    "Não");

                if (!confirm) return;

                IsBusy = true;

                // Chama o serviço para atribuir o chamado no backend
                await _chamadoService.AtribuirChamadoAsync(chamado.Id);

                // Remove da lista de disponíveis
                ChamadosDisponiveis.Remove(chamado);

                // Atualiza o status e adiciona aos meus chamados
                chamado.Status = "EM_ANDAMENTO";
                MeusChamados.Add(chamado);

                await Shell.Current.DisplayAlert("Sucesso", "Chamado atribuído com sucesso!", "OK");

                // Muda automaticamente para a aba de "Em Andamento"
                SelectedTab = 1;
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await Shell.Current.DisplayAlert("Erro", "Este chamado já foi atribuído a outro técnico.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao atribuir chamado: {httpEx.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atribuir chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
