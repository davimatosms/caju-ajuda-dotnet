using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")] // Protege o controller inteiro apenas para Admins
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("tecnicos")] // Rota: GET api/admin/tecnicos
    public async Task<IActionResult> GetAllTecnicos()
    {
        var tecnicos = await _adminService.GetAllTecnicosAsync();
        return Ok(tecnicos);
    }
}