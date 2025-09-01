using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IMensagemRepository
{
    Task AddAsync(Mensagem mensagem);
}