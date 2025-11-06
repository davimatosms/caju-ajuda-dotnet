using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using CajuAjuda.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.ViewModels
{
    [QueryProperty(nameof(ChamadoId), "ChamadoId")]
    public partial class DetalheChamadoViewModel : ObservableObject
    {
        private readonly ChamadoService _chamadoService;
        private System.Threading.Timer? _autoRefreshTimer;

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
        [NotifyCanExecuteChangedFor(nameof(UpdateStatusCommand))] // ✅ Adicionado para notificar o comando
        private string _novoStatus = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing = false;

        public ObservableCollection<Mensagem> Mensagens { get; } = new();
        public ObservableCollection<Anexo> Anexos { get; } = new();

        // Lista de status disponíveis para o Picker
        public System.Collections.Generic.List<string> StatusDisponiveis { get; } = new()
        {
            "ABERTO",
            "EM_ANDAMENTO",
            "FECHADO"
        };

        public DetalheChamadoViewModel(ChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        /// <summary>
        /// Carrega os detalhes do chamado incluindo mensagens e anexos
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

                    // Atualiza anexos
                    Anexos.Clear();
                    if (detalhes.Anexos != null)
                    {
                        foreach (var anexo in detalhes.Anexos)
                        {
                            Anexos.Add(anexo);
                        }
                        Debug.WriteLine($"[ViewModel] 📎 {Anexos.Count} anexo(s) carregado(s)");
                    }

                    // 🔄 Inicia auto-refresh das mensagens
                    StartAutoRefresh();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewModel] ❌ Erro ao carregar detalhes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Envia uma nova mensagem SEM BLOQUEAR A UI (UX fluida)
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSendMessage))]
        private async Task EnviarMensagemAsync()
        {
            if (string.IsNullOrWhiteSpace(NovaMensagem)) return;

            // Salva o texto e limpa o campo IMEDIATAMENTE (UX responsiva)
            var textoMensagem = NovaMensagem;
            NovaMensagem = string.Empty;

            try
            {
                var request = new MensagemCreateRequest { Texto = textoMensagem };
                
                // Envia em background SEM bloquear a UI
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var mensagemCriada = await _chamadoService.EnviarMensagemAsync(ChamadoId, request);
                        Debug.WriteLine($"[ViewModel] ✅ Mensagem #{mensagemCriada?.Id} enviada - SignalR irá atualizar a UI");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ViewModel] ❌ Erro ao enviar mensagem: {ex.Message}");
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.DisplayAlert("Erro", $"Erro ao enviar: {ex.Message}", "OK");
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                // Se falhar imediatamente, restaura o texto
                NovaMensagem = textoMensagem;
                await Shell.Current.DisplayAlert("Erro", $"Erro: {ex.Message}", "OK");
            }
        }

        private bool CanSendMessage() => !IsBusy && !string.IsNullOrWhiteSpace(NovaMensagem);

        /// <summary>
        /// Faz download de um anexo
        /// </summary>
        [RelayCommand]
        private async Task DownloadAnexoAsync(Anexo anexo)
        {
            try
            {
                Debug.WriteLine($"[ViewModel] 📥 Baixando anexo: {anexo.NomeArquivo} (ID: {anexo.Id})");
                
                var bytes = await _chamadoService.DownloadAnexoAsync(anexo.Id);
                
                if (bytes != null && bytes.Length > 0)
                {
                    // Abre o seletor de pasta para salvar o arquivo
                    var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var filePath = Path.Combine(downloadsPath, anexo.NomeArquivo);
                    
                    await File.WriteAllBytesAsync(filePath, bytes);
                    await Shell.Current.DisplayAlert("Sucesso", $"Arquivo salvo em: {filePath}", "OK");
                    Debug.WriteLine($"[ViewModel] ✅ Arquivo salvo em: {filePath}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível baixar o arquivo", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewModel] ❌ Erro ao baixar anexo: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Erro ao baixar: {ex.Message}", "OK");
            }
        }

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
                Debug.WriteLine($"[ViewModel] 🔄 Atualizando status do chamado {ChamadoId} para {NovoStatus}");
                
                if (Enum.TryParse<StatusChamado>(NovoStatus, out var statusEnum))
                {
                    await _chamadoService.UpdateStatusChamadoAsync(ChamadoId, statusEnum);
                    await Shell.Current.DisplayAlert("Sucesso", "Status atualizado com sucesso!", "OK");
                    
                    // Atualiza os detalhes do chamado
                    await LoadDetalhesAsync();
                    
                    // Limpa o picker
                    NovoStatus = string.Empty;
                    
                    Debug.WriteLine($"[ViewModel] ✅ Status atualizado com sucesso");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Status inválido selecionado", "OK");
                    Debug.WriteLine($"[ViewModel] ❌ Status inválido: {NovoStatus}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewModel] ❌ Erro ao atualizar status: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atualizar status: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanUpdateStatus() => !IsBusy && !string.IsNullOrWhiteSpace(NovoStatus);

        /// <summary>
        /// Inicia timer para atualizar mensagens automaticamente a cada 3 segundos
        /// </summary>
        private void StartAutoRefresh()
        {
            // Para o timer anterior se existir
            StopAutoRefresh();

            Debug.WriteLine("[ViewModel] 🔄 Iniciando auto-refresh (3 segundos)");

            _autoRefreshTimer = new System.Threading.Timer(async _ =>
            {
                await RefreshMensagensAsync();
            }, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        /// <summary>
        /// Para o auto-refresh
        /// </summary>
        private void StopAutoRefresh()
        {
            _autoRefreshTimer?.Dispose();
            _autoRefreshTimer = null;
            Debug.WriteLine("[ViewModel] ⏸️ Auto-refresh parado");
        }

        /// <summary>
        /// Busca novas mensagens via REST API
        /// </summary>
        private async Task RefreshMensagensAsync()
        {
            if (IsRefreshing || ChamadoId == 0) return;

            try
            {
                IsRefreshing = true;
                Debug.WriteLine($"[ViewModel] � Buscando novas mensagens...");

                var detalhes = await _chamadoService.GetChamadoDetalhesAsync(ChamadoId);
                
                if (detalhes?.Mensagens != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Adiciona apenas mensagens novas
                        foreach (var msg in detalhes.Mensagens.OrderBy(m => m.DataEnvio))
                        {
                            if (!Mensagens.Any(m => m.Id == msg.Id))
                            {
                                Debug.WriteLine($"[ViewModel] ➕ Nova mensagem #{msg.Id} adicionada");
                                Mensagens.Add(msg);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewModel] ❌ Erro ao atualizar mensagens: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// Cleanup ao sair da tela
        /// </summary>
        public Task OnDisappearingAsync()
        {
            Debug.WriteLine($"[ViewModel] 🚪 Saindo da tela do chamado #{ChamadoId}");
            StopAutoRefresh();
            return Task.CompletedTask;
        }
    }
}