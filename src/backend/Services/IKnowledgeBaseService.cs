using CajuAjuda.Backend.Models;
namespace CajuAjuda.Backend.Services;
public interface IKnowledgeBaseService
{
    Task<IEnumerable<KbCategoria>> GetAllCategoriasComArtigosAsync();
    Task<IEnumerable<KbArtigo>> SearchArtigosAsync(string searchTerm);
}