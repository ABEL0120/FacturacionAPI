using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/cfdi-consulta")]
    public class CfdiConsultaController : ControllerBase
    {
        private readonly ICfdiConsultaService _cfdiConsultaService;

        public CfdiConsultaController(ICfdiConsultaService cfdiConsultaService)
        {
            _cfdiConsultaService = cfdiConsultaService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CfdiConsultaItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<CfdiConsultaItemDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<CfdiConsultaItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Consultar(
            [FromQuery] int numeroCliente,
            [FromQuery] string? serie = null,
            [FromQuery] string? folio = null)
        {
            try
            {
                var respuesta = await _cfdiConsultaService.ConsultarAsync(numeroCliente, serie, folio);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<CfdiConsultaItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{idCfdi}/archivo")]
        public async Task<IActionResult> DescargarArchivo(
            string idCfdi,
            [FromQuery] string tipo,
            [FromQuery] int? numeroCliente = null,
            [FromQuery] bool inline = false)
        {
            try
            {
                var resultado = await _cfdiConsultaService.ObtenerArchivoAsync(idCfdi, tipo, numeroCliente);
                if (!resultado.Success)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = resultado.Message
                    });
                }

                if (inline)
                {
                    Response.Headers["Content-Disposition"] = $"inline; filename=\"{resultado.FileName}\"";
                    return File(resultado.Bytes, resultado.ContentType);
                }

                return File(resultado.Bytes, resultado.ContentType, resultado.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("{idCfdi}/enviar-correo")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EnviarCorreo(string idCfdi, [FromBody] EnviarCorreoCfdiRequestDto dto)
        {
            try
            {
                var respuesta = await _cfdiConsultaService.EnviarCorreoAsync(
                    idCfdi,
                    dto?.NumeroCliente ?? 0,
                    dto?.CorreoDestino);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }
    }
}
