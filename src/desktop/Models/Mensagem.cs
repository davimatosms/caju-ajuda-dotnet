// CajuAjuda.Desktop/Models/Mensagem.cs

using System;
using System.Collections.Generic;
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

        [JsonPropertyName("anexos")]
        public List<Anexo>? Anexos { get; set; }
    }

    public class Anexo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nomeArquivo")]
        public string NomeArquivo { get; set; } = string.Empty;

        [JsonPropertyName("nomeUnico")]
        public string NomeUnico { get; set; } = string.Empty;

        [JsonPropertyName("tipoArquivo")]
        public string TipoArquivo { get; set; } = string.Empty;

        [JsonPropertyName("chamadoId")]
        public long ChamadoId { get; set; }

        // Propriedades legadas (para compatibilidade)
        [JsonPropertyName("caminhoArquivo")]
        public string CaminhoArquivo 
        { 
            get => NomeUnico; 
            set => NomeUnico = value; 
        }

        [JsonPropertyName("tipoConteudo")]
        public string TipoConteudo 
        { 
            get => TipoArquivo; 
            set => TipoArquivo = value; 
        }

        [JsonPropertyName("tamanho")]
        public long Tamanho { get; set; }

        /// <summary>
        /// Propriedade auxiliar para exibir o tamanho do arquivo formatado
        /// </summary>
        public string TamanhoFormatado => FormatarTamanho(Tamanho);

        private static string FormatarTamanho(long bytes)
        {
            if (bytes == 0) return "Tamanho desconhecido";
            
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;
            
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            
            return $"{number:n1} {suffixes[counter]}";
        }
    }
}