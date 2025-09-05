using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IMensagemService
{
    Task<Mensagem> AddMensagemAsync(long chamadoId, MensagemCreateDto mensagemDto, string userEmail, string userRole);
    Task<Mensagem> AddNotaInternaAsync(long chamadoId, MensagemCreateDto notaDto, string tecnicoEmail); 
}