using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturasController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet("cliente/{idCliente:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FacturaItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConsultarPorCliente(int idCliente)
        {
            var respuesta = await _facturaService.ConsultarFacturasClienteAsync(idCliente);
            return Ok(respuesta);
        }

        [HttpGet("{idCfdi}")]
        [ProducesResponseType(typeof(ApiResponse<FacturaItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<FacturaItemDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPorId(string idCfdi)
        {
            var respuesta = await _facturaService.ObtenerFacturaPorIdAsync(idCfdi);
            if (!respuesta.Success)
            {
                return NotFound(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("cancelar")]
        [ProducesResponseType(typeof(ApiResponse<CancelarFacturaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<CancelarFacturaResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancelar([FromBody] CancelarFacturaRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<CancelarFacturaResponseDto> { Success = false, Message = errores });
            }

            var respuesta = await _facturaService.CancelarFacturaAsync(dto);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}

