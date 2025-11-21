using CajuAjuda.Desktop.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<LoginResponse?> LoginAsync(string email, string senha)
        {
            var loginRequest = new { Email = email, Senha = senha };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, _serializerOptions);
                    
                    // 🔥 CRITICAL: Salva o token no SecureStorage
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("user_token", loginResponse.Token);
                        System.Diagnostics.Debug.WriteLine($"[AuthService] ✅ Token salvo no SecureStorage");
                    }
                    
                    return loginResponse;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] ❌ Erro no login: {ex.Message}");
                return null;
            }
        }
    }
}