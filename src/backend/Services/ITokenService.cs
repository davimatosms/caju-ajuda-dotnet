using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}