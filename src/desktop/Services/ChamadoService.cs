// CajuAjuda.Desktop/Services/ChamadoService.cs

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Models;

namespace CajuAjuda.Desktop.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7113"; // A mesma URL do AuthService

        public ChamadoService()
        {
            // Reutilizamos a mesma lógica de bypass de SSL para desenvolvimento
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (m, c, ch, e) => true };
            _httpClient = new HttpClient(handler) { BaseAddress = new System.Uri(BaseUrl) };
        }

        public async Task<List<Chamado>> GetChamadosAsync()
        {
            // 1. Recupera o token do armazenamento seguro.
            var token = await SecureStorage.Default.GetAsync("jwt_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new System.Exception("Usuário não autenticado.");
            }

            // 2. Adiciona o token ao cabeçalho de autorização da requisição.
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 3. Faz a chamada GET para o endpoint que retorna os chamados.
            //    ATENÇÃO: "/api/chamados" é um exemplo. Use o endpoint correto da sua API!
            var response = await _httpClient.GetAsync("/api/chamados");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Chamado>>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new System.Exception(errorContent ?? "Erro ao buscar os chamados.");
            }
        }
    }
}