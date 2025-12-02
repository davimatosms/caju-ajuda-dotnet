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
                System.Diagnostics.Debug.WriteLine($"[AuthService] Enviando requisição para: {_httpClient.BaseAddress}api/auth/login");
                System.Diagnostics.Debug.WriteLine($"[AuthService] Email: {email}");
                
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
                
                System.Diagnostics.Debug.WriteLine($"[AuthService] Status Code: {response.StatusCode}");
                
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[AuthService] Response Content: {content}");
                
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, _serializerOptions);
                    
                    // 🔥 CRITICAL: Salva o token no SecureStorage
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("user_token", loginResponse.Token);
                        System.Diagnostics.Debug.WriteLine($"[AuthService] ✅ Token salvo no SecureStorage");
                    }
                    
                    return loginResponse;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[AuthService] ❌ Erro HTTP: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"[AuthService] Resposta do servidor: {content}");
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] ❌ HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] StackTrace: {ex.StackTrace}");
                throw; // Re-throw para ser capturado no ViewModel
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] ❌ Erro no login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}