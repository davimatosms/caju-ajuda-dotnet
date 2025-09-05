using CajuAjuda.Backend.Models;
namespace CajuAjuda.Backend.Repositories;
public interface IKnowledgeBaseRepository
{
    Task<IEnumerable<KbCategoria>> GetAllCategoriasComArtigosAsync();
    Task<IEnumerable<KbArtigo>> SearchArtigosAsync(string searchTerm);
}