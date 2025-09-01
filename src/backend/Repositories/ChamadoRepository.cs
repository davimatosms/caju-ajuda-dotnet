using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

public class ChamadoRepository : IChamadoRepository
{
    private readonly CajuAjudaDbContext _context;

    public ChamadoRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Chamado chamado)
    {
        _context.Chamados.Add(chamado);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId)
    {
        // Retorna todos os chamados onde o ClienteId corresponde ao ID fornecido
        return await _context.Chamados
            .Where(c => c.ClienteId == clienteId)
            .OrderByDescending(c => c.DataCriacao) // Ordena pelos mais recentes
            .ToListAsync();
    }

    public async Task<IEnumerable<Chamado>> GetAllAsync()
    {
        // Retorna todos os chamados, incluindo os dados do cliente relacionado
        // "Include" é o equivalente ao JOIN para carregar o objeto Usuario
        // Ordena pelos mais recentes
        return await _context.Chamados
            .Include(c => c.Cliente)
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync();
    }

    public async Task<Chamado?> GetByIdAsync(long id)
    {
        // Retorna um único chamado pelo seu ID
        // Inclui o Cliente, as Mensagens e o Autor de cada mensagem para a tela de detalhes
        return await _context.Chamados
            .Include(c => c.Cliente)
            .Include(c => c.Mensagens)
                .ThenInclude(m => m.Autor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Chamado chamado)
{
    // Marca a entidade como modificada para que o EF Core gere o comando UPDATE
    _context.Chamados.Update(chamado);
    await _context.SaveChangesAsync();
}
}