# 9. CLASSES CRUD E API REST

## 9.1 INTRODUÇÃO

Este documento apresenta as principais classes responsáveis pelas operações CRUD (Create, Read, Update, Delete) e pela exposição da API REST do sistema CajuAjuda.

**Arquitetura em Camadas**:
```
Controllers (API REST) 
    ↓
Services (Regras de Negócio)
    ↓
Repositories (Acesso a Dados)
    ↓
DbContext (Entity Framework Core)
    ↓
SQL Server
```

---

## 9.2 CAMADA DE REPOSITÓRIOS (DATA ACCESS)

### **Interface: `IUsuarioRepository.cs`**

```csharp
using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

/// <summary>
/// Interface do repositório de usuários.
/// Define as operações de acesso a dados para a entidade Usuario.
/// </summary>
public interface IUsuarioRepository
{
    // CREATE
    Task<Usuario> AddAsync(Usuario usuario);
    
    // READ
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByVerificationTokenAsync(string token);
    Task<List<Usuario>> GetAllAsync();
    Task<List<Usuario>> GetByRoleAsync(Role role);
    
    // UPDATE
    Task UpdateAsync(Usuario usuario);
    
    // DELETE
    Task DeleteAsync(int id);
    
    // SPECIFIC QUERIES
    Task<int> CountByRoleAsync(Role role);
    Task<bool> ExistsAsync(int id);
}
```

### **Implementação: `UsuarioRepository.cs`**

```csharp
using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

/// <summary>
/// Implementação do repositório de usuários.
/// Utiliza Entity Framework Core para acesso ao banco de dados.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly CajuAjudaDbContext _context;

    public UsuarioRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    // ========================================================================
    // CREATE - Criar novo usuário
    // ========================================================================
    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    // ========================================================================
    // READ - Buscar usuário por ID
    // ========================================================================
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    // ========================================================================
    // READ - Buscar usuário por email
    // ========================================================================
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    // ========================================================================
    // READ - Buscar usuário por token de verificação
    // ========================================================================
    public async Task<Usuario?> GetByVerificationTokenAsync(string token)
    {
        Console.WriteLine($"[REPO] Buscando usuário com token: '{token}'");
        
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.VerificationToken == token);
        
        if (user != null)
        {
            Console.WriteLine($"[REPO] ✅ Usuário encontrado: {user.Email}");
        }
        else
        {
            Console.WriteLine($"[REPO] ❌ Nenhum usuário encontrado");
        }
        
        return user;
    }

    // ========================================================================
    // READ - Listar todos os usuários
    // ========================================================================
    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    // ========================================================================
    // READ - Listar usuários por perfil (Role)
    // ========================================================================
    public async Task<List<Usuario>> GetByRoleAsync(Role role)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Role == role)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    // ========================================================================
    // UPDATE - Atualizar usuário existente
    // ========================================================================
    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    // ========================================================================
    // DELETE - Excluir usuário por ID
    // ========================================================================
    public async Task DeleteAsync(int id)
    {
        var usuario = await GetByIdAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }

    // ========================================================================
    // SPECIFIC QUERIES - Contar usuários por perfil
    // ========================================================================
    public async Task<int> CountByRoleAsync(Role role)
    {
        return await _context.Usuarios
            .CountAsync(u => u.Role == role);
    }

    // ========================================================================
    // SPECIFIC QUERIES - Verificar se usuário existe
    // ========================================================================
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Id == id);
    }
}
```

---

## 9.3 CAMADA DE SERVIÇOS (BUSINESS LOGIC)

### **Interface: `IUsuarioService.cs`**

```csharp
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

/// <summary>
/// Interface do serviço de usuários.
/// Define as regras de negócio para gerenciamento de usuários.
/// </summary>
public interface IUsuarioService
{
    // AUTENTICAÇÃO
    Task<Usuario?> AuthenticateAsync(LoginDto loginDto);
    Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto);
    Task<bool> VerifyEmailAsync(string token);
    
    // PERFIL
    Task<PerfilResponseDto> GetPerfilAsync(string userEmail);
    Task UpdatePerfilAsync(string userEmail, PerfilUpdateDto perfilDto);
    Task UpdateSenhaAsync(string userEmail, SenhaUpdateDto senhaDto);
}
```

### **Implementação: `UsuarioService.cs`**

```csharp
using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

/// <summary>
/// Serviço de gerenciamento de usuários.
/// Contém as regras de negócio e validações.
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly EmailTemplateService _emailTemplateService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        EmailTemplateService emailTemplateService)
    {
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
    }

    // ========================================================================
    // AUTENTICAÇÃO - Validar credenciais do usuário
    // ========================================================================
    public async Task<Usuario?> AuthenticateAsync(LoginDto loginDto)
    {
        // Buscar usuário por email
        var user = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

        // Validar:
        // 1. Usuário existe
        // 2. Conta está habilitada (email verificado)
        // 3. Senha está correta
        if (user == null || 
            !user.Enabled || 
            !BCrypt.Net.BCrypt.Verify(loginDto.Senha, user.Senha))
        {
            Console.WriteLine($"[AUTH] Falha na autenticação para: {loginDto.Email}");
            return null;
        }

        Console.WriteLine($"[AUTH] Login bem-sucedido: {loginDto.Email}");
        return user;
    }

    // ========================================================================
    // CADASTRO - Registrar novo cliente
    // ========================================================================
    public async Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto)
    {
        // Validar se email já está cadastrado
        var existingUser = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
        if (existingUser != null)
        {
            throw new BusinessRuleException("Este e-mail já está cadastrado.");
        }

        // Criptografar senha com bcrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);

        // Gerar token de verificação único
        var verificationToken = Guid.NewGuid().ToString();
        Console.WriteLine($"[REGISTER] Token gerado: '{verificationToken}'");

        // Criar novo usuário
        var novoUsuario = new Usuario
        {
            Nome = usuarioDto.Nome,
            Email = usuarioDto.Email,
            Senha = passwordHash,
            Role = Role.CLIENTE,
            Enabled = false,  // Desabilitado até verificar email
            VerificationToken = verificationToken
        };

        // Salvar no banco
        await _usuarioRepository.AddAsync(novoUsuario);

        // Enviar email de verificação
        await SendVerificationEmailAsync(
            novoUsuario.Email,
            novoUsuario.Nome,
            verificationToken
        );

        return novoUsuario;
    }

    // ========================================================================
    // VERIFICAÇÃO - Ativar conta através do token
    // ========================================================================
    public async Task<bool> VerifyEmailAsync(string token)
    {
        Console.WriteLine($"[VERIFY_SERVICE] Buscando usuário com token: '{token}'");
        
        // Buscar usuário pelo token
        var user = await _usuarioRepository.GetByVerificationTokenAsync(token);
        
        if (user == null)
        {
            Console.WriteLine($"[VERIFY_SERVICE] ❌ Token inválido!");
            return false;
        }

        // Ativar conta
        Console.WriteLine($"[VERIFY_SERVICE] ✅ Ativando: {user.Email}");
        user.Enabled = true;
        user.VerificationToken = null;  // Remover token usado
        
        await _usuarioRepository.UpdateAsync(user);
        
        Console.WriteLine($"[VERIFY_SERVICE] ✅ Conta ativada com sucesso!");
        return true;
    }

    // ========================================================================
    // PERFIL - Obter dados do perfil do usuário
    // ========================================================================
    public async Task<PerfilResponseDto> GetPerfilAsync(string userEmail)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        return new PerfilResponseDto
        {
            Nome = user.Nome,
            Email = user.Email
        };
    }

    // ========================================================================
    // PERFIL - Atualizar dados do perfil
    // ========================================================================
    public async Task UpdatePerfilAsync(string userEmail, PerfilUpdateDto perfilDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        // Validar se novo email já está em uso
        if (user.Email != perfilDto.Email)
        {
            var existingUserWithEmail = await _usuarioRepository
                .GetByEmailAsync(perfilDto.Email);
                
            if (existingUserWithEmail != null)
            {
                throw new BusinessRuleException(
                    "O e-mail informado já está em uso por outra conta."
                );
            }
        }

        // Atualizar dados
        user.Nome = perfilDto.Nome;
        user.Email = perfilDto.Email;
        
        await _usuarioRepository.UpdateAsync(user);
    }

    // ========================================================================
    // PERFIL - Alterar senha
    // ========================================================================
    public async Task UpdateSenhaAsync(string userEmail, SenhaUpdateDto senhaDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        // Validar senha atual
        if (!BCrypt.Net.BCrypt.Verify(senhaDto.SenhaAtual, user.Senha))
        {
            throw new BusinessRuleException("A senha atual está incorreta.");
        }

        // Atualizar para nova senha
        user.Senha = BCrypt.Net.BCrypt.HashPassword(senhaDto.NovaSenha);
        
        await _usuarioRepository.UpdateAsync(user);
    }

    // ========================================================================
    // PRIVADO - Enviar email de verificação
    // ========================================================================
    private async Task SendVerificationEmailAsync(
        string email,
        string nome,
        string token)
    {
        var emailBody = _emailTemplateService.GetVerificationEmailBody(nome, token);
        await _emailService.SendEmailAsync(
            email,
            "Verificação de E-mail - Caju Ajuda",
            emailBody
        );
    }
}
```

---

## 9.4 CAMADA DE CONTROLLERS (API REST)

### **DTOs (Data Transfer Objects)**

```csharp
using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

// ============================================================================
// DTO: Login
// ============================================================================
public class LoginDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

// ============================================================================
// DTO: Criação de Usuário
// ============================================================================
public class UsuarioCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

// ============================================================================
// DTO: Resposta do Perfil
// ============================================================================
public class PerfilResponseDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// ============================================================================
// DTO: Atualização do Perfil
// ============================================================================
public class PerfilUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;
}

// ============================================================================
// DTO: Atualização de Senha
// ============================================================================
public class SenhaUpdateDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória.")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}
```

### **Controller: `AuthController.cs`**

```csharp
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

/// <summary>
/// Controller responsável pela autenticação e cadastro de usuários.
/// Expõe endpoints públicos (sem autenticação).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    public AuthController(
        IUsuarioService usuarioService,
        ITokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    // ========================================================================
    // POST api/auth/register/cliente
    // Cadastrar novo cliente
    // ========================================================================
    /// <summary>
    /// Registra um novo cliente no sistema.
    /// Envia email de verificação automaticamente.
    /// </summary>
    /// <param name="usuarioDto">Dados do novo usuário</param>
    /// <returns>201 Created</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos ou email já cadastrado</response>
    [HttpPost("register/cliente")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterCliente([FromBody] UsuarioCreateDto usuarioDto)
    {
        // Validar ModelState (Data Annotations)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var novoUsuario = await _usuarioService.RegisterClienteAsync(usuarioDto);

            return CreatedAtAction(
                nameof(RegisterCliente),
                new { id = novoUsuario.Id },
                new
                {
                    message = "Registro bem-sucedido. Verifique seu e-mail para ativar a conta."
                }
            );
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ========================================================================
    // POST api/auth/login
    // Fazer login no sistema
    // ========================================================================
    /// <summary>
    /// Autentica um usuário e retorna um token JWT.
    /// </summary>
    /// <param name="loginDto">Credenciais (email e senha)</param>
    /// <returns>200 OK com token JWT</returns>
    /// <response code="200">Login bem-sucedido</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Credenciais incorretas ou conta não verificada</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usuarioService.AuthenticateAsync(loginDto);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou senha inválidos, ou a conta não foi verificada."
            });
        }

        // Gerar token JWT
        var token = _tokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                nome = user.Nome,
                email = user.Email,
                role = user.Role.ToString()
            }
        });
    }

    // ========================================================================
    // GET api/auth/verify?token=xxx
    // Verificar email através do token
    // ========================================================================
    /// <summary>
    /// Verifica o email do usuário através do token enviado por email.
    /// </summary>
    /// <param name="token">Token de verificação</param>
    /// <returns>200 OK se verificado com sucesso</returns>
    /// <response code="200">Email verificado com sucesso</response>
    /// <response code="400">Token inválido ou expirado</response>
    [HttpGet("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        Console.WriteLine($"[VERIFY] Token recebido: '{token}'");

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Token não fornecido." });
        }

        var success = await _usuarioService.VerifyEmailAsync(token);

        if (success)
        {
            return Ok(new
            {
                message = "E-mail verificado com sucesso! Você já pode fazer login."
            });
        }

        return BadRequest(new
        {
            message = "Token inválido ou expirado. Tente se registrar novamente."
        });
    }
}
```

---

## 9.5 EXEMPLO COMPLETO: CRUD DE CHAMADOS

### **Controller: `ChamadosController.cs`**

```csharp
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CajuAjuda.Backend.Controllers;

/// <summary>
/// Controller de gerenciamento de chamados.
/// Requer autenticação (JWT token).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Todos os endpoints requerem autenticação
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    // ========================================================================
    // GET api/chamados
    // Listar chamados do usuário autenticado
    // ========================================================================
    /// <summary>
    /// Lista os chamados do usuário autenticado.
    /// Clientes veem apenas seus chamados.
    /// Técnicos/Admins veem todos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ChamadoListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarChamados(
        [FromQuery] string? status = null,
        [FromQuery] string? prioridade = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var chamados = await _chamadoService.ListarChamadosAsync(
            userEmail!,
            userRole!,
            status,
            prioridade,
            pagina,
            tamanhoPagina
        );

        return Ok(chamados);
    }

    // ========================================================================
    // GET api/chamados/{id}
    // Buscar chamado por ID
    // ========================================================================
    /// <summary>
    /// Retorna os detalhes completos de um chamado.
    /// Inclui mensagens, anexos, cliente e técnico.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChamadoDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterChamado(int id)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        try
        {
            var chamado = await _chamadoService.ObterChamadoPorIdAsync(
                id,
                userEmail!,
                userRole!
            );

            return Ok(chamado);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    // ========================================================================
    // POST api/chamados
    // Criar novo chamado
    // ========================================================================
    /// <summary>
    /// Cria um novo chamado.
    /// Apenas clientes podem criar chamados.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "CLIENTE")]
    [ProducesResponseType(typeof(ChamadoDetailResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarChamado([FromBody] ChamadoCreateDto chamadoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        try
        {
            var novoChamado = await _chamadoService.CriarChamadoAsync(
                chamadoDto,
                userEmail!
            );

            return CreatedAtAction(
                nameof(ObterChamado),
                new { id = novoChamado.Id },
                novoChamado
            );
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ========================================================================
    // PUT api/chamados/{id}/status
    // Atualizar status do chamado
    // ========================================================================
    /// <summary>
    /// Atualiza o status de um chamado.
    /// Apenas técnicos e admins podem alterar status.
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "TECNICO,ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(
        int id,
        [FromBody] StatusUpdateDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _chamadoService.AtualizarStatusAsync(id, statusDto.NovoStatus);
            return Ok(new { message = "Status atualizado com sucesso." });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ========================================================================
    // PUT api/chamados/{id}/atribuir
    // Atribuir chamado a um técnico
    // ========================================================================
    /// <summary>
    /// Atribui um técnico a um chamado.
    /// Apenas técnicos e admins podem atribuir.
    /// </summary>
    [HttpPut("{id}/atribuir")]
    [Authorize(Roles = "TECNICO,ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtribuirTecnico(
        int id,
        [FromBody] AtribuirTecnicoDto atribuicaoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _chamadoService.AtribuirTecnicoAsync(id, atribuicaoDto.TecnicoId);
            return Ok(new { message = "Técnico atribuído com sucesso." });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ========================================================================
    // DELETE api/chamados/{id}
    // Excluir chamado
    // ========================================================================
    /// <summary>
    /// Exclui um chamado.
    /// Apenas admins podem excluir chamados.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirChamado(int id)
    {
        try
        {
            await _chamadoService.ExcluirChamadoAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
```

---

## 9.6 CONFIGURAÇÃO NO `Program.cs`

```csharp
using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// REGISTRAR DBCONTEXT
// ============================================================================
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ============================================================================
// REGISTRAR REPOSITÓRIOS (Scoped = uma instância por requisição)
// ============================================================================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IAnexoRepository, AnexoRepository>();

// ============================================================================
// REGISTRAR SERVIÇOS
// ============================================================================
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IChamadoService, ChamadoService>();
builder.Services.AddScoped<IMensagemService, MensagemService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailTemplateService>();

// ============================================================================
// CONFIGURAR JWT AUTHENTICATION
// ============================================================================
var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(jwtKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// ============================================================================
// CONFIGURAR CONTROLLERS E SWAGGER
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================================
// CONFIGURAR CORS
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ============================================================================
// MIDDLEWARES
// ============================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## 9.7 ENDPOINTS DISPONÍVEIS

### **Autenticação (Públicos)**

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/register/cliente` | Cadastrar cliente | ❌ Não |
| POST | `/api/auth/login` | Fazer login | ❌ Não |
| GET | `/api/auth/verify?token=xxx` | Verificar email | ❌ Não |

### **Chamados (Autenticados)**

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/api/chamados` | Listar chamados | Todos |
| GET | `/api/chamados/{id}` | Detalhes do chamado | Todos |
| POST | `/api/chamados` | Criar chamado | CLIENTE |
| PUT | `/api/chamados/{id}/status` | Atualizar status | TECNICO, ADMIN |
| PUT | `/api/chamados/{id}/atribuir` | Atribuir técnico | TECNICO, ADMIN |
| DELETE | `/api/chamados/{id}` | Excluir chamado | ADMIN |

### **Perfil (Autenticados)**

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/api/perfil` | Ver perfil | Todos |
| PUT | `/api/perfil` | Atualizar perfil | Todos |
| PUT | `/api/perfil/senha` | Alterar senha | Todos |

### **Admin (Autenticados)**

| Método | Endpoint | Descrição | Roles |
|--------|----------|-----------|-------|
| GET | `/api/admin/dashboard` | Estatísticas | ADMIN |
| GET | `/api/admin/usuarios` | Listar usuários | ADMIN |
| PUT | `/api/admin/usuarios/{id}/role` | Alterar role | ADMIN |
| DELETE | `/api/admin/usuarios/{id}` | Excluir usuário | ADMIN |

---

## 9.8 TRATAMENTO DE EXCEÇÕES

### **Middleware Global de Exceções**

```csharp
using CajuAjuda.Backend.Exceptions;
using System.Net;
using System.Text.Json;

namespace CajuAjuda.Backend.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro inesperado.";

        // Mapear tipos de exceção para status codes
        switch (exception)
        {
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case BusinessRuleException:
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Forbidden;
                message = "Acesso negado.";
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            error = true,
            message = message,
            statusCode = (int)code
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
```

**Registrar no `Program.cs`**:
```csharp
app.UseMiddleware<GlobalExceptionHandler>();
```

---

## 9.9 TESTES (EXEMPLO COM XUNIT)

```csharp
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Moq;
using Xunit;

namespace CajuAjuda.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _mockRepository = new Mock<IUsuarioRepository>();
        _mockEmailService = new Mock<IEmailService>();
        var emailTemplateService = new EmailTemplateService();
        
        _service = new UsuarioService(
            _mockRepository.Object,
            _mockEmailService.Object,
            emailTemplateService
        );
    }

    [Fact]
    public async Task RegisterCliente_DeveCriarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var dto = new UsuarioCreateDto
        {
            Nome = "João Silva",
            Email = "joao@teste.com",
            Senha = "123456"
        };

        _mockRepository
            .Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync((Usuario?)null);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => u);

        // Act
        var result = await _service.RegisterClienteAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nome, result.Nome);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(Role.CLIENTE, result.Role);
        Assert.False(result.Enabled);
    }

    [Fact]
    public async Task Authenticate_DeveRetornarNull_QuandoSenhaIncorreta()
    {
        // Arrange
        var usuario = new Usuario
        {
            Email = "teste@teste.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("senha_correta"),
            Enabled = true
        };

        _mockRepository
            .Setup(r => r.GetByEmailAsync(usuario.Email))
            .ReturnsAsync(usuario);

        var loginDto = new LoginDto
        {
            Email = usuario.Email,
            Senha = "senha_errada"
        };

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }
}
```

---

**Resumo**: As classes CRUD e API REST implementam a arquitetura em camadas (Controllers → Services → Repositories → DbContext), garantindo separação de responsabilidades, testabilidade, e fácil manutenção. Os endpoints RESTful seguem as convenções HTTP e incluem autenticação JWT, autorização baseada em roles, validação de dados, e tratamento global de exceções.
