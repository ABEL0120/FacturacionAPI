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
    [Route("api/catalogos-sat")]
    public class CatalogosSatController : ControllerBase
    {
        private readonly ICatalogosSatService _catalogosSatService;

        public CatalogosSatController(ICatalogosSatService catalogosSatService)
        {
            _catalogosSatService = catalogosSatService;
        }

        [HttpGet("regimenes-fiscales")]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerRegimenesFiscales()
        {
            try
            {
                return Ok(await _catalogosSatService.ObtenerRegimenesFiscalesAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<CatalogoItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("usos-cfdi")]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerUsosCfdi()
        {
            try
            {
                return Ok(await _catalogosSatService.ObtenerUsosCfdiAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<CatalogoItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("formas-pago")]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerFormasPago()
        {
            try
            {
                return Ok(await _catalogosSatService.ObtenerFormasPagoAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<CatalogoItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("monedas")]
        [ProducesResponseType(typeof(ApiResponse<List<MonedaCatalogoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<MonedaCatalogoDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerMonedas()
        {
            try
            {
                return Ok(await _catalogosSatService.ObtenerMonedasAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<MonedaCatalogoDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("estados")]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerEstados()
        {
            try
            {
                return Ok(await _catalogosSatService.ObtenerEstadosAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<List<CatalogoItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("motivos-cancelacion")]
        [ProducesResponseType(typeof(ApiResponse<List<CatalogoItemDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerMotivosCancelacion()
        {
            return Ok(await _catalogosSatService.ObtenerMotivosCancelacionAsync());
        }
    }
}

