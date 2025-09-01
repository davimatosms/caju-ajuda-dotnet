using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;

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
}