using CajuAjuda.Backend.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Backend.Services;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIService> _logger;

    
    private readonly string _model = "gemini-2.5-flash"; 
    private readonly string _apiVersion = "v1beta";
    private string? _apiKey = null;

    public AIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        
        _apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogError("!!!!!!!!!! CHAVE DE API DO GEMINI (Gemini:ApiKey) NÃO ENCONTRADA NA CONFIGURAÇÃO (secrets.json) !!!!!!!!!!");
            throw new InvalidOperationException("Chave de API do Gemini (Gemini:ApiKey) não está configurada.");
        }
    }

    public async Task<PrioridadeChamado> DefinirPrioridadeAsync(string titulo, string descricao)
    {
        _logger.LogInformation("Iniciando DefinirPrioridadeAsync para API Generative Language (Modelo: {Model}, Versão: {ApiVersion})", _model, _apiVersion);

        var httpClient = _httpClientFactory.CreateClient();
        var requestUrl = $"https://generativelanguage.googleapis.com/{_apiVersion}/models/{_model}:generateContent?key={_apiKey}";
        _logger.LogInformation("Endpoint Gemini: {EndpointUrl}", $"https://generativelanguage.googleapis.com/{_apiVersion}/models/{_model}:generateContent?key=...");

        var prompt = $@"Classifique a prioridade do chamado de suporte técnico abaixo em APENAS UMA palavra: BAIXA, MEDIA, ALTA ou URGENTE.

Título: {titulo}
Descrição: {descricao}

Responda APENAS com uma das quatro palavras (BAIXA, MEDIA, ALTA ou URGENTE):";

        var requestBodyObject = new
        {
            contents = new[] {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                candidateCount = 1,
                maxOutputTokens = 1024,
                temperature = 0.2
            },
            systemInstruction = new
            {
                parts = new[] {
                    new { text = "Você é um assistente de triagem. Responda APENAS com a prioridade, sem explicações." }
                }
            }
        };

        _logger.LogInformation("Enviando requisição para Gemini API...");

        var response = await httpClient.PostAsJsonAsync(requestUrl, requestBodyObject);
        _logger.LogInformation("Resposta recebida da Gemini API com Status Code: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Erro na chamada da API Gemini ({StatusCode}): {ErrorContent}", response.StatusCode, errorContent);
            throw new System.Exception($"Erro na API do Gemini: {response.StatusCode} - {errorContent}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Resposta JSON completa da Gemini API: {JsonResponse}", jsonResponse);

        string predictionResult = "BAIXA"; // Default
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var candidate = doc.RootElement.GetProperty("candidates")[0];

            // Verificação de segurança (como no log de erro anterior)
            if (candidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var partsElement) &&
                partsElement.GetArrayLength() > 0 &&
                partsElement[0].TryGetProperty("text", out var textElement))
            {
                predictionResult = textElement.GetString()?.Trim().ToUpper() ?? "BAIXA";
            }
            else
            {
                // Se 'parts' não existir (ex: MAX_TOKENS ou erro de segurança)
                string finishReason = candidate.TryGetProperty("finishReason", out var reasonElement)
                                      ? reasonElement.GetString() ?? "Desconhecido"
                                      : "Sem 'parts'";
                _logger.LogWarning("Resposta da Gemini API não continha 'parts'. Motivo: {FinishReason}. Resposta: {JsonResponse}", finishReason, jsonResponse);
                throw new Exception($"Falha ao extrair 'parts' da resposta da API Gemini. Motivo: {finishReason}");
            }
        }
        catch (Exception ex) when (ex is KeyNotFoundException || ex is IndexOutOfRangeException || ex is JsonException)
        {
            _logger.LogWarning(ex, "Estrutura da resposta da Gemini API inesperada. Resposta: {JsonResponse}", jsonResponse);
            throw new Exception("Falha ao parsear a resposta da API Gemini.", ex);
        }

        _logger.LogInformation("Predição de Prioridade recebida da Gemini API: {PredictionResult}", predictionResult);

        return predictionResult switch
        {
            "MEDIA" => PrioridadeChamado.MEDIA,
            "ALTA" => PrioridadeChamado.ALTA,
            "URGENTE" => PrioridadeChamado.URGENTE,
            _ => PrioridadeChamado.BAIXA, // Se a IA responder algo inválido, será BAIXA
        };
    }

    public async Task<string> SugerirSolucaoAsync(string titulo, string descricao, List<Mensagem>? historicoMensagens = null)
    {
        _logger.LogInformation("🤖 [AIService] Gerando sugestão de solução para chamado: {Titulo}", titulo);

        var httpClient = _httpClientFactory.CreateClient();
        var requestUrl = $"https://generativelanguage.googleapis.com/{_apiVersion}/models/{_model}:generateContent?key={_apiKey}";

        // Construir contexto com histórico de mensagens, se disponível
        var contextoHistorico = "";
        if (historicoMensagens != null && historicoMensagens.Any())
        {
            contextoHistorico = "\n\n📜 HISTÓRICO DA CONVERSA:\n";
            foreach (var msg in historicoMensagens.OrderBy(m => m.DataEnvio))
            {
                contextoHistorico += $"- [{msg.DataEnvio:HH:mm}] {msg.Autor?.Nome ?? "Sistema"}: {msg.Texto}\n";
            }
        }

        var prompt = $@"Analise o problema de suporte técnico abaixo e sugira 2-3 soluções práticas e diretas.{contextoHistorico}

📋 Título: {titulo}
📝 Descrição: {descricao}

💡 Forneça 2-3 sugestões numeradas e objetivas:";

        var requestBodyObject = new
        {
            contents = new[] {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                candidateCount = 1,
                maxOutputTokens = 2048,
                temperature = 0.7
            },
            systemInstruction = new
            {
                parts = new[] {
                    new { text = "Você é um assistente de suporte técnico. Seja objetivo e forneça soluções práticas." }
                }
            }
        };

        _logger.LogInformation("Enviando requisição para Gemini API (Sugestão de Solução)...");

        var response = await httpClient.PostAsJsonAsync(requestUrl, requestBodyObject);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("❌ Erro na chamada da API Gemini ({StatusCode}): {ErrorContent}", response.StatusCode, errorContent);
            return "Não foi possível gerar sugestões no momento. Um técnico analisará seu chamado em breve.";
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var candidate = doc.RootElement.GetProperty("candidates")[0];

            if (candidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var partsElement) &&
                partsElement.GetArrayLength() > 0 &&
                partsElement[0].TryGetProperty("text", out var textElement))
            {
                var sugestao = textElement.GetString()?.Trim() ?? "Sem sugestões disponíveis.";
                _logger.LogInformation("✅ Sugestão gerada com sucesso ({Length} caracteres)", sugestao.Length);
                return sugestao;
            }
            else
            {
                _logger.LogWarning("⚠️ Resposta da Gemini API não continha 'parts' para sugestão de solução");
                return "Não foi possível gerar sugestões no momento.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao parsear resposta da Gemini API para sugestão de solução");
            return "Erro ao processar sugestões. Um técnico analisará seu chamado.";
        }
    }

    public async Task<string> AnalisarContextoEResponderAsync(long chamadoId, string novaMensagem, List<Mensagem> historicoMensagens)
    {
        _logger.LogInformation("🧠 [AIService] Analisando contexto do chamado #{ChamadoId} com {Count} mensagens no histórico", 
            chamadoId, historicoMensagens.Count);

        var httpClient = _httpClientFactory.CreateClient();
        var requestUrl = $"https://generativelanguage.googleapis.com/{_apiVersion}/models/{_model}:generateContent?key={_apiKey}";

        // Construir histórico completo
        var contextoCompleto = "📜 HISTÓRICO COMPLETO DA CONVERSA:\n\n";
        foreach (var msg in historicoMensagens.OrderBy(m => m.DataEnvio))
        {
            var tipo = msg.Autor?.Role == Role.CLIENTE ? "👤 Cliente" : "🔧 Técnico";
            contextoCompleto += $"[{msg.DataEnvio:dd/MM HH:mm}] {tipo} ({msg.Autor?.Nome}): {msg.Texto}\n";
        }

        var prompt = $@"
            Você é um assistente técnico de suporte com memória contextual.
            Você tem acesso ao histórico completo da conversa e deve fornecer uma resposta útil considerando TODO o contexto.

            {contextoCompleto}

            💬 NOVA MENSAGEM DO CLIENTE:
            ""{novaMensagem}""

            📌 INSTRUÇÕES:
            - Analise TODO o histórico acima para entender o contexto completo
            - Se o cliente já mencionou algo antes, referencie isso
            - Se houver tentativas de solução anteriores, considere-as
            - Forneça uma resposta direta, útil e contextualizada
            - Se você perceber frustração ou problema recorrente, seja mais empático
            - Mantenha consistência com mensagens anteriores
            - Se já foi sugerida uma solução antes, pergunte se funcionou

            💡 RESPOSTA CONTEXTUALIZADA:";

        var requestBodyObject = new
        {
            contents = new[] {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                candidateCount = 1,
                maxOutputTokens = 1024,
                temperature = 0.8
            }
        };

        _logger.LogInformation("Enviando requisição para Gemini API (Análise Contextual)...");

        var response = await httpClient.PostAsJsonAsync(requestUrl, requestBodyObject);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("❌ Erro na chamada da API Gemini ({StatusCode}): {ErrorContent}", response.StatusCode, errorContent);
            return "Entendi sua mensagem. Um técnico responderá em breve com mais detalhes.";
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var candidate = doc.RootElement.GetProperty("candidates")[0];

            if (candidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var partsElement) &&
                partsElement.GetArrayLength() > 0 &&
                partsElement[0].TryGetProperty("text", out var textElement))
            {
                var resposta = textElement.GetString()?.Trim() ?? "Mensagem recebida. Analisando...";
                _logger.LogInformation("✅ Resposta contextual gerada com sucesso ({Length} caracteres)", resposta.Length);
                return resposta;
            }
            else
            {
                _logger.LogWarning("⚠️ Resposta da Gemini API não continha 'parts' para análise contextual");
                return "Entendi sua mensagem. Aguarde enquanto um técnico analisa.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao parsear resposta da Gemini API para análise contextual");
            return "Mensagem recebida. Um técnico responderá em breve.";
        }
    }
}