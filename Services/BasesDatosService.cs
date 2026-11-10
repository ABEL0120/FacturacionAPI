using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FacturacionAPI.DTOs;
using FacturacionAPI.Helpers;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface IBasesDatosService
    {
        Task<ApiResponse<IEnumerable<BaseDatosAppDto>>> ObtenerBasesPorAppAsync(string? idApp);
    }

    public class BasesDatosService : IBasesDatosService
    {
        private readonly IBasesDatosRepository _basesDatosRepository;
        private readonly string _defaultIdApp;

        public BasesDatosService(IBasesDatosRepository basesDatosRepository, IConfiguration configuration)
        {
            _basesDatosRepository = basesDatosRepository;
            _defaultIdApp = configuration["AppConfig:IdApp"] ?? "WebPuntoVentaTiendas2024";
        }

        public async Task<ApiResponse<IEnumerable<BaseDatosAppDto>>> ObtenerBasesPorAppAsync(string? idApp)
        {
            string appTarget = string.IsNullOrWhiteSpace(idApp) ? _defaultIdApp : idApp.Trim();
            var bases = await _basesDatosRepository.ObtenerBasesPorAppAsync(appTarget);

            var lista = bases.Select(b => new BaseDatosAppDto
            {
                Numero = b.Numero,
                IdApp = b.idAPP,
                Nombre = EncryptionHelper.Desencriptar(b.Nombre),
                Servidor = EncryptionHelper.Desencriptar(b.Servidor),
                NombreBase = EncryptionHelper.Desencriptar(b.Nombre_Base),
                Usuario = EncryptionHelper.Desencriptar(b.Usuario),
                Acceso = EncryptionHelper.Desencriptar(b.Acceso)
            });

            return new ApiResponse<IEnumerable<BaseDatosAppDto>>
            {
                Success = true,
                Data = lista
            };
        }
    }
}
