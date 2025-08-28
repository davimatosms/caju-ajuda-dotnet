using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IUsuarioService
{
    Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto);
}