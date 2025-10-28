using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Backend.Repositories;

public class MensagemRepository : IMensagemRepository
{
    private readonly CajuAjudaDbContext _context;
    private readonly ILogger<MensagemRepository> _logger;

    public MensagemRepository(CajuAjudaDbContext context, ILogger<MensagemRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Mensagem mensagem)
    {
        _logger.LogInformation("📝 [MensagemRepository] Adicionando mensagem ID {MensagemId} ao contexto...", mensagem.Id);
        
        _context.Mensagens.Add(mensagem);
        
        _logger.LogInformation("💾 [MensagemRepository] Salvando mensagem no banco de dados...");
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("✅ [MensagemRepository] Mensagem ID {MensagemId} salva com sucesso!", mensagem.Id);
        
        // Limpa o cache do Entity Framework para forçar busca fresca
        _context.ChangeTracker.Clear();
        _logger.LogInformation("🧹 [MensagemRepository] Cache do ChangeTracker limpo.");
    }
    
    // NOVO MÉTODO
    public async Task MarkMessagesAsReadByClienteAsync(long chamadoId)
    {
        // Encontra todas as mensagens não lidas de um chamado e atualiza o status para lido
        await _context.Mensagens
            .Where(m => m.ChamadoId == chamadoId && !m.LidoPeloCliente)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.LidoPeloCliente, true));
    }
}