using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;

namespace CajuAjuda.Backend.Services;

public class RespostaProntaService : IRespostaProntaService
{
    private readonly IRespostaProntaRepository _repository;

    public RespostaProntaService(IRespostaProntaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RespostaPronta>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}