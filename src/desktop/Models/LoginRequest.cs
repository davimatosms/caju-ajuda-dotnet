

using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty; 

        [JsonPropertyName("senha")]
        public string Senha { get; set; } = string.Empty; 
    }
}