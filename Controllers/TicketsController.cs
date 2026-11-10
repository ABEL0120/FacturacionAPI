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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("consultar")]
        [ProducesResponseType(typeof(ApiResponse<ResumenTicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ResumenTicketDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Consultar([FromBody] ConsultaTicketRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<ResumenTicketDto> { Success = false, Message = errores });
            }

            var respuesta = await _ticketService.ConsultarTicketAsync(dto);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("validar")]
        [ProducesResponseType(typeof(ApiResponse<ValidacionTicketsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ValidacionTicketsResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Validar([FromBody] ValidarTicketsRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<ValidacionTicketsResponseDto> { Success = false, Message = errores });
            }

            var respuesta = await _ticketService.ValidarTicketsParaFacturacionAsync(dto);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}

