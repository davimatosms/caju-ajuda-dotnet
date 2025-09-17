using System.Collections.Generic;

namespace CajuAjuda.Backend.Services.Dtos;

// Classe auxiliar para os dados do gráfico de linha
public class DailyStat
{
    public string Dia { get; set; } = string.Empty;
    public int Criados { get; set; }
    public int Fechados { get; set; }
}

// Classe auxiliar para os dados dos gráficos de barra/pizza
public class ChartDataPoint
{
    public string Name { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class DashboardResponseDto
{
    public int TotalChamados { get; set; }
    public int ChamadosAbertos { get; set; }
    public int ChamadosEmAndamento { get; set; }
    public int ChamadosFechados { get; set; }
    public double PercentualResolvidos { get; set; }
    
    // A propriedade agora é uma Lista do nosso novo tipo
    public List<ChartDataPoint> ChamadosPorPrioridade { get; set; } = new();

    public double TempoMedioPrimeiraRespostaHoras { get; set; }
    public double TempoMedioResolucaoHoras { get; set; }
    public List<DailyStat> StatsUltimos7Dias { get; set; } = new();
}