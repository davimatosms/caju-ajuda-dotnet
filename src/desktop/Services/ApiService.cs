using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        public ApiService(HttpClient client) => _client = client;

        public async Task<T?> GetAsync<T>(string route)
        {
            var res = await _client.GetAsync(route);
            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                // ação: logout, limpar token e notificar UI
                throw new HttpRequestException("Unauthorized: token ausente ou inválido", null, res.StatusCode);
            }
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<T?>();
        }
    }
}