using Microsoft.AspNetCore.Http;

namespace CajuAjuda.Backend.Services;

public class LocalStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public LocalStorageService(IConfiguration configuration)
    {
        // Pega o caminho do nosso arquivo de configuração
        _storagePath = configuration.GetValue<string>("FileStorage:LocalStoragePath") ?? "caju_uploads";

        // Garante que o diretório de uploads exista
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Arquivo inválido.");
        }

        // Cria um nome de arquivo único para evitar colisões
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }
}