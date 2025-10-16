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

        public async Task<List<Chamado>> GetMeusChamadosAsync()
        {
            var response = await _httpClient.GetAsync("api/Chamados/atribuidos");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Chamado>>(content, _serializerOptions) ?? new List<Chamado>();
        }

        public async Task<List<Chamado>> GetChamadosDisponiveisAsync()
        {
            var response = await _httpClient.GetAsync("api/Chamados/disponiveis");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Chamado>>(content, _serializerOptions) ?? new List<Chamado>();
        }

        public async Task<ChamadoDetalhes?> GetChamadoByIdAsync(int chamadoId)
        {
            var response = await _httpClient.GetAsync($"api/Chamados/{chamadoId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChamadoDetalhes>(content, _serializerOptions);
        }

        public async Task<Mensagem?> EnviarMensagemAsync(int chamadoId, MensagemCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Chamados/{chamadoId}/mensagens", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Mensagem>(content, _serializerOptions);
        }

        public async Task UpdateStatusAsync(int chamadoId, StatusChamado novoStatus)
        {
            var requestUrl = $"api/Chamados/{chamadoId}/status";
            var updateStatusDto = new { Status = novoStatus };
            var response = await _httpClient.PutAsJsonAsync(requestUrl, updateStatusDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignTicketAsync(int chamadoId)
        {
            var requestUrl = $"api/Chamados/{chamadoId}/assign";
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}