using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CajuAjuda.Desktop.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ChamadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                // Importante: serializa enums como strings
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// Obtém todos os chamados atribuídos ao técnico logado
        /// </summary>
        public async Task<List<Chamado>> GetMeusChamadosAsync()
        {
            var response = await _httpClient.GetAsync("api/Chamados/atribuidos");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Chamado>>(content, _serializerOptions) ?? new List<Chamado>();
        }

        /// <summary>
        /// Obtém todos os chamados disponíveis (sem técnico atribuído)
        /// </summary>
        public async Task<List<Chamado>> GetChamadosDisponiveisAsync()
        {
            var response = await _httpClient.GetAsync("api/Chamados/disponiveis");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Chamado>>(content, _serializerOptions) ?? new List<Chamado>();
        }

        /// <summary>
        /// Obtém os detalhes completos de um chamado específico (incluindo mensagens)
        /// </summary>
        public async Task<ChamadoDetalhes?> GetChamadoDetalhesAsync(int chamadoId)
        {
            var response = await _httpClient.GetAsync($"api/Chamados/{chamadoId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChamadoDetalhes>(content, _serializerOptions);
        }

        /// <summary>
        /// Atribui um chamado disponível ao técnico logado
        /// </summary>
        public async Task AtribuirChamadoAsync(int chamadoId)
        {
            var response = await _httpClient.PostAsync($"api/Chamados/{chamadoId}/atribuir", null);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Atualiza o status de um chamado
        /// </summary>
        public async Task UpdateStatusChamadoAsync(int chamadoId, StatusChamado novoStatus)
        {
            try
            {
                // ✅ CORRIGIDO: Cria um DTO tipado com o enum diretamente
                var statusDto = new UpdateStatusRequest { Status = novoStatus };
                
                Debug.WriteLine($"[ChamadoService] 🔄 Atualizando status do chamado {chamadoId} para {novoStatus}");
                
                var response = await _httpClient.PutAsJsonAsync($"api/Chamados/{chamadoId}/status", statusDto, _serializerOptions);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[ChamadoService] ❌ Erro ao atualizar status: {response.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"Erro ao atualizar status: {response.StatusCode} - {errorContent}");
                }
                
                Debug.WriteLine($"[ChamadoService] ✅ Status atualizado com sucesso");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ChamadoService] ❌ Exceção ao atualizar status: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Envia uma nova mensagem em um chamado e retorna a mensagem criada
        /// </summary>
        public async Task<Mensagem?> EnviarMensagemAsync(int chamadoId, MensagemCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Chamados/{chamadoId}/mensagens", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Mensagem>(content, _serializerOptions);
        }

        /// <summary>
        /// Faz download de um anexo
        /// </summary>
        public async Task<byte[]?> DownloadAnexoAsync(long anexoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Chamados/anexos/{anexoId}/download");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    // ✅ DTO tipado para atualização de status
    public class UpdateStatusRequest
    {
        public StatusChamado Status { get; set; }
    }
}
