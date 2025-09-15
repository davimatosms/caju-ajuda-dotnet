using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public interface IUsuarioService
{
    Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto);
    Task<Usuario?> AuthenticateAsync(LoginDto loginDto);
    Task<bool> VerifyEmailAsync(string token);
    Task<PerfilResponseDto> GetPerfilAsync(string userEmail);
    Task UpdatePerfilAsync(string userEmail, PerfilUpdateDto perfilDto);
    Task UpdateSenhaAsync(string userEmail, SenhaUpdateDto senhaDto);
}