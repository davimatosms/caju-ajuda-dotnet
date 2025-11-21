namespace CajuAjuda.Shared.Models
{
    /// <summary>
    /// Representa um DTO (Data Transfer Object) para os detalhes completos de um chamado.
    /// É usado para agrupar o chamado principal e suas mensagens em uma única resposta da API.
    /// </summary>
    public class ChamadoDetalhes
    {
        public Chamado Chamado { get; set; } = null!;
        public List<Mensagem> Mensagens { get; set; } = new();
    }
}


//container