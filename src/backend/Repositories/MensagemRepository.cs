using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

public class MensagemRepository : IMensagemRepository
{
    private readonly CajuAjudaDbContext _context;

    public MensagemRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Mensagem mensagem)
    {
        _context.Mensagens.Add(mensagem);
        await _context.SaveChangesAsync();
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