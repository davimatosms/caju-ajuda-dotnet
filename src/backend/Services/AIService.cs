using CajuAjuda.Backend.Models;
using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public class AIService : IAIService
{
    private readonly string _projectId;
    private readonly string _location;
    private readonly string _model;

    public AIService(IConfiguration configuration)
    {
        
        _projectId = configuration["Gemini:ProjectId"]!; 
        _location = "us-central1";
        _model = "gemini-1.5-flash-001";
    }

    public async Task<PrioridadeChamado> DefinirPrioridadeAsync(string titulo, string descricao)
    {
        
        var predictionServiceClient = await new PredictionServiceClientBuilder
        {
            Endpoint = $"{_location}-aiplatform.googleapis.com"
            
        }.BuildAsync();

        var endpointName = EndpointName.FromProjectLocationPublisherModel(_projectId, _location, "google", _model);

        var prompt = $@"
            Analise o seguinte ticket de suporte e classifique sua prioridade.
            Responda apenas com uma das seguintes palavras: BAIXA, MEDIA, ALTA, URGENTE.

            Exemplo 1:
            - Título: Dúvida sobre fatura
            - Descrição: Olá, gostaria de entender melhor o valor cobrado este mês.
            - Resposta: BAIXA

            Exemplo 2:
            - Título: Não consigo acessar minha conta
            - Descrição: A senha não funciona e o reset não chega no meu e-mail.
            - Resposta: MEDIA

            Exemplo 3:
            - Título: O sistema está fora do ar para todos os usuários!!!
            - Descrição: Ninguém na empresa consegue trabalhar, o site principal está caído.
            - Resposta: URGENTE

            ---
            Ticket para Análise:
            - Título: {titulo}
            - Descrição: {descricao}
            - Resposta: 
        ";

        
        var instance = new Google.Protobuf.WellKnownTypes.Value
        {
            StructValue = new Google.Protobuf.WellKnownTypes.Struct
            {
                Fields = { { "prompt", Google.Protobuf.WellKnownTypes.Value.ForString(prompt) } }
            }
        };

        var request = new PredictRequest
        {
            EndpointAsEndpointName = endpointName,
            Instances = { instance }
        };

        var response = await predictionServiceClient.PredictAsync(request);
        var predictionResult = response.Predictions[0].StructValue.Fields["content"].StringValue.Trim().ToUpper();

        return predictionResult switch
        {
            "MEDIA" => PrioridadeChamado.MEDIA,
            "ALTA" => PrioridadeChamado.ALTA,
            "URGENTE" => PrioridadeChamado.URGENTE,
            _ => PrioridadeChamado.BAIXA,
        };
    }
}