using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "TECNICO,ADMIN")] // Apenas técnicos e admins podem ver as respostas prontas
public class RespostasProntasController : ControllerBase
{
    private readonly IRespostaProntaService _service;

    public RespostasProntasController(IRespostaProntaService service)
    {
        _service = service;
    }

    [HttpGet] // Rota: GET api/respostasprontas
    public async Task<IActionResult> GetAll()
    {
        var respostas = await _service.GetAllAsync();
        return Ok(respostas);
    }
}