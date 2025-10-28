using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Repositories
{
    public class ChamadoRepository : IChamadoRepository
    {
        private readonly CajuAjudaDbContext _context;
        private readonly ILogger<ChamadoRepository> _logger;

        public ChamadoRepository(CajuAjudaDbContext context, ILogger<ChamadoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Chamado chamado)
        {
            await _context.Chamados.AddAsync(chamado);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Chamado> chamados, int totalCount)> GetAllAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade)
        {
            var query = _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.TecnicoResponsavel)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (prioridade.HasValue)
            {
                query = query.Where(c => c.Prioridade == prioridade.Value);
            }

            var totalCount = await query.CountAsync();
            var chamados = await query
                .OrderByDescending(c => c.DataCriacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (chamados, totalCount);
        }

        public async Task<Chamado?> GetByIdAsync(long id)
        {
            _logger.LogInformation("🔍 [ChamadoRepository] Buscando chamado ID {ChamadoId} com AsNoTracking...", id);
            
            // Busca o chamado com AsNoTracking para evitar cache do EF
            var chamado = await _context.Chamados
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.TecnicoResponsavel)
                .Include(c => c.Mensagens.OrderBy(m => m.DataEnvio))
                    .ThenInclude(m => m.Autor)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (chamado != null)
            {
                var qtdMensagens = chamado.Mensagens?.Count ?? 0;
                _logger.LogInformation("📦 [ChamadoRepository] Chamado ID {ChamadoId} carregado com {QtdMensagens} mensagens", id, qtdMensagens);
            }
            else
            {
                _logger.LogWarning("⚠️ [ChamadoRepository] Chamado ID {ChamadoId} não encontrado!", id);
            }
            
            return chamado;
        }

        // CORREÇÃO: O tipo do parâmetro foi alterado de 'string' para 'long'
        public async Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId)
        {
            return await _context.Chamados
                .AsNoTracking()
                .Include(c => c.Mensagens.OrderBy(m => m.DataEnvio))
                    .ThenInclude(m => m.Autor)
                .Where(c => c.ClienteId == clienteId) 
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }

        public async Task UpdateAsync(Chamado chamado)
        {
            _context.Chamados.Update(chamado);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chamado>> GetByTecnicoIdAsync(long tecnicoId)
        {
            return await _context.Chamados
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Where(c => c.TecnicoResponsavelId == tecnicoId)
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> GetNaoAtribuidosAsync()
        {
            return await _context.Chamados
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Where(c => c.TecnicoResponsavelId == null)
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }
    }
}