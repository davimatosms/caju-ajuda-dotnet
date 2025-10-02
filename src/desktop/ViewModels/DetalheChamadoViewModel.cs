// CajuAjuda.Desktop/ViewModels/DetalheChamadoViewModel.cs

using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
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
        [NotifyCanExecuteChangedFor(nameof(EnviarMensagemCommand))]
        private bool _isBusy;

        // Propriedades individuais para a View
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
        private string _dataCriacao = string.Empty;
        [ObservableProperty]
        private ObservableCollection<Mensagem> _mensagens;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EnviarMensagemCommand))]
        private string _novaMensagem;

        public DetalheChamadoViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
            _mensagens = new ObservableCollection<Mensagem>();
            _novaMensagem = string.Empty;
        }

        // Este método foi removido na versão anterior, a carga é iniciada pelo OnNavigatedTo na View.
        // partial void OnChamadoIdChanged(int value) { ... }

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

                    Mensagens.Clear();
                    foreach (var msg in chamadoDetalhado.Mensagens)
                    {
                        Mensagens.Add(msg);
                    }
                }
                else
                {
                    Titulo = "Chamado não encontrado";
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os detalhes do chamado: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanEnviarMensagem))]
        private async Task EnviarMensagemAsync()
        {
            var textoMensagem = NovaMensagem;
            var request = new MensagemCreateRequest { Texto = textoMensagem };

            // Limpa a caixa de texto imediatamente para o usuário
            NovaMensagem = string.Empty;

            // Adiciona a mensagem à lista da UI de forma otimista
            var mensagemTemporaria = new Mensagem
            {
                Texto = textoMensagem,
                AutorNome = "Técnico (Enviando...)",
                DataEnvio = DateTime.Now
            };
            Mensagens.Add(mensagemTemporaria);

            try
            {
                // Envia para a API em segundo plano
                var mensagemCriada = await _chamadoService.EnviarMensagemAsync(ChamadoId, request);

                // Quando a API responde, atualizamos a mensagem temporária com os dados reais
                if (mensagemCriada != null)
                {
                    mensagemTemporaria.AutorNome = mensagemCriada.AutorNome;
                    mensagemTemporaria.DataEnvio = mensagemCriada.DataEnvio;
                    mensagemTemporaria.Id = mensagemCriada.Id;
                }
                else
                {
                    // Se a API não retornou a mensagem, removemos a temporária e avisamos
                    Mensagens.Remove(mensagemTemporaria);
                    await Shell.Current.DisplayAlert("Aviso", "A mensagem foi enviada, mas não foi possível confirmar a atualização.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Se deu erro, removemos a mensagem que adicionamos e mostramos o alerta
                Mensagens.Remove(mensagemTemporaria);
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível enviar a mensagem: {ex.Message}", "OK");
            }
        }

        private bool CanEnviarMensagem() => !string.IsNullOrWhiteSpace(NovaMensagem) && !IsBusy;
    }
}