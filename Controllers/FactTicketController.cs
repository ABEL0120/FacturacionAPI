using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/fact-ticket")]
    public class FactTicketController : ControllerBase
    {
        private readonly IFactTicketService _factTicketService;

        public FactTicketController(IFactTicketService factTicketService)
        {
            _factTicketService = factTicketService;
        }

        [HttpGet("inicializar/{numeroCliente:int}")]
        [ProducesResponseType(typeof(ApiResponse<FactTicketInitResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<FactTicketInitResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<FactTicketInitResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Inicializar(int numeroCliente, [FromQuery] string? rfc = null)
        {
            try
            {
                var respuesta = await _factTicketService.InicializarAsync(numeroCliente, rfc);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<FactTicketInitResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("ticket/consultar")]
        [ProducesResponseType(typeof(ApiResponse<TicketConsultaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<TicketConsultaResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<TicketConsultaResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConsultarTicket([FromBody] ConsultarTicketRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<TicketConsultaResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _factTicketService.ConsultarTicketAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("ticket/validar")]
        [ProducesResponseType(typeof(ApiResponse<ValidarTicketsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ValidarTicketsResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ValidarTicketsResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidarTickets([FromBody] ValidarTicketsRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<ValidarTicketsResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _factTicketService.ValidarTicketsAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<ValidarTicketsResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("rfc")]
        [ProducesResponseType(typeof(ApiResponse<BuscarRfcResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<BuscarRfcResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<BuscarRfcResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuscarRfc([FromQuery] string rfc, [FromQuery] int? numeroCliente = null)
        {
            try
            {
                var respuesta = await _factTicketService.BuscarRfcAsync(rfc, numeroCliente);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<BuscarRfcResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("cliente/{numeroCliente:int}/rfcs")]
        [ProducesResponseType(typeof(ApiResponse<List<ClienteRfcItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ClienteRfcItemDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<ClienteRfcItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarRfcsCliente(int numeroCliente)
        {
            try
            {
                var respuesta = await _factTicketService.ListarRfcsClienteAsync(numeroCliente);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<ClienteRfcItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("rfc")]
        [ProducesResponseType(typeof(ApiResponse<ClienteFiscalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClienteFiscalDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ClienteFiscalDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarRfc([FromBody] RegistrarRfcRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<ClienteFiscalDto> { Success = false, Message = errores });
                }

                var respuesta = await _factTicketService.RegistrarRfcAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("vista-previa")]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VistaPrevia([FromBody] GenerarFacturaTicketRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<FacturaTicketResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _factTicketService.VistaPreviaFacturaAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("generar")]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<FacturaTicketResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerarFactura([FromBody] GenerarFacturaTicketRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<FacturaTicketResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _factTicketService.GenerarFacturaAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}

