using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface ITicketService
    {
        Task<ApiResponse<ResumenTicketDto>> ConsultarTicketAsync(ConsultaTicketRequestDto dto);
        Task<ApiResponse<ValidacionTicketsResponseDto>> ValidarTicketsParaFacturacionAsync(ValidarTicketsRequestDto dto);
    }

    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<ApiResponse<ResumenTicketDto>> ConsultarTicketAsync(ConsultaTicketRequestDto dto)
        {
            string folio = (dto.Folio ?? string.Empty).Trim();
            long transaccion = dto.Transaccion;

            if (string.IsNullOrWhiteSpace(folio))
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "Capture un Folio."
                };
            }

            if (transaccion <= 0)
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "Capture la referencia o transacción."
                };
            }

            var validacion = (await _ticketRepository.ObtenerValidacionTicketAsync(folio, transaccion)).ToList();
            if (!validacion.Any())
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "No se encuentra el ticket especificado."
                };
            }

            var primerRegistro = validacion.First();

            if (!string.IsNullOrWhiteSpace(primerRegistro.uuid) && string.IsNullOrWhiteSpace(primerRegistro.EsGlobal))
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "El Ticket ya ha sido Facturado."
                };
            }

            if (primerRegistro.Baja == "*")
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "El ticket fue eliminado."
                };
            }

            string prefijoMes = DateTime.Now.ToString("yyyy/MM", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(primerRegistro.Fecha) && !primerRegistro.Fecha.Contains(prefijoMes))
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "Solo se pueden facturar tickets del mes en curso."
                };
            }

            var ordenes = (await _ticketRepository.ObtenerOrdenesTicketAsync(folio, transaccion)).ToList();
            if (!ordenes.Any())
            {
                return new ApiResponse<ResumenTicketDto>
                {
                    Success = false,
                    Message = "No se encontraron partidas válidas para el ticket."
                };
            }

            var partidas = ordenes.Select(o => new TicketPartidaDto
            {
                Numero = o.Numero,
                Folio = o.Folio,
                Transaccion = o.Transaccion,
                Sucursal = o.Sucursal,
                Fecha = o.Fecha,
                Producto = o.Producto,
                Descripcion = o.Descripcion,
                ClaveProdServSat = o.ClaveProdServ_SAT,
                ClaveProdServDesc = o.ClaveProdServDesc,
                UnidadMedidaSat = o.Unidad_Medida_SAT,
                UnidadMedida = o.Unidad_Medida,
                Cantidad = 1,
                Precio = o.Precio,
                PorcentajeIva = o.Porcentaje_Iva,
                PrecioSinIva = o.PrecioSinIva,
                Subtotal = o.Subtotal,
                Iva = o.ImpIva,
                Descuento = o.Descuento,
                Total = o.TotalFacturar > 0 ? o.TotalFacturar : o.Total
            }).ToList();

            var resumen = new ResumenTicketDto
            {
                Folio = folio,
                Transaccion = transaccion,
                Sucursal = primerRegistro.Sucursal,
                Fecha = primerRegistro.Fecha,
                Subtotal = partidas.Sum(p => p.Subtotal),
                Iva = partidas.Sum(p => p.Iva),
                Descuento = partidas.Sum(p => p.Descuento),
                Total = partidas.Sum(p => p.Total),
                Partidas = partidas
            };

            return new ApiResponse<ResumenTicketDto>
            {
                Success = true,
                Message = "Ticket consultado con éxito.",
                Data = resumen
            };
        }

        public async Task<ApiResponse<ValidacionTicketsResponseDto>> ValidarTicketsParaFacturacionAsync(ValidarTicketsRequestDto dto)
        {
            if (dto.Tickets == null || !dto.Tickets.Any())
            {
                return new ApiResponse<ValidacionTicketsResponseDto>
                {
                    Success = false,
                    Message = "No se proporcionaron tickets para validar."
                };
            }

            int sucursalReferencia = 0;
            var todasLasPartidas = new List<TicketPartidaDto>();

            foreach (var t in dto.Tickets)
            {
                var consulta = await ConsultarTicketAsync(t);
                if (!consulta.Success || consulta.Data == null)
                {
                    return new ApiResponse<ValidacionTicketsResponseDto>
                    {
                        Success = false,
                        Message = $"Error en ticket {t.Folio}: {consulta.Message}"
                    };
                }

                if (sucursalReferencia == 0)
                {
                    sucursalReferencia = consulta.Data.Sucursal;
                }
                else if (consulta.Data.Sucursal != sucursalReferencia)
                {
                    return new ApiResponse<ValidacionTicketsResponseDto>
                    {
                        Success = false,
                        Message = "Los tickets deben pertenecer a la misma Sucursal."
                    };
                }

                todasLasPartidas.AddRange(consulta.Data.Partidas);
            }

            var respuesta = new ValidacionTicketsResponseDto
            {
                Valido = true,
                Mensaje = "Tickets listos y validados para facturar.",
                Sucursal = sucursalReferencia,
                SubtotalTotal = todasLasPartidas.Sum(p => p.Subtotal),
                IvaTotal = todasLasPartidas.Sum(p => p.Iva),
                DescuentoTotal = todasLasPartidas.Sum(p => p.Descuento),
                GranTotal = todasLasPartidas.Sum(p => p.Total),
                PartidasAgrupadas = todasLasPartidas
            };

            return new ApiResponse<ValidacionTicketsResponseDto>
            {
                Success = true,
                Message = "Validación completada.",
                Data = respuesta
            };
        }
    }
}
