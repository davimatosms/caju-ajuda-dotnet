// CajuAjuda.Desktop/Services/ChamadoService.cs

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Models;

namespace CajuAjuda.Desktop.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7113";

        public ChamadoService()
        {
            // Bypass de SSL para ambiente de desenvolvimento
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, e) => true };
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<Chamado>> GetChamadosAsync()
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token");
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Usuário não autenticado.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Chamados");

            if (response.IsSuccessStatusCode)
            {
                var pagedResponse = await response.Content.ReadFromJsonAsync<PagedList<Chamado>>();
                return pagedResponse?.Items ?? new List<Chamado>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent ?? "Erro ao buscar os chamados.");
            }
        }

        public async Task<ChamadoDetail?> GetChamadoByIdAsync(int chamadoId)
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token");
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Usuário não autenticado.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/api/Chamados/{chamadoId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChamadoDetail>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent ?? $"Erro ao buscar o chamado ID {chamadoId}.");
            }
        }

        public async Task<Mensagem?> EnviarMensagemAsync(int chamadoId, MensagemCreateRequest request)
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token");
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Usuário não autenticado.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync($"/api/Chamados/{chamadoId}/mensagens", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Mensagem>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent ?? "Erro ao enviar a mensagem.");
            }
        }
    }
}