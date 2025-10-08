// CajuAjuda.Desktop/Models/LoginResponse.cs

using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty; 
    }
}