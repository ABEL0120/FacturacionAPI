using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface IClienteService
    {
        Task<ApiResponse<ClienteDto>> ObtenerPorIdAsync(int numeroCliente);
        Task<ApiResponse<ClienteDto>> ObtenerPorRfcAsync(int numeroCliente, string rfc);
        Task<ApiResponse<IEnumerable<ClienteRfcItemDto>>> ObtenerRfcsPorClienteAsync(int numeroCliente);
        Task<ApiResponse<ClienteDto>> RegistrarRfcAsync(CrearClienteDto dto);
        Task<ApiResponse<bool>> ActualizarClienteAsync(ActualizarClienteDto dto);
    }

    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ApiResponse<ClienteDto>> ObtenerPorIdAsync(int numeroCliente)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(numeroCliente);
            if (cliente == null)
            {
                return new ApiResponse<ClienteDto>
                {
                    Success = false,
                    Message = "Cliente no encontrado."
                };
            }

            return new ApiResponse<ClienteDto>
            {
                Success = true,
                Data = MapearADto(cliente)
            };
        }

        public async Task<ApiResponse<ClienteDto>> ObtenerPorRfcAsync(int numeroCliente, string rfc)
        {
            var cliente = await _clienteRepository.ObtenerPorRfcAsync(numeroCliente, rfc);
            if (cliente == null)
            {
                return new ApiResponse<ClienteDto>
                {
                    Success = false,
                    Message = $"No se encontró registro para el RFC '{rfc}'."
                };
            }

            return new ApiResponse<ClienteDto>
            {
                Success = true,
                Data = MapearADto(cliente)
            };
        }

        public async Task<ApiResponse<IEnumerable<ClienteRfcItemDto>>> ObtenerRfcsPorClienteAsync(int numeroCliente)
        {
            var clientes = await _clienteRepository.ObtenerRfcsPorClienteAsync(numeroCliente);
            var lista = clientes.Select(c => new ClienteRfcItemDto
            {
                NumeroCliente = c.Numero_Cliente,
                RFC = c.RFC,
                RazonSocial = c.Razon_Social
            });

            return new ApiResponse<IEnumerable<ClienteRfcItemDto>>
            {
                Success = true,
                Data = lista
            };
        }

        public async Task<ApiResponse<ClienteDto>> RegistrarRfcAsync(CrearClienteDto dto)
        {
            string rfc = dto.RFC.Trim().ToUpperInvariant();
            string razonSocial = dto.RazonSocial.Trim().ToUpperInvariant();

            bool existe = await _clienteRepository.ExisteClienteRfcAsync(dto.NumeroCliente, rfc);
            if (existe)
            {
                return new ApiResponse<ClienteDto>
                {
                    Success = false,
                    Message = $"Ya existe un registro con el cliente '{dto.NumeroCliente}' y RFC '{rfc}'."
                };
            }

            var nuevoCliente = new Cliente
            {
                Numero_Cliente = dto.NumeroCliente,
                Razon_Social = razonSocial,
                RFC = rfc,
                Direccion = (dto.Direccion ?? string.Empty).Trim(),
                Numero_Ext = (dto.NumeroExt ?? string.Empty).Trim(),
                Numero_Int = (dto.NumeroInt ?? string.Empty).Trim(),
                Colonia = (dto.Colonia ?? string.Empty).Trim(),
                Ciudad = (dto.Ciudad ?? string.Empty).Trim(),
                Delegacion = (dto.Delegacion ?? string.Empty).Trim(),
                Estado = (dto.Estado ?? "COAHUILA").Trim(),
                Codigo_Postal = dto.CodigoPostal.Trim(),
                Telefono = (dto.Telefono ?? string.Empty).Trim(),
                Mail = (dto.Mail ?? string.Empty).Trim(),
                SAT_RegimenFiscal = (dto.RegimenFiscal ?? "612").Trim(),
                SAT_FORMA_PAGO = (dto.FormaPago ?? "01").Trim(),
                Sat_Uso_Cfdi = (dto.UsoCfdi ?? "G03").Trim(),
                ValidadoReceptor = "S"
            };

            bool insertado = await _clienteRepository.InsertarClienteAsync(nuevoCliente);
            if (!insertado)
            {
                return new ApiResponse<ClienteDto>
                {
                    Success = false,
                    Message = "Error al registrar el RFC para el cliente."
                };
            }

            return new ApiResponse<ClienteDto>
            {
                Success = true,
                Message = "RFC registrado exitosamente.",
                Data = MapearADto(nuevoCliente)
            };
        }

        public async Task<ApiResponse<bool>> ActualizarClienteAsync(ActualizarClienteDto dto)
        {
            var cliente = new Cliente
            {
                Numero_Cliente = dto.NumeroCliente,
                Razon_Social = dto.RazonSocial.Trim().ToUpperInvariant(),
                RFC = dto.RFC.Trim().ToUpperInvariant(),
                Direccion = (dto.Direccion ?? string.Empty).Trim(),
                Numero_Ext = (dto.NumeroExt ?? string.Empty).Trim(),
                Numero_Int = (dto.NumeroInt ?? string.Empty).Trim(),
                Colonia = (dto.Colonia ?? string.Empty).Trim(),
                Ciudad = (dto.Ciudad ?? string.Empty).Trim(),
                Delegacion = (dto.Delegacion ?? string.Empty).Trim(),
                Estado = (dto.Estado ?? string.Empty).Trim(),
                Codigo_Postal = dto.CodigoPostal.Trim(),
                Telefono = (dto.Telefono ?? string.Empty).Trim(),
                Mail = (dto.Mail ?? string.Empty).Trim(),
                SAT_RegimenFiscal = (dto.RegimenFiscal ?? string.Empty).Trim(),
                SAT_FORMA_PAGO = (dto.FormaPago ?? string.Empty).Trim(),
                Sat_Uso_Cfdi = (dto.UsoCfdi ?? string.Empty).Trim()
            };

            bool actualizado = await _clienteRepository.ActualizarClienteAsync(cliente);
            return new ApiResponse<bool>
            {
                Success = actualizado,
                Message = actualizado ? "Datos fiscales actualizados con éxito." : "No se pudo actualizar el cliente.",
                Data = actualizado
            };
        }

        private static ClienteDto MapearADto(Cliente c)
        {
            return new ClienteDto
            {
                NumeroCliente = c.Numero_Cliente,
                RazonSocial = c.Razon_Social,
                RFC = c.RFC,
                Direccion = c.Direccion,
                NumeroExt = c.Numero_Ext,
                NumeroInt = c.Numero_Int,
                Colonia = c.Colonia,
                Ciudad = c.Ciudad,
                Delegacion = c.Delegacion,
                Estado = c.Estado,
                CodigoPostal = c.Codigo_Postal,
                Telefono = c.Telefono,
                Mail = c.Mail,
                RegimenFiscal = c.SAT_RegimenFiscal,
                FormaPago = c.SAT_FORMA_PAGO,
                UsoCfdi = c.Sat_Uso_Cfdi,
                ValidadoReceptor = c.ValidadoReceptor
            };
        }
    }
}
