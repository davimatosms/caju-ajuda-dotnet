using CajuAjuda.Backend.Exceptions;
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
        var autor = await _usuarioRepository.GetByEmailAsync(userEmail);
        if (autor == null)
        {
            throw new NotFoundException("Utilizador autor não encontrado.");
        }

        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }

        if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.ClienteId != autor.Id)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para adicionar mensagens a este chamado.");
        }
        
        if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
        {
            throw new BusinessRuleException("Não é possível adicionar mensagens a um chamado fechado ou cancelado.");
        }

        var novaMensagem = new Mensagem
        {
            Texto = mensagemDto.Texto,
            DataEnvio = DateTime.UtcNow,
            AutorId = autor.Id,
            ChamadoId = chamadoId,
            IsNotaInterna = false,
            LidoPeloCliente = (autor.Role == Role.CLIENTE)
        };
        
        if (autor.Role == Role.TECNICO || autor.Role == Role.ADMIN)
        {
            chamado.Status = StatusChamado.AGUARDANDO_CLIENTE;
        }
        else 
        {
            chamado.Status = StatusChamado.EM_ANDAMENTO;
        }

        await _mensagemRepository.AddAsync(novaMensagem);
        
        return novaMensagem;
    }

    public async Task<Mensagem> AddNotaInternaAsync(long chamadoId, MensagemCreateDto notaDto, string tecnicoEmail)
    {
        var autor = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
        if (autor == null || autor.Role == Role.CLIENTE)
        {
            throw new UnauthorizedAccessException("Apenas técnicos ou administradores podem adicionar notas internas.");
        }

        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }

        var novaNota = new Mensagem
        {
            Texto = notaDto.Texto,
            DataEnvio = DateTime.UtcNow,
            AutorId = autor.Id,
            ChamadoId = chamadoId,
            IsNotaInterna = true, 
            LidoPeloCliente = false
        };

        await _mensagemRepository.AddAsync(novaNota);
        return novaNota;
    }
}