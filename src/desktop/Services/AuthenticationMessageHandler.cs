using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics; // Adicionar esta referência

namespace CajuAjuda.Desktop.Services
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"[Handler] Interceptando requisição para: {request.RequestUri}");

            var token = await SecureStorage.GetAsync("auth_token");

            // LINHAS DE DEBUG: Vamos ver se o token está sendo lido corretamente.
            if (!string.IsNullOrEmpty(token))
            {
                Debug.WriteLine($"[Handler] Token LIDO do Storage. Valor: {token.Substring(0, 20)}...");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine("[Handler] Cabeçalho de Autorização FOI ADICIONADO.");
            }
            else
            {
                Debug.WriteLine("[Handler] Token NÃO ENCONTRADO no Storage. A requisição seguirá sem autorização.");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}