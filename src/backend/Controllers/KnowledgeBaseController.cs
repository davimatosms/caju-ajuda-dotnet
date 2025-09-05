
using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    public KnowledgeBaseController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    [HttpGet("categorias")] // Rota: GET api/knowledgebase/categorias
    public async Task<IActionResult> GetAllCategoriasComArtigos()
    {
        var categorias = await _service.GetAllCategoriasComArtigosAsync();
        return Ok(categorias);
    }

    [HttpGet("search")] // Rota: GET api/knowledgebase/search?termo=senha
    public async Task<IActionResult> SearchArtigos([FromQuery] string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
        {
            return BadRequest("O termo de busca não pode ser vazio.");
        }
        
        var artigos = await _service.SearchArtigosAsync(termo);
        return Ok(artigos);
    }
}