using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IAnexoRepository
{
    Task AddAsync(Anexo anexo);
}