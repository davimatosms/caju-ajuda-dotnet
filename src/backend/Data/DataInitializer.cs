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

        Console.WriteLine("[SEED] Verificando usuários essenciais do sistema...");

        // --- GARANTIR ADMIN PADRÃO ---
        var adminEmail = "admin@cajuajuda.com";
        var adminExistente = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == adminEmail);
        
        if (adminExistente == null)
        {
            var admin = new Usuario 
            { 
                Nome = "Admin Caju", 
                Email = adminEmail, 
                Senha = BCrypt.Net.BCrypt.HashPassword("Admin@2025"), 
                Role = Role.ADMIN, 
                Enabled = true,
                VerificationToken = null
            };
            await context.Usuarios.AddAsync(admin);
            await context.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ Admin criado: {adminEmail} | Senha: Admin@2025");
        }
        else
        {
            Console.WriteLine($"[SEED] ℹ️  Admin já existe: {adminEmail}");
        }

        // --- GARANTIR IA ASSISTENTE ---
        var iaEmail = "ia@cajuajuda.com";
        var iaExistente = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == iaEmail);
        
        if (iaExistente == null)
        {
            var aiAssistant = new Usuario 
            { 
                Nome = "🤖 Assistente IA Caju", 
                Email = iaEmail, 
                Senha = BCrypt.Net.BCrypt.HashPassword("IA@2025SecurePassword"), 
                Role = Role.ADMIN, 
                Enabled = true,
                VerificationToken = null
            };
            await context.Usuarios.AddAsync(aiAssistant);
            await context.SaveChangesAsync();
            Console.WriteLine($"[SEED] ✅ IA Assistente criado: {iaEmail}");
        }
        else
        {
            Console.WriteLine($"[SEED] ℹ️  IA Assistente já existe: {iaEmail}");
        }

        // --- POPULAR DADOS DE EXEMPLO (apenas se não existir NENHUM usuário além dos essenciais) ---
        var totalUsuarios = await context.Usuarios.CountAsync();
        
        if (totalUsuarios <= 2) // Apenas Admin e IA existem
        {
            Console.WriteLine("[SEED] Criando dados de exemplo (técnicos, clientes, chamados)...");
            await SeedExampleDataAsync(context);
        }
        else
        {
            Console.WriteLine($"[SEED] ℹ️  Sistema já possui {totalUsuarios} usuários. Dados de exemplo não serão criados.");
        }

        Console.WriteLine("[SEED] Inicialização concluída!");
    }

    private async Task SeedExampleDataAsync(CajuAjudaDbContext context)
    {
        // --- CRIAÇÃO DE USUÁRIOS DE EXEMPLO ---
        var tecnico = new Usuario { Nome = "Técnico Caju", Email = "tecnico@cajuajuda.com", Senha = BCrypt.Net.BCrypt.HashPassword("Tecnico@2025"), Role = Role.TECNICO, Enabled = true, VerificationToken = null };
        var cliente1 = new Usuario { Nome = "Ana Cliente", Email = "ana.cliente@email.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.CLIENTE, Enabled = true, VerificationToken = null };
        var cliente2 = new Usuario { Nome = "Beto Cliente", Email = "beto.cliente@email.com", Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), Role = Role.CLIENTE, Enabled = true, VerificationToken = null };

        await context.Usuarios.AddRangeAsync(tecnico, cliente1, cliente2);
        await context.SaveChangesAsync(); // Salva para obter os IDs
        
        Console.WriteLine("[SEED] ✅ Técnico criado: tecnico@cajuajuda.com | Senha: Tecnico@2025");
        Console.WriteLine("[SEED] ✅ Clientes de exemplo criados");

        // --- CRIAÇÃO DE CHAMADOS DE EXEMPLO ---
        Console.WriteLine("[SEED] ✅ Técnico criado: tecnico@cajuajuda.com | Senha: Tecnico@2025");
        Console.WriteLine("[SEED] ✅ Clientes de exemplo criados");

        // --- CRIAÇÃO DE CHAMADOS DE EXEMPLO ---
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
            TecnicoResponsavelId = tecnico.Id
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
        await context.SaveChangesAsync();
        Console.WriteLine("[SEED] ✅ Chamados de exemplo criados");

        // --- CRIAÇÃO DE MENSAGENS ---
        var mensagens = new List<Mensagem>
        {
            new() { Texto = "Olá Beto, recebi seu chamado sobre a lentidão. Estou investigando a causa.", ChamadoId = chamadoEmAndamento.Id, AutorId = tecnico.Id, LidoPeloCliente = false },
            new() { Texto = "Obrigado pelo retorno! Fico no aguardo.", ChamadoId = chamadoEmAndamento.Id, AutorId = cliente2.Id, LidoPeloCliente = true, IsNotaInterna = false},
            new() { Texto = "NOTA: Verificar os índices da tabela de vendas. Pode ser a causa da lentidão.", ChamadoId = chamadoEmAndamento.Id, AutorId = tecnico.Id, IsNotaInterna = true}
        };

        await context.Mensagens.AddRangeAsync(mensagens);
        await context.SaveChangesAsync();
        Console.WriteLine("[SEED] ✅ Mensagens de exemplo criadas");

        // --- CRIAÇÃO DE ANEXOS DE EXEMPLO ---
        var anexos = new List<Anexo>
        {
            new() { 
                NomeArquivo = "print_erro_pagamento.png", 
                NomeUnico = "exemplo_print_erro.png", 
                TipoArquivo = "image/png", 
                ChamadoId = chamadoAberto.Id 
            },
            new() { 
                NomeArquivo = "relatorio_performance.pdf", 
                NomeUnico = "exemplo_relatorio.pdf", 
                TipoArquivo = "application/pdf", 
                ChamadoId = chamadoEmAndamento.Id 
            },
            new() { 
                NomeArquivo = "logs_sistema.txt", 
                NomeUnico = "exemplo_logs.txt", 
                TipoArquivo = "text/plain", 
                ChamadoId = chamadoEmAndamento.Id 
            }
        };

        await context.Anexos.AddRangeAsync(anexos);
        await context.SaveChangesAsync();
        Console.WriteLine("[SEED] ✅ Anexos de exemplo criados");

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
            await context.SaveChangesAsync();
            Console.WriteLine("[SEED] ✅ Respostas prontas criadas");
        }
    }
}