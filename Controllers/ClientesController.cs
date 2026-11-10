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
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ISatValidadorService _satValidadorService;

        public ClientesController(IClienteService clienteService, ISatValidadorService satValidadorService)
        {
            _clienteService = clienteService;
            _satValidadorService = satValidadorService;
        }

        [HttpGet("{numeroCliente:int}")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPorId(int numeroCliente)
        {
            var respuesta = await _clienteService.ObtenerPorIdAsync(numeroCliente);
            if (!respuesta.Success)
            {
                return NotFound(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("{numeroCliente:int}/rfcs")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClienteRfcItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerRfcs(int numeroCliente)
        {
            var respuesta = await _clienteService.ObtenerRfcsPorClienteAsync(numeroCliente);
            return Ok(respuesta);
        }

        [HttpGet("{numeroCliente:int}/rfc/{rfc}")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPorRfc(int numeroCliente, string rfc)
        {
            var respuesta = await _clienteService.ObtenerPorRfcAsync(numeroCliente, rfc);
            if (!respuesta.Success)
            {
                return NotFound(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("validar-sat/{rfc}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidarRfcSat(string rfc)
        {
            var respuesta = await _satValidadorService.ValidarRfcSatAsync(rfc);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("rfcs")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarRfc([FromBody] CrearClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<ClienteDto> { Success = false, Message = errores });
            }

            var respuesta = await _clienteService.RegistrarRfcAsync(dto);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarCliente([FromBody] ActualizarClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ApiResponse<bool> { Success = false, Message = errores });
            }

            var respuesta = await _clienteService.ActualizarClienteAsync(dto);
            if (!respuesta.Success)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
