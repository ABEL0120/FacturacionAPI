using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface ICatalogosSatService
    {
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerRegimenesFiscalesAsync();
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerUsosCfdiAsync();
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerFormasPagoAsync();
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerEstadosAsync();
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerMonedasAsync();
        Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerMotivosCancelacionAsync();
    }

    public class CatalogosSatService : ICatalogosSatService
    {
        private readonly ICatalogosSatRepository _catalogosSatRepository;

        public CatalogosSatService(ICatalogosSatRepository catalogosSatRepository)
        {
            _catalogosSatRepository = catalogosSatRepository;
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerRegimenesFiscalesAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerRegimenesFiscalesAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerUsosCfdiAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerUsosCfdiAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerFormasPagoAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerFormasPagoAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerEstadosAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerEstadosAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerMonedasAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerMonedasAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        public async Task<ApiResponse<IEnumerable<CatalogoItemDto>>> ObtenerMotivosCancelacionAsync()
        {
            var datos = await _catalogosSatRepository.ObtenerMotivosCancelacionAsync();
            return new ApiResponse<IEnumerable<CatalogoItemDto>>
            {
                Success = true,
                Data = datos.Select(Mapear)
            };
        }

        private static CatalogoItemDto Mapear(CatalogoItem item)
        {
            return new CatalogoItemDto
            {
                Numero = item.Numero,
                Descripcion = item.Descripcion
            };
        }
    }
}
