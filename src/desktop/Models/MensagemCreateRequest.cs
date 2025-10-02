// CajuAjuda.Desktop/Models/MensagemCreateRequest.cs

using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class MensagemCreateRequest
    {
        [JsonPropertyName("texto")]
        public string Texto { get; set; } = string.Empty;
    }
}