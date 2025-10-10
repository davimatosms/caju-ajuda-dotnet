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
                    return JsonSerializer.Deserialize<LoginResponse>(content, _serializerOptions);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}