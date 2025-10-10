using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using CajuAjuda.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
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
        // CORREÇÃO FINAL: Usando strings em vez de nameof para compatibilidade com o gerador de código.
        [NotifyCanExecuteChangedFor("EnviarMensagemCommand")]
        [NotifyCanExecuteChangedFor("UpdateStatusCommand")]
        private bool _isBusy;

        [ObservableProperty] private string _titulo = "Carregando...";
        [ObservableProperty] private string _descricao = string.Empty;
        [ObservableProperty] private string _nomeCliente = string.Empty;
        [ObservableProperty] private string _status = string.Empty;
        [ObservableProperty] private string _prioridade = string.Empty;
        [ObservableProperty] private string _dataCriacao = string.Empty;
        [ObservableProperty] private ObservableCollection<Mensagem> _mensagens;

        [ObservableProperty]
        // CORREÇÃO FINAL: Usando strings em vez de nameof.
        [NotifyCanExecuteChangedFor("EnviarMensagemCommand")]
        private string _novaMensagem;

        public ObservableCollection<StatusChamado> StatusOptions { get; }

        [ObservableProperty]
        private StatusChamado _selectedStatus;

        public DetalheChamadoViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
            _mensagens = new ObservableCollection<Mensagem>();
            _novaMensagem = string.Empty;
            StatusOptions = new ObservableCollection<StatusChamado>(Enum.GetValues<StatusChamado>());
        }

        [RelayCommand]
        private async Task LoadDetalhesAsync()
        {
            if (IsBusy || ChamadoId == 0) return;
            IsBusy = true;
            try
            {
                var chamadoDetalhado = await _chamadoService.GetChamadoByIdAsync(ChamadoId);
                if (chamadoDetalhado != null)
                {
                    Titulo = chamadoDetalhado.Titulo;
                    Descricao = chamadoDetalhado.Descricao;
                    NomeCliente = chamadoDetalhado.NomeCliente;
                    Status = chamadoDetalhado.Status;
                    Prioridade = chamadoDetalhado.Prioridade;
                    DataCriacao = chamadoDetalhado.DataCriacao.ToString("g");

                    if (Enum.TryParse(chamadoDetalhado.Status, true, out StatusChamado statusAtual))
                    {
                        SelectedStatus = statusAtual;
                    }

                    Mensagens.Clear();
                    foreach (var msg in chamadoDetalhado.Mensagens)
                    {
                        Mensagens.Add(msg);
                    }
                }
                else { Titulo = "Chamado não encontrado"; }
            }
            catch (Exception ex)
            {
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os detalhes: {ex.Message}", "OK");
#pragma warning restore CA1416
            }
            finally { IsBusy = false; }
        }

        [RelayCommand(CanExecute = nameof(CanEnviarMensagem))]
        private async Task EnviarMensagemAsync()
        {
            var textoMensagem = NovaMensagem;
            var request = new MensagemCreateRequest { Texto = textoMensagem };
            NovaMensagem = string.Empty;

            var mensagemTemporaria = new Mensagem { Texto = textoMensagem, AutorNome = "Técnico (Enviando...)", DataEnvio = DateTime.Now };
            Mensagens.Add(mensagemTemporaria);

            try
            {
                var mensagemCriada = await _chamadoService.EnviarMensagemAsync(ChamadoId, request);
                if (mensagemCriada != null)
                {
                    mensagemTemporaria.AutorNome = mensagemCriada.AutorNome;
                    mensagemTemporaria.DataEnvio = mensagemCriada.DataEnvio;
                    mensagemTemporaria.Id = mensagemCriada.Id;
                }
            }
            catch (Exception ex)
            {
                Mensagens.Remove(mensagemTemporaria);
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível enviar a mensagem: {ex.Message}", "OK");
#pragma warning restore CA1416
            }
        }

        [RelayCommand(CanExecute = nameof(CanUpdateStatus))]
        private async Task UpdateStatusAsync()
        {
            IsBusy = true;
            try
            {
                await _chamadoService.UpdateStatusAsync(ChamadoId, SelectedStatus);
                Status = SelectedStatus.ToString();
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Sucesso!", "O status foi atualizado.", "OK");
#pragma warning restore CA1416
            }
            catch (Exception ex)
            {
#pragma warning disable CA1416
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível atualizar o status: {ex.Message}", "OK");
#pragma warning restore CA1416
            }
            finally { IsBusy = false; }
        }

        private bool CanEnviarMensagem() => !string.IsNullOrWhiteSpace(NovaMensagem) && !IsBusy;
        private bool CanUpdateStatus() => !IsBusy;
    }
}