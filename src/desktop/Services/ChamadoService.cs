using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ChamadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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
            var statusDto = new { Status = novoStatus.ToString() };
            var response = await _httpClient.PutAsJsonAsync($"api/Chamados/{chamadoId}/status", statusDto);
            response.EnsureSuccessStatusCode();
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
    }
}
