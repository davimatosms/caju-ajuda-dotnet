

using System;
using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class Chamado
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty; 

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty; 

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; 
        [JsonPropertyName("prioridade")]
        public string Prioridade { get; set; } = string.Empty; 

        [JsonPropertyName("clienteNome")]
        public string ClienteNome { get; set; } = string.Empty; 

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }
    }
}