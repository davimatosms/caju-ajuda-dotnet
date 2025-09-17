using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        var todosChamados = await _context.Chamados
            .Include(c => c.Mensagens)
            .ThenInclude(m => m.Autor)
            .ToListAsync();

        var chamadosFechados = todosChamados.Where(c => c.Status == StatusChamado.FECHADO).ToList();
        
        var prioridades = todosChamados
            .GroupBy(c => c.Prioridade.ToString())
            .Select(g => new ChartDataPoint { Name = g.Key, Total = g.Count() })
            .ToList();

        var totalChamados = todosChamados.Count;
        var totalFechados = chamadosFechados.Count;
        var percentualResolvidos = totalChamados > 0 ? ((double)totalFechados / totalChamados) * 100 : 0;
        double totalHorasResolucao = 0;
        if (chamadosFechados.Any())
        {
            totalHorasResolucao = chamadosFechados
                .Where(c => c.DataFechamento.HasValue)
                .Average(c => (c.DataFechamento.Value - c.DataCriacao).TotalHours);
        }
        double totalHorasPrimeiraResposta = 0;
        var chamadosComResposta = todosChamados
            .Where(c => c.Mensagens.Any(m => m.Autor.Role == Role.TECNICO || m.Autor.Role == Role.ADMIN))
            .ToList();
        if (chamadosComResposta.Any())
        {
            totalHorasPrimeiraResposta = chamadosComResposta.Average(c =>
            {
                var primeiraResposta = c.Mensagens
                    .Where(m => m.Autor.Role == Role.TECNICO || m.Autor.Role == Role.ADMIN)
                    .OrderBy(m => m.DataEnvio)
                    .First();
                return (primeiraResposta.DataEnvio - c.DataCriacao).TotalHours;
            });
        }
        var statsDiarios = new List<DailyStat>();
        var hoje = DateTime.UtcNow.Date;
        for (int i = 6; i >= 0; i--)
        {
            var dia = hoje.AddDays(-i);
            statsDiarios.Add(new DailyStat
            {
                Dia = dia.ToString("dd/MM"),
                Criados = todosChamados.Count(c => c.DataCriacao.Date == dia),
                Fechados = chamadosFechados.Count(c => c.DataFechamento.HasValue && c.DataFechamento.Value.Date == dia)
            });
        }
        
        var response = new DashboardResponseDto
        {
            TotalChamados = totalChamados,
            ChamadosAbertos = todosChamados.Count(c => c.Status == StatusChamado.ABERTO),
            ChamadosEmAndamento = todosChamados.Count(c => c.Status == StatusChamado.EM_ANDAMENTO),
            ChamadosFechados = totalFechados,
            PercentualResolvidos = percentualResolvidos,
            ChamadosPorPrioridade = prioridades,
            TempoMedioResolucaoHoras = totalHorasResolucao,
            TempoMedioPrimeiraRespostaHoras = totalHorasPrimeiraResposta,
            StatsUltimos7Dias = statsDiarios
        };

        return response;
    }
}