// CajuAjuda.Desktop/Models/Mensagem.cs

using System;
using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class Mensagem
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; } = string.Empty;

        [JsonPropertyName("dataEnvio")]
        public DateTime DataEnvio { get; set; }

        [JsonPropertyName("autorNome")]
        public string AutorNome { get; set; } = string.Empty;
    }
}