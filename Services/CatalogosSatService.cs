using System.Collections.Generic;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface ICatalogosSatService
    {
        Task<ApiResponse<List<CatalogoItemDto>>> ObtenerRegimenesFiscalesAsync();
        Task<ApiResponse<List<CatalogoItemDto>>> ObtenerUsosCfdiAsync();
        Task<ApiResponse<List<CatalogoItemDto>>> ObtenerFormasPagoAsync();
        Task<ApiResponse<List<MonedaCatalogoDto>>> ObtenerMonedasAsync();
        Task<ApiResponse<List<CatalogoItemDto>>> ObtenerEstadosAsync();
        Task<ApiResponse<List<CatalogoItemDto>>> ObtenerMotivosCancelacionAsync();
    }

    public class CatalogosSatService : ICatalogosSatService
    {
        private readonly ICatalogosSatRepository _catalogosSatRepository;

        public CatalogosSatService(ICatalogosSatRepository catalogosSatRepository)
        {
            _catalogosSatRepository = catalogosSatRepository;
        }

        public async Task<ApiResponse<List<CatalogoItemDto>>> ObtenerRegimenesFiscalesAsync()
        {
            var data = await _catalogosSatRepository.ObtenerRegimenesFiscalesAsync();
            return new ApiResponse<List<CatalogoItemDto>>
            {
                Success = true,
                Message = "Catálogo de regímenes fiscales obtenido correctamente.",
                Data = data
            };
        }

        public async Task<ApiResponse<List<CatalogoItemDto>>> ObtenerUsosCfdiAsync()
        {
            var data = await _catalogosSatRepository.ObtenerUsosCfdiAsync();
            return new ApiResponse<List<CatalogoItemDto>>
            {
                Success = true,
                Message = "Catálogo de usos de CFDI obtenido correctamente.",
                Data = data
            };
        }

        public async Task<ApiResponse<List<CatalogoItemDto>>> ObtenerFormasPagoAsync()
        {
            var data = await _catalogosSatRepository.ObtenerFormasPagoAsync();
            return new ApiResponse<List<CatalogoItemDto>>
            {
                Success = true,
                Message = "Catálogo de formas de pago obtenido correctamente.",
                Data = data
            };
        }

        public async Task<ApiResponse<List<MonedaCatalogoDto>>> ObtenerMonedasAsync()
        {
            var data = await _catalogosSatRepository.ObtenerMonedasAsync();
            return new ApiResponse<List<MonedaCatalogoDto>>
            {
                Success = true,
                Message = "Catálogo de monedas obtenido correctamente.",
                Data = data
            };
        }

        public async Task<ApiResponse<List<CatalogoItemDto>>> ObtenerEstadosAsync()
        {
            var data = await _catalogosSatRepository.ObtenerEstadosMexicoAsync();
            return new ApiResponse<List<CatalogoItemDto>>
            {
                Success = true,
                Message = "Catálogo de estados obtenido correctamente.",
                Data = data
            };
        }

        public Task<ApiResponse<List<CatalogoItemDto>>> ObtenerMotivosCancelacionAsync()
        {
            var motivos = new List<CatalogoItemDto>
            {
                new() { Numero = "01", Descripcion = "Comprobante emitido con errores con relación" },
                new() { Numero = "02", Descripcion = "Comprobante emitido con errores sin relación" },
                new() { Numero = "03", Descripcion = "No se llevó a cabo la operación" },
                new() { Numero = "04", Descripcion = "Operación nominativa relacionada en una factura global" }
            };

            return Task.FromResult(new ApiResponse<List<CatalogoItemDto>>
            {
                Success = true,
                Message = "Catálogo de motivos de cancelación obtenido correctamente.",
                Data = motivos
            });
        }
    }
}

