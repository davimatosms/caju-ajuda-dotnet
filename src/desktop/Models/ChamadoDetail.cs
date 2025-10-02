// CajuAjuda.Desktop/Models/ChamadoDetail.cs

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class ChamadoDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("nomeCliente")]
        public string NomeCliente { get; set; } = string.Empty;

        [JsonPropertyName("nomeTecnicoResponsavel")]
        public string? NomeTecnicoResponsavel { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("prioridade")]
        public string Prioridade { get; set; } = string.Empty;

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("mensagens")]
        public List<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    }
}