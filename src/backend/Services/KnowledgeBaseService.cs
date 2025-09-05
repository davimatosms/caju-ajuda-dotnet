using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
namespace CajuAjuda.Backend.Services;
public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly IKnowledgeBaseRepository _repository;
    public KnowledgeBaseService(IKnowledgeBaseRepository repository) { _repository = repository; }
    
    public async Task<IEnumerable<KbCategoria>> GetAllCategoriasComArtigosAsync()
    {
        return await _repository.GetAllCategoriasComArtigosAsync();
    }

    public async Task<IEnumerable<KbArtigo>> SearchArtigosAsync(string searchTerm)
    {
        return await _repository.SearchArtigosAsync(searchTerm);
    }
}