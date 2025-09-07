using Xunit;
using Moq;
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Data;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Tests;

public class AdminServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
    private readonly Mock<CajuAjudaDbContext> _mockDbContext;
    private readonly Mock<ILogger<AdminService>> _mockLogger;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _mockUsuarioRepo = new Mock<IUsuarioRepository>();
        _mockDbContext = new Mock<CajuAjudaDbContext>(); // Agora funciona por causa do construtor vazio
        _mockLogger = new Mock<ILogger<AdminService>>();
        
        _adminService = new AdminService(_mockUsuarioRepo.Object, _mockDbContext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllTecnicosAsync_DeveRetornarApenasUsuariosComRoleTecnico()
    {
        // Arrange (Preparação)
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nome = "Tecnico 1", Email = "tec1@email.com", Role = Role.TECNICO },
            new() { Id = 2, Nome = "Cliente 1", Email = "cli1@email.com", Role = Role.CLIENTE },
            new() { Id = 3, Nome = "Tecnico 2", Email = "tec2@email.com", Role = Role.TECNICO }
        };

        // Ensina o mock do DbContext o que fazer quando a propriedade Usuarios for chamada
        _mockDbContext.Setup(x => x.Usuarios).ReturnsDbSet(usuarios);

        // Act (Execução)
        var resultado = await _adminService.GetAllTecnicosAsync();
        var listaResultado = resultado.ToList();

        // Assert (Verificação)
        Assert.NotNull(resultado);
        Assert.Equal(2, listaResultado.Count); // Deve retornar apenas 2 técnicos
        Assert.All(listaResultado, item => Assert.NotEqual("Cliente 1", item.Nome)); // Garante que o cliente não está na lista
        Assert.Equal("Tecnico 1", listaResultado[0].Nome);
    }
}