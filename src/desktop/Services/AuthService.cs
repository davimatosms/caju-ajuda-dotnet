// CajuAjuda.Desktop/Services/AuthService.cs
// VERSÃO COMPLETA E ATUALIZADA

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CajuAjuda.Desktop.Models;

namespace CajuAjuda.Desktop.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // URL base da sua API, que descobrimos ser esta:
        private const string BaseUrl = "https://localhost:7113";

        public AuthService()
        {
            // ======================================================================================
            // ATENÇÃO: O CÓDIGO ABAIXO É APENAS PARA FINS DE DEPURAÇÃO LOCAL.
            // Ele desabilita a verificação do certificado SSL e NUNCA deve ser usado em produção.
            // ======================================================================================
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
            // ======================================================================================

            _httpClient.BaseAddress = new System.Uri(BaseUrl);
        }

        /// <summary>
        /// Envia as credenciais de login para a API.
        /// </summary>
        /// <param name="loginRequest">O objeto contendo email e senha.</param>
        /// <returns>A resposta da API contendo o token JWT.</returns>
        /// <exception cref="System.Exception">Lançada quando a API retorna um erro.</exception>
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            // Endpoint correto para a autenticação de login
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return loginResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new System.Exception(errorContent ?? "Erro ao fazer login. Verifique suas credenciais.");
            }
        }
    }
}