using System.ComponentModel.DataAnnotations;
using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoUpdateStatusDto
{
    [Required(ErrorMessage = "O novo status é obrigatório.")]
    // Garante que o valor enviado corresponde a um dos valores do Enum StatusChamado
    [EnumDataType(typeof(StatusChamado), ErrorMessage = "Status inválido.")]
    public StatusChamado NovoStatus { get; set; }
}