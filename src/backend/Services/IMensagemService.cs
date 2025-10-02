// CajuAjuda.Backend/Services/IMensagemService.cs

using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services
{
    public interface IMensagemService
    {
        
        Task<MensagemResponseDto> AddMensagemAsync(long chamadoId, MensagemCreateDto mensagemDto, string userEmail, string userRole);

       
        Task<MensagemResponseDto> AddNotaInternaAsync(long chamadoId, MensagemCreateDto notaDto, string tecnicoEmail);
    }
}