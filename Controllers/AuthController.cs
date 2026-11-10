using System;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<LoginResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _authService.LoginAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("registro")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] RegistroClienteRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<LoginResponseDto> { Success = false, Message = errores });
                }

                var respuesta = await _authService.RegistrarAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("validar-cuenta-recuperacion")]
        [ProducesResponseType(typeof(ApiResponse<CuentaRecuperacionValidaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<CuentaRecuperacionValidaDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<CuentaRecuperacionValidaDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidarCuentaRecuperacion([FromBody] ValidarCuentaRecuperacionRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<CuentaRecuperacionValidaDto> { Success = false, Message = errores });
                }

                var respuesta = await _authService.ValidarCuentaRecuperacionAsync(dto);
                if (!respuesta.Success)
                {
                    return BadRequest(respuesta);
                }

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<CuentaRecuperacionValidaDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("recuperar-contrasena")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecuperarContrasena([FromBody] RecuperarContrasenaRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<bool> { Success = false, Message = errores });
                }

                var respuesta = await _authService.RecuperarContrasenaAsync(dto);
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
                    Message = ex.Message
                });
            }
        }

        [HttpPost("cambiar-contrasena")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ApiResponse<bool> { Success = false, Message = errores });
                }

                var respuesta = await _authService.CambiarContrasenaAsync(dto);
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
                    Message = ex.Message
                });
            }
        }
    }
}
