using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
using CajuAjuda.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Desktop.Services
{       
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ChamadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task PrepareAuthenticatedClient()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
#pragma warning disable CA1416
                var token = await SecureStorage.GetAsync("auth_token");
#pragma warning restore CA1416

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    throw new Exception("Token de autenticação não encontrado.");
                }
            }
        }

        public async Task<List<Chamado>> GetChamadosAsync()
        {
            try
            {
                await PrepareAuthenticatedClient();
                var response = await _httpClient.GetAsync("api/Chamados");

                // Se 401, redireciona para login (já tratado no handler)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    SecureStorage.Default.Remove("auth_token");
                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var loginPage = MauiProgram.Services?.GetService<LoginPage>();
                        if (loginPage != null && Application.Current != null)
                        {
                            Application.Current.MainPage = loginPage;
                        }
                    });
                    
                    throw new HttpRequestException(
                        "Sessão expirada. Faça login novamente.", 
                        null, 
                        System.Net.HttpStatusCode.Unauthorized);
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Tenta deserializar diferentes formatos
                try
                {
                    var pagedResult = JsonSerializer.Deserialize<PagedList<Chamado>>(
                        content, _serializerOptions);
                    if (pagedResult?.Items != null && pagedResult.Items.Count > 0)
                        return pagedResult.Items;

                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        if (doc.RootElement.TryGetProperty("Items", out var itemsElem) && 
                            itemsElem.ValueKind == JsonValueKind.Array)
                        {
                            var items = JsonSerializer.Deserialize<List<Chamado>>(
                                itemsElem.GetRawText(), _serializerOptions);
                            return items ?? new List<Chamado>();
                        }

                        if (doc.RootElement.TryGetProperty("$values", out var valuesElem) && 
                            valuesElem.ValueKind == JsonValueKind.Array)
                        {
                            var items = JsonSerializer.Deserialize<List<Chamado>>(
                                valuesElem.GetRawText(), _serializerOptions);
                            return items ?? new List<Chamado>();
                        }
                    }

                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        var items = JsonSerializer.Deserialize<List<Chamado>>(
                            content, _serializerOptions);
                        return items ?? new List<Chamado>();
                    }
                }
                catch
                {
                    // Falha na deserialização
                }

                return new List<Chamado>();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw;
            }
        }

        public async Task<ChamadoDetalhes?> GetChamadoByIdAsync(int chamadoId)
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync($"api/Chamados/{chamadoId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var chamado = JsonSerializer.Deserialize<ChamadoDetalhes>(
                content, _serializerOptions);

            if (chamado != null && (chamado.Mensagens == null || chamado.Mensagens.Count == 0))
            {
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("mensagens", out var mensagensElem))
                    {
                        if (mensagensElem.ValueKind == JsonValueKind.Object && 
                            mensagensElem.TryGetProperty("$values", out var valuesElem) && 
                            valuesElem.ValueKind == JsonValueKind.Array)
                        {
                            var msgs = JsonSerializer.Deserialize<List<Mensagem>>(
                                valuesElem.GetRawText(), _serializerOptions);
                            chamado.Mensagens = msgs ?? new List<Mensagem>();
                        }
                        else if (mensagensElem.ValueKind == JsonValueKind.Array)
                        {
                            var msgs = JsonSerializer.Deserialize<List<Mensagem>>(
                                mensagensElem.GetRawText(), _serializerOptions);
                            chamado.Mensagens = msgs ?? new List<Mensagem>();
                        }
                    }
                }
                catch
                {
                    // Mantém lista vazia
                }
            }

            return chamado;
        }

        public async Task<Mensagem?> EnviarMensagemAsync(int chamadoId, MensagemCreateRequest request)
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsJsonAsync(
                $"api/Chamados/{chamadoId}/mensagens", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Mensagem>(content, _serializerOptions);
        }

        public async Task UpdateStatusAsync(int chamadoId, StatusChamado novoStatus)
        {
            await PrepareAuthenticatedClient();
            var requestUrl = $"api/Chamados/{chamadoId}/status";
            var updateStatusDto = new { Status = novoStatus };
            var response = await _httpClient.PutAsJsonAsync(requestUrl, updateStatusDto);
            response.EnsureSuccessStatusCode();
        }
    }

    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
    }
}