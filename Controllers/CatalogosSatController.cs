using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/catalogos-sat")]
    public class CatalogosSatController : ControllerBase
    {
        private readonly ICatalogosSatService _catalogosSatService;
        private readonly ISatValidadorService _satValidadorService;

        public CatalogosSatController(ICatalogosSatService catalogosSatService, ISatValidadorService satValidadorService)
        {
            _catalogosSatService = catalogosSatService;
            _satValidadorService = satValidadorService;
        }

        [HttpGet("regimenes-fiscales")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerRegimenesFiscales()
        {
            var respuesta = await _catalogosSatService.ObtenerRegimenesFiscalesAsync();
            return Ok(respuesta);
        }

        [HttpGet("usos-cfdi")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerUsosCfdi()
        {
            var respuesta = await _catalogosSatService.ObtenerUsosCfdiAsync();
            return Ok(respuesta);
        }

        [HttpGet("formas-pago")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerFormasPago()
        {
            var respuesta = await _catalogosSatService.ObtenerFormasPagoAsync();
            return Ok(respuesta);
        }

        [HttpGet("estados")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerEstados()
        {
            var respuesta = await _catalogosSatService.ObtenerEstadosAsync();
            return Ok(respuesta);
        }

        [HttpGet("monedas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerMonedas()
        {
            var respuesta = await _catalogosSatService.ObtenerMonedasAsync();
            return Ok(respuesta);
        }

        [HttpGet("motivos-cancelacion")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerMotivosCancelacion()
        {
            var respuesta = await _catalogosSatService.ObtenerMotivosCancelacionAsync();
            return Ok(respuesta);
        }

        [HttpGet("validar-rfc/{rfc}")]
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
    }
}
