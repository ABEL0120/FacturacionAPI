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
    [Route("api/bases-datos")]
    public class BasesDatosController : ControllerBase
    {
        private readonly IBasesDatosService _basesDatosService;

        public BasesDatosController(IBasesDatosService basesDatosService)
        {
            _basesDatosService = basesDatosService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BaseDatosAppDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerBasesPorDefecto()
        {
            var respuesta = await _basesDatosService.ObtenerBasesPorAppAsync(null);
            return Ok(respuesta);
        }

        [HttpGet("{idApp}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BaseDatosAppDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerBasesPorApp(string idApp)
        {
            var respuesta = await _basesDatosService.ObtenerBasesPorAppAsync(idApp);
            return Ok(respuesta);
        }
    }
}

