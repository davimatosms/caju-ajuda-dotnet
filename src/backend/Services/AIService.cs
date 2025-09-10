using CajuAjuda.Backend.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public class AIService : IAIService
{
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _model = "gemini-1.5-flash-latest"; 

    public AIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        // Lê a chave de API que configuramos no Secret Manager
        _apiKey = configuration["Gemini:ApiKey"]!;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PrioridadeChamado> DefinirPrioridadeAsync(string titulo, string descricao)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        // Este é o endpoint correto e mais simples da API Generative Language
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var prompt = $@"
            Analise o seguinte ticket de suporte e classifique sua prioridade.
            Responda apenas com uma das seguintes palavras: BAIXA, MEDIA, ALTA, URGENTE.

            Exemplo 1: - Título: Dúvida sobre fatura / Descrição: Gostaria de entender o valor cobrado. -> Resposta: BAIXA
            Exemplo 2: - Título: Não consigo acessar minha conta / Descrição: A senha não funciona. -> Resposta: MEDIA
            Exemplo 3: - Título: O sistema está fora do ar!!! / Descrição: Ninguém consegue trabalhar. -> Resposta: URGENTE
            ---
            Ticket para Análise: - Título: {titulo} / Descrição: {descricao} -> Resposta: 
        ";
        
        // A estrutura do corpo da requisição para esta API
        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var jsonBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(requestUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"Erro na API do Gemini: {response.StatusCode} - {errorContent}");
        }

        // A estrutura da resposta desta API
        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);
        var predictionResult = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "BAIXA";
        predictionResult = predictionResult.Trim().ToUpper();

        return predictionResult switch
        {
            "MEDIA" => PrioridadeChamado.MEDIA,
            "ALTA" => PrioridadeChamado.ALTA,
            "URGENTE" => PrioridadeChamado.URGENTE,
            _ => PrioridadeChamado.BAIXA,
        };
    }
}