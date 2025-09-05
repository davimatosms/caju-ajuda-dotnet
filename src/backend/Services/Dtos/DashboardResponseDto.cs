namespace CajuAjuda.Backend.Services.Dtos;

public class DashboardResponseDto
{
    public int TotalChamados { get; set; }
    public int ChamadosAbertos { get; set; }
    public int ChamadosEmAndamento { get; set; }
    public int ChamadosFechados { get; set; }
    public double PercentualResolvidos { get; set; }
    public Dictionary<string, int> ChamadosPorPrioridade { get; set; } = new();
    public Dictionary<string, int> ChamadosPorTecnico { get; set; } = new();
}