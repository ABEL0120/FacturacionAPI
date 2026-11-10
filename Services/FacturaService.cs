using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface IFacturaService
    {
        Task<ApiResponse<IEnumerable<FacturaItemDto>>> ConsultarFacturasClienteAsync(int idCliente);
        Task<ApiResponse<FacturaItemDto>> ObtenerFacturaPorIdAsync(string idCfdi);
        Task<ApiResponse<CancelarFacturaResponseDto>> CancelarFacturaAsync(CancelarFacturaRequestDto dto);
    }

    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;

        public FacturaService(IFacturaRepository facturaRepository)
        {
            _facturaRepository = facturaRepository;
        }

        public async Task<ApiResponse<IEnumerable<FacturaItemDto>>> ConsultarFacturasClienteAsync(int idCliente)
        {
            var facturas = await _facturaRepository.ConsultarFacturasClienteAsync(idCliente);
            var dtos = facturas.Select(MapearADto);

            return new ApiResponse<IEnumerable<FacturaItemDto>>
            {
                Success = true,
                Data = dtos
            };
        }

        public async Task<ApiResponse<FacturaItemDto>> ObtenerFacturaPorIdAsync(string idCfdi)
        {
            var factura = await _facturaRepository.ObtenerFacturaPorIdAsync(idCfdi);
            if (factura == null)
            {
                return new ApiResponse<FacturaItemDto>
                {
                    Success = false,
                    Message = "Factura no encontrada."
                };
            }

            return new ApiResponse<FacturaItemDto>
            {
                Success = true,
                Data = MapearADto(factura)
            };
        }

        public async Task<ApiResponse<CancelarFacturaResponseDto>> CancelarFacturaAsync(CancelarFacturaRequestDto dto)
        {
            string uuid = dto.Uuid.Trim();
            var factura = await _facturaRepository.ObtenerFacturaPorUuidAsync(uuid);

            if (factura == null)
            {
                return new ApiResponse<CancelarFacturaResponseDto>
                {
                    Success = false,
                    Message = "No se localizó la factura con el UUID especificado."
                };
            }

            if (dto.MotivoCancelacion == "01" && string.IsNullOrWhiteSpace(dto.UuidSustituye))
            {
                return new ApiResponse<CancelarFacturaResponseDto>
                {
                    Success = false,
                    Message = "Cuando el motivo es 01 se debe especificar el UUID sustituto."
                };
            }

            bool actualizado = await _facturaRepository.ActualizarEstatusCancelacionAsync(uuid, 1, "En Proceso Cancelacion");

            return new ApiResponse<CancelarFacturaResponseDto>
            {
                Success = actualizado,
                Message = actualizado ? "Solicitud de cancelación registrada correctamente." : "Error al registrar cancelación.",
                Data = new CancelarFacturaResponseDto
                {
                    Exitoso = actualizado,
                    Mensaje = actualizado ? "Cancelación en proceso." : "Error en base de datos."
                }
            };
        }

        private static FacturaItemDto MapearADto(CfdiFactura f)
        {
            return new FacturaItemDto
            {
                IdCFDI = f.idCFDI,
                Serie = f.Serie,
                Folio = f.Folio,
                RfcReceptor = f.RfcReceptor,
                IdCliente = f.idCliente,
                SubTotal = f.Sub_Total,
                IvaImporte = f.Iva_Importe,
                Total = f.Total,
                FechaTimbre = f.Fecha_Timbre,
                Orden = f.Orden,
                Sucursal = f.Sucursal,
                SucursalDesc = f.SucursalDesc,
                Uuid = f.uuid,
                EstatusCfdi = f.Estatus_CFDI,
                EstatusSat = f.Estatus_SAT,
                TipoDeComprobante = f.TipoDeComprobante
            };
        }
    }
}
