using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using CajuAjuda.Desktop.Views;

namespace CajuAjuda.Desktop.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            string? token = null;
            try
            {
                token = await SecureStorage.Default.GetAsync("auth_token");
            }
            catch
            {
                // Ignora erro de SecureStorage
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Se 401, limpa token e redireciona para login
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.Remove("auth_token");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var loginPage = MauiProgram.Services?.GetService<LoginPage>();
                    if (loginPage != null && Microsoft.Maui.Controls.Application.Current != null)
                    {
                        Microsoft.Maui.Controls.Application.Current.MainPage = loginPage;
                    }
                });
            }

            return response;
        }
    }
}