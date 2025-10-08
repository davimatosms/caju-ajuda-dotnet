using CajuAjuda.Desktop.Models;
using CajuAjuda.Desktop.Models.Enums;
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
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    throw new Exception("Token de autenticação não encontrado.");
                }
            }
        }

        // ======================================================
        //          NOVO MÉTODO ADICIONADO E COMPLETO
        // ======================================================
        public async Task<List<Chamado>> GetChamadosAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync("api/Chamados");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            // A API pode retornar vários formatos: um objeto paginado com 'Items',
            // um objeto com '$values' (serialização EF), ou um array direto.
            try
            {
                // Tenta desserializar como { Items: [...] }
                var pagedResult = JsonSerializer.Deserialize<PagedList<Chamado>>(content, _serializerOptions);
                if (pagedResult?.Items != null && pagedResult.Items.Count > 0)
                    return pagedResult.Items;

                // Tenta detectar { $values: [...] }
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("Items", out var itemsElem) && itemsElem.ValueKind == JsonValueKind.Array)
                    {
                        var itemsJson = itemsElem.GetRawText();
                        var items = JsonSerializer.Deserialize<List<Chamado>>(itemsJson, _serializerOptions);
                        return items ?? new List<Chamado>();
                    }

                    if (doc.RootElement.TryGetProperty("$values", out var valuesElem) && valuesElem.ValueKind == JsonValueKind.Array)
                    {
                        var valuesJson = valuesElem.GetRawText();
                        var items = JsonSerializer.Deserialize<List<Chamado>>(valuesJson, _serializerOptions);
                        return items ?? new List<Chamado>();
                    }
                }

                // Caso seja um array direto
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var items = JsonSerializer.Deserialize<List<Chamado>>(content, _serializerOptions);
                    return items ?? new List<Chamado>();
                }
            }
            catch
            {
                // Em caso de falha na normalização, cai para retorno vazio
            }

            return new List<Chamado>();
        }

        public async Task<ChamadoDetalhes?> GetChamadoByIdAsync(int chamadoId)
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync($"api/Chamados/{chamadoId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var chamado = JsonSerializer.Deserialize<ChamadoDetalhes>(content, _serializerOptions);

            if (chamado != null)
            {
                // Normaliza formatos: a API pode retornar "mensagens" como um array direto
                // ou como um objeto contendo "$values" (serialização EF). Tentamos
                // consertar ambos os casos para garantir que Mensagens nunca fique nulo.
                if (chamado.Mensagens == null || chamado.Mensagens.Count == 0)
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(content);
                        if (doc.RootElement.TryGetProperty("mensagens", out var mensagensElem))
                        {
                            if (mensagensElem.ValueKind == JsonValueKind.Object && mensagensElem.TryGetProperty("$values", out var valuesElem) && valuesElem.ValueKind == JsonValueKind.Array)
                            {
                                var msgsJson = valuesElem.GetRawText();
                                var msgs = JsonSerializer.Deserialize<List<Mensagem>>(msgsJson, _serializerOptions);
                                chamado.Mensagens = msgs ?? new List<Mensagem>();
                            }
                            else if (mensagensElem.ValueKind == JsonValueKind.Array)
                            {
                                var msgsJson = mensagensElem.GetRawText();
                                var msgs = JsonSerializer.Deserialize<List<Mensagem>>(msgsJson, _serializerOptions);
                                chamado.Mensagens = msgs ?? new List<Mensagem>();
                            }
                        }
                    }
                    catch
                    {
                        // Se falhar o parse, mantemos a lista vazia já inicializada no model.
                    }
                }
            }

            return chamado;
        }

        public async Task<Mensagem?> EnviarMensagemAsync(int chamadoId, MensagemCreateRequest request)
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.PostAsJsonAsync($"api/Chamados/{chamadoId}/mensagens", request);
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

    // Classe auxiliar para deserializar a resposta paginada da API
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
    }
}