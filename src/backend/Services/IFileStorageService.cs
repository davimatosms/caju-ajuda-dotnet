using Microsoft.AspNetCore.Http;

namespace CajuAjuda.Backend.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file);
}