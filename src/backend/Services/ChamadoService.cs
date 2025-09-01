using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    // Futuramente, injetaremos o AIService aqui para a priorização automática

    public ChamadoService(IChamadoRepository chamadoRepository, IUsuarioRepository usuarioRepository)
    {
        _chamadoRepository = chamadoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail)
    {
        // 1. Encontrar o usuário cliente pelo e-mail (que virá do token JWT)
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null || cliente.Role != Role.CLIENTE)
        {
            throw new Exception("Cliente não encontrado ou usuário não autorizado para criar chamados.");
        }

        // 2. Criar a nova entidade Chamado
        var novoChamado = new Chamado
        {
            Titulo = chamadoDto.Titulo,
            Descricao = chamadoDto.Descricao,
            Prioridade = chamadoDto.Prioridade, // Por enquanto, usamos a do DTO. No futuro, virá da IA.
            Status = StatusChamado.ABERTO,
            DataCriacao = DateTime.UtcNow,
            ClienteId = cliente.Id
        };

        // 3. Salvar no banco de dados
        await _chamadoRepository.AddAsync(novoChamado);

        return novoChamado;
    }

    public async Task<IEnumerable<Chamado>> GetChamadosByClienteEmailAsync(string clienteEmail)
    {
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null)
        {
            // Lança uma exceção se o usuário do token não for encontrado
            throw new Exception("Cliente não encontrado.");
        }

        return await _chamadoRepository.GetByClienteIdAsync(cliente.Id);
    }

    public async Task<IEnumerable<ChamadoResponseDto>> GetAllChamadosAsync()
    {
        var chamados = await _chamadoRepository.GetAllAsync();

        // Mapeia a lista de entidades Chamado para uma lista de ChamadoResponseDto
        var chamadosDto = chamados.Select(c => new ChamadoResponseDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            NomeCliente = c.Cliente.Nome, // Usamos o nome do cliente que foi carregado
            Status = c.Status,
            Prioridade = c.Prioridade,
            DataCriacao = c.DataCriacao
        });

        return chamadosDto;
    }

    public async Task<ChamadoDetailResponseDto?> GetChamadoByIdAsync(long chamadoId, string userEmail, string userRole)
    {
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            return null; // Chamado não encontrado
        }

        // Lógica de Autorização:
        // Um técnico/admin pode ver qualquer chamado.
        // Um cliente só pode ver o seu próprio chamado.
        if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.Cliente.Email != userEmail)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para visualizar este chamado.");
        }

        // Mapeia a entidade para o DTO de resposta detalhado
        var chamadoDto = new ChamadoDetailResponseDto
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            NomeCliente = chamado.Cliente.Nome,
            Status = chamado.Status,
            Prioridade = chamado.Prioridade,
            DataCriacao = chamado.DataCriacao,
            DataFechamento = chamado.DataFechamento,
            Mensagens = chamado.Mensagens.Select(m => new MensagemResponseDto
            {
                Id = m.Id,
                Texto = m.Texto,
                DataEnvio = m.DataEnvio,
                AutorNome = m.Autor.Nome,
                AutorId = m.Autor.Id
            }).OrderBy(m => m.DataEnvio).ToList()
        };

        return chamadoDto;
    }

    public async Task UpdateChamadoStatusAsync(long chamadoId, StatusChamado novoStatus)
{
    var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
    if (chamado == null)
    {
        throw new Exception("Chamado não encontrado.");
    }

    // Lógica de negócio: um chamado fechado não pode ter seu status alterado.
    if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
    {
        throw new Exception("Não é possível alterar o status de um chamado que já foi fechado ou cancelado.");
    }

    chamado.Status = novoStatus;

    // Se o novo status for FECHADO, atualiza a data de fechamento.
    if (novoStatus == StatusChamado.FECHADO)
    {
        chamado.DataFechamento = DateTime.UtcNow;
    }

    await _chamadoRepository.UpdateAsync(chamado);

    // Futuramente, aqui dispararemos uma notificação por e-mail para o cliente.
}
}