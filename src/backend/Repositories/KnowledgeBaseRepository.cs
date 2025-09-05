using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace CajuAjuda.Backend.Repositories;
public class KnowledgeBaseRepository : IKnowledgeBaseRepository
{
    private readonly CajuAjudaDbContext _context;
    public KnowledgeBaseRepository(CajuAjudaDbContext context) { _context = context; }

    public async Task<IEnumerable<KbCategoria>> GetAllCategoriasComArtigosAsync()
    {
        return await _context.KbCategorias.Include(c => c.Artigos).ToListAsync();
    }

    public async Task<IEnumerable<KbArtigo>> SearchArtigosAsync(string searchTerm)
    {
        return await _context.KbArtigos
            .Where(a => a.Titulo.Contains(searchTerm) || a.Conteudo.Contains(searchTerm))
            .Include(a => a.Categoria)
            .ToListAsync();
    }
}