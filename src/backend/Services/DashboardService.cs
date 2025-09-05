using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Services;

public class DashboardService : IDashboardService
{
    private readonly CajuAjudaDbContext _context;

    public DashboardService(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponseDto> GetDashboardMetricsAsync()
    {
        var totalChamados = await _context.Chamados.CountAsync();
        var chamadosFechados = await _context.Chamados.CountAsync(c => c.Status == StatusChamado.FECHADO);
        
        var chamadosPorPrioridade = await _context.Chamados
            .GroupBy(c => c.Prioridade)
            .Select(g => new { Prioridade = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Prioridade, x => x.Count);

        var chamadosPorTecnico = await _context.Chamados
            .Where(c => c.TecnicoResponsavelId != null)
            .GroupBy(c => c.TecnicoResponsavel!.Nome)
            .Select(g => new { TecnicoNome = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TecnicoNome, x => x.Count);

        var dashboard = new DashboardResponseDto
        {
            TotalChamados = totalChamados,
            ChamadosAbertos = await _context.Chamados.CountAsync(c => c.Status == StatusChamado.ABERTO),
            ChamadosEmAndamento = await _context.Chamados.CountAsync(c => c.Status == StatusChamado.EM_ANDAMENTO),
            ChamadosFechados = chamadosFechados,
            PercentualResolvidos = totalChamados > 0 ? Math.Round((double)chamadosFechados / totalChamados * 100, 2) : 0,
            ChamadosPorPrioridade = chamadosPorPrioridade,
            ChamadosPorTecnico = chamadosPorTecnico
        };

        return dashboard;
    }
}