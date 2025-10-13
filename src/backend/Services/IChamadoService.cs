using CajuAjuda.Backend.Helpers;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services
{
    public interface IChamadoService
    {
        
        Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail);
        Task<IEnumerable<ChamadoListResponseDto>> GetChamadosByClienteEmailAsync(string clienteEmail);
        Task<PagedList<ChamadoResponseDto>> GetAllChamadosAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade);
        Task<ChamadoDetailResponseDto?> GetChamadoByIdAsync(long chamadoId, string userEmail, string userRole);
        Task UpdateChamadoStatusAsync(long chamadoId, StatusChamado novoStatus);
        Task<Anexo> AddAnexoAsync(long chamadoId, IFormFile file, string userEmail);
        Task AssignChamadoAsync(long chamadoId, string tecnicoEmail);
        Task AvaliarChamadoAsync(long chamadoId, AvaliacaoDto avaliacaoDto, string clienteEmail);
        Task MergeChamadosAsync(long chamadoDuplicadoId, long chamadoPrincipalId, string tecnicoEmail);
        Task<IEnumerable<ChamadoResponseDto>> GetChamadosAtribuidosAsync(string tecnicoEmail);
        Task<IEnumerable<ChamadoResponseDto>> GetChamadosDisponiveisAsync();
    }
}