using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Data;

public class DataInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DataInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CajuAjudaDbContext>();

        // Aplica quaisquer migrações pendentes para garantir que o BD esteja atualizado
        await context.Database.MigrateAsync();

        // Verifica se já existem usuários para não popular novamente
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // --- CRIAÇÃO DE USUÁRIOS ---
        var admin = new Usuario { Nome = "Admin Caju", Email = "admin@cajuajuda.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.ADMIN, Enabled = true };
        var tecnico = new Usuario { Nome = "Tecnico Caju", Email = "tecnico@cajuajuda.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.TECNICO, Enabled = true };
        var aiAssistant = new Usuario { Nome = "Assistente IA Caju", Email = "ia@cajuajuda.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.ADMIN, Enabled = true };
        var cliente1 = new Usuario { Nome = "Ana Cliente", Email = "ana.cliente@email.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.CLIENTE, Enabled = true };
        var cliente2 = new Usuario { Nome = "Beto Cliente", Email = "beto.cliente@email.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.CLIENTE, Enabled = true };
        var clienteInativo = new Usuario { Nome = "Carlos Inativo", Email = "carlos.inativo@email.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.CLIENTE, Enabled = false };

        await context.Usuarios.AddRangeAsync(admin, tecnico, aiAssistant, cliente1, cliente2, clienteInativo);
        await context.SaveChangesAsync(); // Salva para obter os IDs

        // --- CRIAÇÃO DE CHAMADOS ---
        var chamadoAberto = new Chamado
        {
            Titulo = "Problema Crítico: Sistema de pagamentos fora do ar",
            Descricao = "Nenhum cliente consegue finalizar a compra. Erro 500 em todas as transações.",
            Status = StatusChamado.ABERTO,
            Prioridade = PrioridadeChamado.ALTA,
            ClienteId = cliente1.Id
        };

        var chamadoEmAndamento = new Chamado
        {
            Titulo = "Lentidão ao gerar relatórios",
            Descricao = "O relatório de vendas mensais está demorando mais de 5 minutos para ser gerado.",
            Status = StatusChamado.EM_ANDAMENTO,
            Prioridade = PrioridadeChamado.MEDIA,
            ClienteId = cliente2.Id,
            TecnicoResponsavelId = tecnico.Id // Chamado já atribuído
        };

        var chamadoFechado = new Chamado
        {
            Titulo = "Dúvida sobre a cor de um botão",
            Descricao = "Gostaria de saber se é possível alterar a cor do botão 'Salvar' para azul.",
            Status = StatusChamado.FECHADO,
            Prioridade = PrioridadeChamado.BAIXA,
            ClienteId = cliente1.Id,
            TecnicoResponsavelId = tecnico.Id,
            DataFechamento = DateTime.UtcNow.AddDays(-5)
        };

        await context.Chamados.AddRangeAsync(chamadoAberto, chamadoEmAndamento, chamadoFechado);
        await context.SaveChangesAsync(); // Salva para obter os IDs

        // --- CRIAÇÃO DE MENSAGENS ---
        var mensagens = new List<Mensagem>
        {
            new() { Texto = "Olá Beto, recebi seu chamado sobre a lentidão. Estou investigando a causa.", ChamadoId = chamadoEmAndamento.Id, AutorId = tecnico.Id, LidoPeloCliente = false },
            new() { Texto = "Obrigado pelo retorno! Fico no aguardo.", ChamadoId = chamadoEmAndamento.Id, AutorId = cliente2.Id, LidoPeloCliente = true, IsNotaInterna = false},
            new() { Texto = "NOTA: Verificar os índices da tabela de vendas. Pode ser a causa da lentidão.", ChamadoId = chamadoEmAndamento.Id, AutorId = tecnico.Id, IsNotaInterna = true}
        };

        await context.Mensagens.AddRangeAsync(mensagens);

        // --- CRIAÇÃO DE RESPOSTAS PRONTAS ---
        if (!await context.RespostasProntas.AnyAsync())
        {
            var respostas = new List<RespostaPronta>
            {
                new() { Titulo = "Saudação Inicial", Corpo = "Olá! Agradecemos o seu contato. Meu nome é [SEU NOME] e vou te ajudar com o seu chamado." },
                new() { Titulo = "Reset de Senha", Corpo = "Para redefinir sua senha, por favor, acesse o link a seguir e siga as instruções: [LINK]" },
                new() { Titulo = "Aguardando Informações", Corpo = "Olá! Para prosseguir com o atendimento, preciso de mais algumas informações. Você poderia me fornecer [INFORMAÇÃO NECESSÁRIA]?" },
                new() { Titulo = "Encerramento", Corpo = "Fico feliz em ajudar! Estou encerrando este chamado. Se precisar de mais alguma coisa, basta abrir um novo ticket. Tenha um ótimo dia!" }
            };
            await context.RespostasProntas.AddRangeAsync(respostas);
        }
    }
}