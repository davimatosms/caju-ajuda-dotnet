using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using CajuAjuda.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    [QueryProperty(nameof(ChamadoId), "ChamadoId")]
    public partial class DetalheChamadoViewModel : ObservableObject
    {
        private readonly ChamadoService _chamadoService;

        [ObservableProperty]
        private int _chamadoId;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EnviarMensagemCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateStatusCommand))]
        private bool _isBusy;

        [ObservableProperty]
        private string _titulo = "Carregando...";

        [ObservableProperty]
        private string _descricao = string.Empty;

        [ObservableProperty]
        private string _nomeCliente = string.Empty;

        [ObservableProperty]
        private string _status = string.Empty;

        [ObservableProperty]
        private string _prioridade = string.Empty;

        [ObservableProperty]
        private DateTime _dataCriacao;

        [ObservableProperty]
        private string? _nomeTecnicoResponsavel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EnviarMensagemCommand))]
        private string _novaMensagem = string.Empty;

        [ObservableProperty]
        private string _novoStatus = string.Empty;

        public ObservableCollection<Mensagem> Mensagens { get; } = new();

        // Lista de status disponíveis para o Picker
        public System.Collections.Generic.List<string> StatusDisponiveis { get; } = new()
        {
            "ABERTO",
            "EM_ANDAMENTO",
            "AGUARDANDO_CLIENTE",
            "RESOLVIDO",
            "FECHADO"
        };

        public DetalheChamadoViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        /// <summary>
        /// Carrega os detalhes do chamado incluindo mensagens
        /// </summary>
        [RelayCommand]
        public async Task LoadDetalhesAsync()
        {
            if (IsBusy || ChamadoId == 0) return;

            IsBusy = true;
            try
            {
                var detalhes = await _chamadoService.GetChamadoDetalhesAsync(ChamadoId);
                if (detalhes != null)
                {
                    Titulo = detalhes.Titulo;
                    Descricao = detalhes.Descricao;
                    NomeCliente = detalhes.NomeCliente;
                    Status = detalhes.Status;
                    Prioridade = detalhes.Prioridade;
                    DataCriacao = detalhes.DataCriacao;
                    NomeTecnicoResponsavel = detalhes.NomeTecnicoResponsavel;

                    // Atualiza mensagens
                    Mensagens.Clear();
                    if (detalhes.Mensagens != null)
                    {
                        foreach (var msg in detalhes.Mensagens.OrderBy(m => m.DataEnvio))
                        {
                            Mensagens.Add(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar detalhes: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Envia uma nova mensagem e adiciona imediatamente na lista (UI responsiva)
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSendMessage))]
        private async Task EnviarMensagemAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NovaMensagem)) return;

            IsBusy = true;

            // Salva o texto e limpa o campo imediatamente (UX responsiva)
            var textoMensagem = NovaMensagem;
            NovaMensagem = string.Empty;

            try
            {
                var request = new MensagemCreateRequest { Texto = textoMensagem };
                var mensagemCriada = await _chamadoService.EnviarMensagemAsync(ChamadoId, request);

                if (mensagemCriada != null)
                {
                    // Adiciona a mensagem criada à lista
                    Mensagens.Add(mensagemCriada);

                    // Opcional: Rola a lista para a última mensagem
                    await Shell.Current.DisplayAlert("Sucesso", "Mensagem enviada!", "OK");
                }
            }
            catch (Exception ex)
            {
                // Se falhar, restaura o texto para o usuário não perder
                NovaMensagem = textoMensagem;
                await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar mensagem: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSendMessage() => !IsBusy && !string.IsNullOrWhiteSpace(NovaMensagem);

        /// <summary>
        /// Atualiza o status do chamado
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanUpdateStatus))]
        private async Task UpdateStatusAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NovoStatus)) return;

            IsBusy = true;
            try
            {
                if (Enum.TryParse<StatusChamado>(NovoStatus, out var statusEnum))
                {
                    await _chamadoService.UpdateStatusChamadoAsync(ChamadoId, statusEnum);
                    await Shell.Current.DisplayAlert("Sucesso", "Status atualizado com sucesso!", "OK");
                    await LoadDetalhesAsync();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atualizar status: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanUpdateStatus() => !IsBusy && !string.IsNullOrWhiteSpace(NovoStatus);
    }
}
