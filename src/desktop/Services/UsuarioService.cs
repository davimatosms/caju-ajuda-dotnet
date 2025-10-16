using CajuAjuda.Desktop.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateSenhaAsync(SenhaUpdateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Usuarios/update-senha", dto);

            if (!response.IsSuccessStatusCode)
            {
                // Tenta ler a mensagem de erro da API
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(errorContent);
            }
        }
    }
}