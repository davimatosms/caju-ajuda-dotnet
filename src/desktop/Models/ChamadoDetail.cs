using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class ChamadoDetalhes
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("nomeCliente")]
        public string NomeCliente { get; set; } = string.Empty;

        // ======================================================
        //          NOVA PROPRIEDADE ADICIONADA AQUI
        // ======================================================
        [JsonPropertyName("nomeTecnicoResponsavel")]
        public string? NomeTecnicoResponsavel { get; set; } // Adicionado '?' para permitir que seja nulo

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("prioridade")]
        public string Prioridade { get; set; } = string.Empty;

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("mensagens")]
        public List<Mensagem> Mensagens { get; set; } = new();
    }
}