using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class SenhaUpdateDto
    {
        [JsonPropertyName("senhaAtual")]
        public string SenhaAtual { get; set; } = string.Empty;

        [JsonPropertyName("novaSenha")]
        public string NovaSenha { get; set; } = string.Empty;
    }
}