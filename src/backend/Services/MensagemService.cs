using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public class MensagemService : IMensagemService
{
    private readonly IMensagemRepository _mensagemRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IChamadoRepository _chamadoRepository;

    public MensagemService(IMensagemRepository mensagemRepository, IUsuarioRepository usuarioRepository, IChamadoRepository chamadoRepository)
    {
        _mensagemRepository = mensagemRepository;
        _usuarioRepository = usuarioRepository;
        _chamadoRepository = chamadoRepository;
    }

    public async Task<Mensagem> AddMensagemAsync(long chamadoId, MensagemCreateDto mensagemDto, string userEmail, string userRole)
    {
        // 1. Validações
        var autor = await _usuarioRepository.GetByEmailAsync(userEmail);
        if (autor == null)
        {
            throw new Exception("Usuário autor não encontrado.");
        }

        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new Exception("Chamado não encontrado.");
        }

        // 2. Lógica de permissão
        // Um técnico/admin pode comentar em qualquer chamado.
        // Um cliente só pode comentar no seu próprio chamado.
        if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.ClienteId != autor.Id)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para adicionar mensagens a este chamado.");
        }
        
        // Impede que mensagens sejam adicionadas a um chamado fechado
        if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
        {
            throw new Exception("Não é possível adicionar mensagens a um chamado fechado ou cancelado.");
        }

        // 3. Criar a nova entidade Mensagem
        var novaMensagem = new Mensagem
        {
            Texto = mensagemDto.Texto,
            DataEnvio = DateTime.UtcNow,
            AutorId = autor.Id,
            ChamadoId = chamadoId
        };
        
        // Opcional: Se um técnico responde, o status do chamado muda
        if (autor.Role == Role.TECNICO || autor.Role == Role.ADMIN)
        {
            chamado.Status = StatusChamado.AGUARDANDO_CLIENTE;
        }
        else // Se um cliente responde, o status volta para em andamento
        {
            chamado.Status = StatusChamado.EM_ANDAMENTO;
        }

        // 4. Salvar no banco de dados
        await _mensagemRepository.AddAsync(novaMensagem);
        
        // Futuramente, aqui dispararíamos o e-mail de notificação

        return novaMensagem;
    }
}