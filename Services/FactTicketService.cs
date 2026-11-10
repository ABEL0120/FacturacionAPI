using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface IFactTicketService
    {
        Task<ApiResponse<FactTicketInitResponseDto>> InicializarAsync(int numeroCliente, string? rfcSeleccionado = null);
        Task<ApiResponse<TicketConsultaResponseDto>> ConsultarTicketAsync(ConsultarTicketRequestDto dto);
        Task<ApiResponse<ValidarTicketsResponseDto>> ValidarTicketsAsync(ValidarTicketsRequestDto dto);
        Task<ApiResponse<BuscarRfcResponseDto>> BuscarRfcAsync(string rfc, int? numeroCliente = null);
        Task<ApiResponse<List<ClienteRfcItemDto>>> ListarRfcsClienteAsync(int numeroCliente);
        Task<ApiResponse<ClienteFiscalDto>> RegistrarRfcAsync(RegistrarRfcRequestDto dto);
        Task<ApiResponse<FacturaTicketResponseDto>> VistaPreviaFacturaAsync(GenerarFacturaTicketRequestDto dto);
        Task<ApiResponse<FacturaTicketResponseDto>> GenerarFacturaAsync(GenerarFacturaTicketRequestDto dto);
    }

    public class FactTicketService : IFactTicketService
    {
        private readonly IFactTicketRepository _factTicketRepository;
        private readonly IClienteFiscalRepository _clienteFiscalRepository;
        private readonly ICatalogosSatRepository _catalogosSatRepository;

        public FactTicketService(
            IFactTicketRepository factTicketRepository,
            IClienteFiscalRepository clienteFiscalRepository,
            ICatalogosSatRepository catalogosSatRepository)
        {
            _factTicketRepository = factTicketRepository;
            _clienteFiscalRepository = clienteFiscalRepository;
            _catalogosSatRepository = catalogosSatRepository;
        }

        public async Task<ApiResponse<FactTicketInitResponseDto>> InicializarAsync(int numeroCliente, string? rfcSeleccionado = null)
        {
            var compania = await _factTicketRepository.ObtenerCompaniaActivaAsync();
            if (compania == null)
            {
                return new ApiResponse<FactTicketInitResponseDto>
                {
                    Success = false,
                    Message = "No se encontró información de la compañía emisora."
                };
            }

            var monedasTask = _catalogosSatRepository.ObtenerMonedasAsync();
            var regimenesTask = _catalogosSatRepository.ObtenerRegimenesFiscalesAsync();
            var usosTask = _catalogosSatRepository.ObtenerUsosCfdiAsync();
            var formasPagoTask = _catalogosSatRepository.ObtenerFormasPagoAsync();
            var estadosTask = _catalogosSatRepository.ObtenerEstadosMexicoAsync();
            var rfcsTask = numeroCliente > 0
                ? _clienteFiscalRepository.ObtenerRfcsPorClienteAsync(numeroCliente)
                : Task.FromResult(new List<ClienteRfcItemDto>());

            await Task.WhenAll(monedasTask, regimenesTask, usosTask, formasPagoTask, estadosTask, rfcsTask);

            var rfcsCliente = rfcsTask.Result;
            ClienteFiscalDto? clienteSeleccionado = null;
            string rfcObjetivo = !string.IsNullOrWhiteSpace(rfcSeleccionado)
                ? rfcSeleccionado.Trim().ToUpperInvariant()
                : rfcsCliente.FirstOrDefault()?.RFC ?? string.Empty;

            if (numeroCliente > 0 && !string.IsNullOrWhiteSpace(rfcObjetivo))
            {
                var cliente = await _clienteFiscalRepository.ObtenerPorNumeroClienteYRfcAsync(numeroCliente, rfcObjetivo);
                if (cliente != null)
                {
                    string mailAcceso = await _clienteFiscalRepository.ObtenerMailAccesoAsync(numeroCliente) ?? string.Empty;
                    clienteSeleccionado = MapearClienteDto(cliente, mailAcceso);
                }
            }

            return new ApiResponse<FactTicketInitResponseDto>
            {
                Success = true,
                Message = "Módulo de facturación de ticket inicializado.",
                Data = new FactTicketInitResponseDto
                {
                    EsPrueba = compania.Prueba.Trim().Equals("S", StringComparison.OrdinalIgnoreCase),
                    EmisorRfc = compania.RFC.Trim(),
                    EmisorRazonSocial = compania.Razon_Social.Trim(),
                    EmisorCp = compania.CP.Trim(),
                    EmisorRegimenFiscal = compania.CveRegimenFiscal.Trim(),
                    RfcsCliente = rfcsCliente,
                    ClienteSeleccionado = clienteSeleccionado,
                    Monedas = monedasTask.Result,
                    RegimenesFiscales = regimenesTask.Result,
                    UsosCfdi = usosTask.Result,
                    FormasPago = formasPagoTask.Result,
                    EstadosMexico = estadosTask.Result
                }
            };
        }

        public async Task<ApiResponse<TicketConsultaResponseDto>> ConsultarTicketAsync(ConsultarTicketRequestDto dto)
        {
            string mailUsuario = await ResolverMailUsuarioAsync(dto.MailUsuario, dto.NumeroCliente);
            var resultado = await ObtenerYValidarTicketIndividualAsync(dto, mailUsuario);
            if (!resultado.Success || resultado.Data == null)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = resultado.Message
                };
            }

            return resultado;
        }

        public async Task<ApiResponse<ValidarTicketsResponseDto>> ValidarTicketsAsync(ValidarTicketsRequestDto dto)
        {
            if (dto.Tickets == null || dto.Tickets.Count == 0)
            {
                return new ApiResponse<ValidarTicketsResponseDto>
                {
                    Success = false,
                    Message = "Debe capturar al menos un ticket."
                };
            }

            string mailUsuario = await ResolverMailUsuarioAsync(dto.MailUsuario, dto.NumeroCliente);
            var validacion = await ValidarGrupoTicketsAsync(dto.Tickets, dto.NumeroCliente, mailUsuario);
            if (!validacion.Success || validacion.Response == null)
            {
                return new ApiResponse<ValidarTicketsResponseDto>
                {
                    Success = false,
                    Message = validacion.Message
                };
            }

            return new ApiResponse<ValidarTicketsResponseDto>
            {
                Success = true,
                Message = "Tickets validados correctamente.",
                Data = validacion.Response
            };
        }

        public async Task<ApiResponse<BuscarRfcResponseDto>> BuscarRfcAsync(string rfc, int? numeroCliente = null)
        {
            string rfcLimpio = QuitaDobleEspacio(rfc).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(rfcLimpio))
            {
                return new ApiResponse<BuscarRfcResponseDto>
                {
                    Success = false,
                    Message = "RFC inválido."
                };
            }

            if (!EsRfcValido(rfcLimpio))
            {
                return new ApiResponse<BuscarRfcResponseDto>
                {
                    Success = false,
                    Message = "El formato del RFC no es válido."
                };
            }

            Cliente? cliente = numeroCliente.HasValue && numeroCliente.Value > 0
                ? await _clienteFiscalRepository.ObtenerPorNumeroClienteYRfcAsync(numeroCliente.Value, rfcLimpio)
                : await _clienteFiscalRepository.ObtenerPorRfcGeneralAsync(rfcLimpio);

            if (cliente == null)
            {
                return new ApiResponse<BuscarRfcResponseDto>
                {
                    Success = true,
                    Message = $"El RFC {rfcLimpio} no se encuentra registrado. Debe completar el registro.",
                    Data = new BuscarRfcResponseDto
                    {
                        Existe = false,
                        RequiereRegistro = true,
                        RFC = rfcLimpio,
                        Cliente = null
                    }
                };
            }

            string mailAcceso = cliente.Numero_Cliente > 0
                ? await _clienteFiscalRepository.ObtenerMailAccesoAsync(cliente.Numero_Cliente) ?? string.Empty
                : string.Empty;

            return new ApiResponse<BuscarRfcResponseDto>
            {
                Success = true,
                Message = "Información fiscal del RFC cargada correctamente.",
                Data = new BuscarRfcResponseDto
                {
                    Existe = true,
                    RequiereRegistro = false,
                    RFC = rfcLimpio,
                    Cliente = MapearClienteDto(cliente, mailAcceso)
                }
            };
        }

        public async Task<ApiResponse<List<ClienteRfcItemDto>>> ListarRfcsClienteAsync(int numeroCliente)
        {
            if (numeroCliente <= 0)
            {
                return new ApiResponse<List<ClienteRfcItemDto>>
                {
                    Success = false,
                    Message = "Número de cliente inválido."
                };
            }

            var rfcs = await _clienteFiscalRepository.ObtenerRfcsPorClienteAsync(numeroCliente);
            return new ApiResponse<List<ClienteRfcItemDto>>
            {
                Success = true,
                Message = "RFCs del cliente obtenidos correctamente.",
                Data = rfcs
            };
        }

        public async Task<ApiResponse<ClienteFiscalDto>> RegistrarRfcAsync(RegistrarRfcRequestDto dto)
        {
            if (dto.NumeroCliente <= 0)
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "Cliente inválido."
                };
            }

            string rfc = QuitaDobleEspacio(dto.RFC).ToUpperInvariant();
            string razonSocial = QuitaDobleEspacio(dto.RazonSocial).ToUpperInvariant();
            string codigoPostal = (dto.CodigoPostal ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(rfc) || !EsRfcValido(rfc))
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "RFC inválido."
                };
            }

            if (string.IsNullOrWhiteSpace(razonSocial))
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "Razón Social inválida."
                };
            }

            if (string.IsNullOrWhiteSpace(codigoPostal))
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "CP inválido."
                };
            }

            bool existe = await _clienteFiscalRepository.ExisteClienteRfcAsync(dto.NumeroCliente, rfc);
            if (existe)
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = $"Ya existe un registro con el Id.: {dto.NumeroCliente} y con el RFC: {rfc}"
                };
            }

            var regimen = await _catalogosSatRepository.ObtenerRegimenFiscalPorNumeroAsync(dto.RegimenFiscal);
            if (regimen == null)
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "El régimen fiscal seleccionado no existe en el catálogo SAT."
                };
            }

            var usoCfdi = await _catalogosSatRepository.ObtenerUsoCfdiPorNumeroAsync(dto.UsoCfdi);
            if (usoCfdi == null)
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "El uso de CFDI seleccionado no existe en el catálogo SAT."
                };
            }

            string estadoNombre = (dto.Estado ?? "Coahuila").Trim();
            if (!string.IsNullOrWhiteSpace(estadoNombre))
            {
                string? resuelto = await _catalogosSatRepository.ResolverNombreEstadoAsync(estadoNombre);
                if (!string.IsNullOrWhiteSpace(resuelto))
                {
                    estadoNombre = resuelto;
                }
            }

            string mailAcceso = await _clienteFiscalRepository.ObtenerMailAccesoAsync(dto.NumeroCliente) ?? string.Empty;
            string mailFinal = string.IsNullOrWhiteSpace(dto.Mail) ? mailAcceso : dto.Mail.Trim();

            var nuevoCliente = new Cliente
            {
                Numero_Cliente = dto.NumeroCliente,
                Razon_Social = razonSocial,
                RFC = rfc,
                Direccion = (dto.Direccion ?? string.Empty).Trim(),
                Numero_Ext = (dto.NumeroExt ?? string.Empty).Trim(),
                Numero_Int = (dto.NumeroInt ?? string.Empty).Trim(),
                Colonia = (dto.Colonia ?? string.Empty).Trim(),
                Delegacion = (dto.Delegacion ?? string.Empty).Trim(),
                Ciudad = (dto.Ciudad ?? string.Empty).Trim(),
                Estado = estadoNombre,
                Codigo_Postal = codigoPostal,
                Telefono = (dto.Telefono ?? string.Empty).Trim(),
                Mail = mailFinal,
                SAT_RegimenFiscal = regimen.Numero,
                SAT_FORMA_PAGO = string.IsNullOrWhiteSpace(dto.FormaPago) ? "01" : dto.FormaPago.Trim(),
                Sat_Uso_Cfdi = usoCfdi.Numero,
                ValidadoReceptor = "S"
            };

            bool guardado = await _clienteFiscalRepository.RegistrarRfcAsync(nuevoCliente);
            if (!guardado)
            {
                return new ApiResponse<ClienteFiscalDto>
                {
                    Success = false,
                    Message = "No fue posible guardar el RFC."
                };
            }

            return new ApiResponse<ClienteFiscalDto>
            {
                Success = true,
                Message = "RFC guardado con éxito.",
                Data = MapearClienteDto(nuevoCliente, mailAcceso)
            };
        }

        public Task<ApiResponse<FacturaTicketResponseDto>> VistaPreviaFacturaAsync(GenerarFacturaTicketRequestDto dto)
        {
            return PrepararFacturaTicketAsync(dto, actualizarCliente: false);
        }

        public Task<ApiResponse<FacturaTicketResponseDto>> GenerarFacturaAsync(GenerarFacturaTicketRequestDto dto)
        {
            return PrepararFacturaTicketAsync(dto, actualizarCliente: true);
        }

        private async Task<ApiResponse<FacturaTicketResponseDto>> PrepararFacturaTicketAsync(
            GenerarFacturaTicketRequestDto dto,
            bool actualizarCliente)
        {
            if (dto.Tickets == null || dto.Tickets.Count == 0)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Debe incluir al menos un ticket para facturar."
                };
            }

            string rfc = QuitaDobleEspacio(dto.RFC).ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(rfc) || !EsRfcValido(rfc))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "RFC inválida."
                };
            }

            string monedaClave = string.IsNullOrWhiteSpace(dto.Moneda) ? "MXN" : dto.Moneda.Trim().ToUpperInvariant();
            var moneda = await _catalogosSatRepository.ObtenerMonedaPorNumeroAsync(monedaClave);
            if (moneda == null)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "La moneda seleccionada no existe en el catálogo SAT."
                };
            }

            decimal tipoCambio = monedaClave == "MXN" ? 1.0000m : dto.TipoCambio;
            if (monedaClave != "MXN" && tipoCambio <= 0)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Tipo de cambio inválido."
                };
            }

            var regimen = await _catalogosSatRepository.ObtenerRegimenFiscalPorNumeroAsync(dto.RegimenFiscal);
            if (regimen == null)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Régimen Fiscal inválido."
                };
            }

            var usoCfdi = await _catalogosSatRepository.ObtenerUsoCfdiPorNumeroAsync(dto.UsoCfdi);
            if (usoCfdi == null)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Tipo de gasto (Uso de CFDI) inválido."
                };
            }

            Cliente? cliente = dto.NumeroCliente > 0
                ? await _clienteFiscalRepository.ObtenerPorNumeroClienteYRfcAsync(dto.NumeroCliente, rfc)
                : await _clienteFiscalRepository.ObtenerPorRfcGeneralAsync(rfc);

            if (cliente == null)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "No existe el RFC registrado para el cliente. Realice primero el registro del RFC."
                };
            }

            string razonSocialFinal = !string.IsNullOrWhiteSpace(dto.RazonSocial)
                ? QuitaDobleEspacio(dto.RazonSocial).ToUpperInvariant()
                : QuitaDobleEspacio(cliente.Razon_Social).ToUpperInvariant();
            string cpFinal = !string.IsNullOrWhiteSpace(dto.CodigoPostal)
                ? dto.CodigoPostal.Trim()
                : cliente.Codigo_Postal.Trim();

            if (string.IsNullOrWhiteSpace(razonSocialFinal))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "La razón social del receptor es obligatoria."
                };
            }

            if (string.IsNullOrWhiteSpace(cpFinal))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "El código postal del receptor es obligatorio."
                };
            }

            string mailUsuario = await ResolverMailUsuarioAsync(dto.Mail, cliente.Numero_Cliente);

            var solicitudes = dto.Tickets.Select(t => new ConsultarTicketRequestDto
            {
                Folio = t.Folio,
                Serie = t.Serie,
                Total = t.Total,
                Transaccion = t.Transaccion,
                NumeroCliente = cliente.Numero_Cliente,
                MailUsuario = mailUsuario
            }).ToList();

            var validacion = await ValidarGrupoTicketsAsync(solicitudes, cliente.Numero_Cliente, mailUsuario);
            if (!validacion.Success || validacion.Response == null || validacion.PartidasDetalle.Count == 0)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = validacion.Message
                };
            }

            if (validacion.Response.GranTotal <= 0)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "No es posible facturar con total negativo o igual a 0."
                };
            }

            foreach (var folioDistinto in validacion.PartidasDetalle.Select(p => p.Folio).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                string? yaTimbrado = await _factTicketRepository.VerificarFolioYaTimbradoIndividualAsync(folioDistinto);
                if (!string.IsNullOrWhiteSpace(yaTimbrado))
                {
                    return new ApiResponse<FacturaTicketResponseDto>
                    {
                        Success = false,
                        Message = $"Ya se timbró el Folio {yaTimbrado}."
                    };
                }
            }

            var compania = await _factTicketRepository.ObtenerCompaniaActivaAsync();
            if (compania == null)
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "No se encontró configuración de la compañía emisora."
                };
            }

            if (string.IsNullOrWhiteSpace(compania.CveRegimenFiscal))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Régimen Fiscal de Emisor inválido."
                };
            }

            bool esPrueba = !compania.Prueba.Trim().Equals("N", StringComparison.OrdinalIgnoreCase);
            string? serieSucursal = await _factTicketRepository.ObtenerSerieFacturaSucursalAsync(validacion.Response.Sucursal);
            string serieFactura = esPrueba
                ? "PRUEBA"
                : (!string.IsNullOrWhiteSpace(serieSucursal) ? serieSucursal.Trim() : compania.Serie_Factura.Trim());

            if (string.IsNullOrWhiteSpace(serieFactura))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Falta Serie de Facturación, en Sucursal."
                };
            }

            string formaPagoSat = validacion.Response.FormaPagoSat;
            string formaPagoDesc = validacion.Response.FormaPagoDescripcion;
            if (string.IsNullOrWhiteSpace(formaPagoSat) && !string.IsNullOrWhiteSpace(dto.FormaPago))
            {
                var fpCatalogo = await _catalogosSatRepository.ObtenerFormaPagoPorNumeroAsync(dto.FormaPago);
                if (fpCatalogo != null)
                {
                    formaPagoSat = fpCatalogo.Numero;
                    formaPagoDesc = fpCatalogo.Descripcion;
                }
            }

            if (string.IsNullOrWhiteSpace(formaPagoSat))
            {
                return new ApiResponse<FacturaTicketResponseDto>
                {
                    Success = false,
                    Message = "Clave SAT, de Forma de pago inválida."
                };
            }

            string direccionFinal = dto.Direccion?.Trim() ?? cliente.Direccion;
            string numExtFinal = dto.NumeroExt?.Trim() ?? cliente.Numero_Ext;
            string numIntFinal = dto.NumeroInt?.Trim() ?? cliente.Numero_Int;
            string coloniaFinal = dto.Colonia?.Trim() ?? cliente.Colonia;
            string ciudadFinal = dto.Ciudad?.Trim() ?? cliente.Ciudad;
            string delegacionFinal = dto.Delegacion?.Trim() ?? cliente.Delegacion;
            string estadoFinal = dto.Estado?.Trim() ?? cliente.Estado;

            if (actualizarCliente)
            {
                cliente.Razon_Social = razonSocialFinal;
                cliente.Direccion = direccionFinal;
                cliente.Numero_Ext = numExtFinal;
                cliente.Numero_Int = numIntFinal;
                cliente.Colonia = coloniaFinal;
                cliente.Codigo_Postal = cpFinal;
                cliente.Estado = estadoFinal;
                cliente.Delegacion = delegacionFinal;
                cliente.Ciudad = ciudadFinal;
                cliente.SAT_RegimenFiscal = regimen.Numero;
                cliente.Sat_Uso_Cfdi = usoCfdi.Numero;
                cliente.ValidadoReceptor = "S";
                await _clienteFiscalRepository.ActualizarDatosFiscalesAsync(cliente);
            }

            int folioTentativo = await _factTicketRepository.ObtenerSiguienteFolioPreviewAsync(serieFactura);

            var fac = new Factura40
            {
                Serie = serieFactura,
                Folio = folioTentativo,
                Fecha = DateTime.Now,
                FormaPago = formaPagoSat,
                FormaPagoDesc = formaPagoDesc,
                NoCertificado = compania.Numero_Certificado.Trim(),
                CondicionesDePago = string.Empty,
                Moneda = moneda.Numero,
                MonedaDesc = Regex.Replace(moneda.Descripcion, " {2,}", " "),
                TipoCambio = tipoCambio,
                Total = Math.Round(validacion.Response.GranTotal, 2, MidpointRounding.AwayFromZero),
                TipoDeComprobante = "I",
                MetodoPago = "PUE",
                MetodoPagoDesc = "Pago en una sola exhibición",
                LugarExpedicion = compania.CP.Trim(),
                Exportacion = "01",
                EmisorRFC = compania.RFC.Trim(),
                EmisorNombre = compania.Razon_Social.Trim(),
                EmisorCalle = compania.Direccion.Trim(),
                EmisorColonia = compania.Colonia.Trim(),
                EmisorEstado = compania.Estado.Trim(),
                EmisorCP = compania.CP.Trim(),
                EmisorNumExt = compania.Num_Ext.Trim(),
                EmisorNumInt = compania.Num_Int.Trim(),
                EmisorPais = compania.Pais.Trim(),
                EmisorDelegacion = compania.Delegacion.Trim(),
                UbicaLlave = compania.Ubica_Llave.Trim(),
                UbicaCertificado = compania.Ubica_Certificado.Trim(),
                CertificadoPassword = compania.A_Password,
                EmisorRegimenFiscal = compania.CveRegimenFiscal.Trim(),
                EmisorRegimenFiscalDesc = compania.RegimenFiscalDesc.Trim(),
                RecepRFC = rfc,
                RecepNombre = razonSocialFinal,
                RecepUsoCFDI = usoCfdi.Numero,
                RecepUsoCFDIDesc = usoCfdi.Descripcion,
                RegimenFiscalReceptor = regimen.Numero,
                RegimenFiscalReceptorDesc = regimen.Descripcion,
                RecepCalle = direccionFinal,
                RecepNumExt = numExtFinal,
                RecepNumInt = numIntFinal,
                RecepColonia = coloniaFinal,
                RecepDelegacion = delegacionFinal,
                RecepCiudad = ciudadFinal,
                RecepEstado = estadoFinal,
                RecepCP = cpFinal
            };

            long sec = 0;
            foreach (var p in validacion.PartidasDetalle)
            {
                if (string.IsNullOrWhiteSpace(p.ClaveProdServ_SAT))
                {
                    return new ApiResponse<FacturaTicketResponseDto>
                    {
                        Success = false,
                        Message = $"Falta Cve. Prod. Sat en Sec. {p.Numero}"
                    };
                }

                if (string.IsNullOrWhiteSpace(p.Unidad_Medida_SAT))
                {
                    return new ApiResponse<FacturaTicketResponseDto>
                    {
                        Success = false,
                        Message = $"Falta U.Med. Sat en Sec. {p.Numero}"
                    };
                }

                sec++;
                decimal importe = p.Cantidad * p.PrecioSinIva;
                decimal tasaOCuota = p.Porcentaje_Iva > 0 ? p.Porcentaje_Iva * 0.01m : 0m;
                bool esExento = p.Exento.Trim().Equals("S", StringComparison.OrdinalIgnoreCase);

                fac.Conceptos.Add(new ConceptoFactura40
                {
                    Secuencia = sec,
                    ClaveProdServ = p.ClaveProdServ_SAT.Trim(),
                    NoIdentificacion = p.Producto.ToString(CultureInfo.InvariantCulture),
                    Cantidad = p.Cantidad,
                    ClaveUnidad = p.Unidad_Medida_SAT.Trim(),
                    Unidad = p.Unidad_Medida.Trim(),
                    Descripcion = p.Descripcion.Trim(),
                    ValorUnitario = p.PrecioSinIva,
                    Importe = importe,
                    TasaOCuota = tasaOCuota,
                    ImpuestoIva = p.ImpIva,
                    Descuento = 0m,
                    ObjetoImp = esExento ? "01" : "02"
                });

                if (!esExento)
                {
                    decimal baseImp = Math.Round(importe, 2, MidpointRounding.AwayFromZero);
                    fac.Traslados.Add(new ImpuestoConcepto40
                    {
                        Secuencia = sec,
                        NoIdentificacion = p.Producto.ToString(CultureInfo.InvariantCulture),
                        Base = baseImp,
                        Impuesto = "002",
                        TipoFactor = "Tasa",
                        TasaOCuota = tasaOCuota,
                        Importe = tasaOCuota * importe
                    });
                }
            }

            fac.CreaCadenaOriginal();
            fac.M_Letras = Factura40.NumALetras(fac.Total, fac.MonedaDesc, fac.Moneda);
            string xmlSinTimbrar = fac.CreaXml();

            NotaCreditoPreviaDto? notaCreditoPrevia = null;
            var partidasGlobales = validacion.PartidasDetalle
                .Where(p => !string.IsNullOrWhiteSpace(p.Uuid) && p.EsGlobal.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (partidasGlobales.Count > 0)
            {
                notaCreditoPrevia = await ConstruirNotaCreditoPreviaAsync(
                    compania,
                    partidasGlobales,
                    formaPagoSat,
                    formaPagoDesc,
                    moneda,
                    tipoCambio);
            }

            var responseDto = new FacturaTicketResponseDto
            {
                Timbrado = false,
                EstadoTimbrado = actualizarCliente ? "PENDIENTE_TIMBRADOR" : "VISTA_PREVIA",
                FirmaOmitida = true,
                EsPrueba = esPrueba,
                Serie = fac.Serie,
                FolioTentativo = fac.Folio,
                FechaEmision = fac.Fecha.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                FormaPago = fac.FormaPago,
                FormaPagoDescripcion = fac.FormaPagoDesc,
                MetodoPago = fac.MetodoPago,
                Moneda = fac.Moneda,
                TipoCambio = fac.TipoCambio,
                TipoDeComprobante = fac.TipoDeComprobante,
                Exportacion = fac.Exportacion,
                LugarExpedicion = fac.LugarExpedicion,
                Subtotal = fac.SubTotal,
                Descuento = fac.Descuento,
                Iva = Math.Round(fac.TotalImpuestosTrasladados, 2, MidpointRounding.AwayFromZero),
                Total = fac.Total,
                ImporteConLetra = fac.M_Letras,
                Emisor = new EmisorCfdiDto
                {
                    Rfc = fac.EmisorRFC,
                    Nombre = fac.EmisorNombre,
                    RegimenFiscal = fac.EmisorRegimenFiscal,
                    RegimenFiscalDesc = fac.EmisorRegimenFiscalDesc,
                    LugarExpedicion = fac.LugarExpedicion
                },
                Receptor = new ReceptorCfdiDto
                {
                    NumeroCliente = cliente.Numero_Cliente,
                    Rfc = fac.RecepRFC,
                    Nombre = fac.RecepNombre,
                    DomicilioFiscalReceptor = fac.RecepCP,
                    RegimenFiscalReceptor = fac.RegimenFiscalReceptor,
                    RegimenFiscalReceptorDesc = fac.RegimenFiscalReceptorDesc,
                    UsoCfdi = fac.RecepUsoCFDI,
                    UsoCfdiDesc = fac.RecepUsoCFDIDesc,
                    Mail = mailUsuario
                },
                Conceptos = fac.Conceptos.Select(c => new ConceptoCfdiDto
                {
                    Secuencia = c.Secuencia,
                    ClaveProdServ = c.ClaveProdServ,
                    NoIdentificacion = c.NoIdentificacion,
                    Cantidad = c.Cantidad,
                    ClaveUnidad = c.ClaveUnidad,
                    Unidad = c.Unidad,
                    Descripcion = c.Descripcion,
                    ValorUnitario = c.ValorUnitario,
                    Importe = c.Importe,
                    Descuento = c.Descuento,
                    ObjetoImp = c.ObjetoImp
                }).ToList(),
                Traslados = fac.Traslados.Select(t => new TrasladoCfdiDto
                {
                    Secuencia = t.Secuencia,
                    NoIdentificacion = t.NoIdentificacion,
                    Base = t.Base,
                    Impuesto = t.Impuesto,
                    TipoFactor = t.TipoFactor,
                    TasaOCuota = t.TasaOCuota,
                    Importe = t.Importe
                }).ToList(),
                CadenaOriginal = fac.CadenaOriginal,
                XmlSinTimbrar = xmlSinTimbrar,
                NotaCreditoPrevia = notaCreditoPrevia
            };

            string msg = actualizarCliente
                ? "Estructura CFDI 4.0 generada y datos fiscales actualizados. Pendiente firma y timbrado hasta contar con datos del timbrador."
                : "Vista previa CFDI 4.0 generada correctamente sin firma ni timbrado.";

            return new ApiResponse<FacturaTicketResponseDto>
            {
                Success = true,
                Message = msg,
                Data = responseDto
            };
        }

        private async Task<NotaCreditoPreviaDto> ConstruirNotaCreditoPreviaAsync(
            Compania compania,
            List<OrdenTicketDetalle> partidasGlobales,
            string formaPagoSat,
            string formaPagoDesc,
            MonedaCatalogoDto moneda,
            decimal tipoCambio)
        {
            string serieNc = compania.Prueba.Trim().Equals("S", StringComparison.OrdinalIgnoreCase)
                ? "PRUEBANC"
                : (!string.IsNullOrWhiteSpace(compania.Serie_NotaCredito) ? compania.Serie_NotaCredito.Trim() : "NC");

            int folioNc = await _factTicketRepository.ObtenerSiguienteFolioPreviewAsync(serieNc);
            var uuids = partidasGlobales.Select(p => p.Uuid.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var nc = new Factura40
            {
                Serie = serieNc,
                Folio = folioNc,
                Fecha = DateTime.Now,
                FormaPago = formaPagoSat,
                FormaPagoDesc = formaPagoDesc,
                NoCertificado = compania.Numero_Certificado.Trim(),
                Moneda = moneda.Numero,
                MonedaDesc = Regex.Replace(moneda.Descripcion, " {2,}", " "),
                TipoCambio = tipoCambio,
                Total = Math.Round(partidasGlobales.Sum(p => p.TotalFacturar), 2, MidpointRounding.AwayFromZero),
                TipoDeComprobante = "E",
                MetodoPago = "PUE",
                MetodoPagoDesc = "Pago en una sola exhibición",
                LugarExpedicion = compania.CP.Trim(),
                Exportacion = "01",
                TipoRelacion = "01",
                TipoRelacionDesc = "Nota de crédito de los documentos relacionados",
                UuidRelacionados = string.Join(",", uuids),
                EmisorRFC = compania.RFC.Trim(),
                EmisorNombre = compania.Razon_Social.Trim(),
                EmisorRegimenFiscal = compania.CveRegimenFiscal.Trim(),
                EmisorRegimenFiscalDesc = compania.RegimenFiscalDesc.Trim(),
                RecepRFC = "XAXX010101000",
                RecepNombre = "Publico en General",
                RecepUsoCFDI = "G02",
                RecepUsoCFDIDesc = "Devoluciones, descuentos o bonificaciones.",
                RecepCP = compania.CP.Trim(),
                RegimenFiscalReceptor = "616",
                RegimenFiscalReceptorDesc = "Sin obligaciones fiscales"
            };

            long sec = 0;
            foreach (var p in partidasGlobales)
            {
                sec++;
                decimal valorUnitario = p.Cantidad > 0
                    ? Math.Round(p.Subtotal / p.Cantidad, 2, MidpointRounding.AwayFromZero)
                    : 0m;
                decimal importe = p.Cantidad * valorUnitario;
                decimal tasaOCuota = p.Porcentaje_Iva > 0 ? p.Porcentaje_Iva * 0.01m : 0m;
                bool esExento = p.Exento.Trim().Equals("S", StringComparison.OrdinalIgnoreCase);

                nc.Conceptos.Add(new ConceptoFactura40
                {
                    Secuencia = sec,
                    ClaveProdServ = p.ClaveProdServ_SAT.Trim(),
                    NoIdentificacion = p.Producto.ToString(CultureInfo.InvariantCulture),
                    Cantidad = p.Cantidad,
                    ClaveUnidad = p.Unidad_Medida_SAT.Trim(),
                    Unidad = p.Unidad_Medida.Trim(),
                    Descripcion = p.Descripcion.Trim(),
                    ValorUnitario = valorUnitario,
                    Importe = importe,
                    ImpuestoIva = p.ImpIva,
                    TasaOCuota = tasaOCuota,
                    Descuento = 0m,
                    ObjetoImp = esExento ? "01" : "02"
                });

                if (!esExento)
                {
                    nc.Traslados.Add(new ImpuestoConcepto40
                    {
                        Secuencia = sec,
                        NoIdentificacion = p.Producto.ToString(CultureInfo.InvariantCulture),
                        Base = Math.Round(importe, 2, MidpointRounding.AwayFromZero),
                        Impuesto = "002",
                        TipoFactor = "Tasa",
                        TasaOCuota = tasaOCuota,
                        Importe = Math.Round(tasaOCuota * importe, 4, MidpointRounding.AwayFromZero)
                    });
                }
            }

            nc.CreaCadenaOriginal();
            string xmlNc = nc.CreaXml();

            return new NotaCreditoPreviaDto
            {
                Serie = nc.Serie,
                FolioTentativo = nc.Folio,
                TipoDeComprobante = nc.TipoDeComprobante,
                TipoRelacion = nc.TipoRelacion,
                UuidsRelacionados = nc.UuidRelacionados,
                Subtotal = nc.SubTotal,
                Total = nc.Total,
                CadenaOriginal = nc.CadenaOriginal,
                XmlSinTimbrar = xmlNc
            };
        }

        private async Task<ApiResponse<TicketConsultaResponseDto>> ObtenerYValidarTicketIndividualAsync(
            ConsultarTicketRequestDto dto,
            string mailUsuario)
        {
            string folio = (dto.Folio ?? string.Empty).Trim().ToUpperInvariant();
            string serie = (dto.Serie ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(folio))
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "Capture un Folio."
                };
            }

            if (dto.Total.HasValue && dto.Total.Value < 0)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "Capture un Total válido mayor a 0."
                };
            }

            var foliosCandidatos = new List<string> { folio };
            if (!string.IsNullOrWhiteSpace(serie) && !folio.StartsWith(serie, StringComparison.OrdinalIgnoreCase))
            {
                foliosCandidatos.Add($"{serie}{folio}");
                if (long.TryParse(folio, out _))
                {
                    foliosCandidatos.Add($"{serie}{folio.PadLeft(7, '0')}");
                }
            }

            var todasPartidas = await _factTicketRepository.ObtenerPartidasPorFoliosCandidatosAsync(
                foliosCandidatos.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                dto.Transaccion);

            if (todasPartidas.Count == 0 && long.TryParse(folio, out long ordenNumero))
            {
                todasPartidas = await _factTicketRepository.ObtenerPartidasPorOrdenAsync(ordenNumero, dto.Transaccion);
            }

            if (todasPartidas.Count == 0)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "No se encuentra el ticket."
                };
            }

            List<OrdenTicketDetalle> partidasSerie = todasPartidas;
            if (!string.IsNullOrWhiteSpace(serie))
            {
                partidasSerie = todasPartidas.Where(p =>
                    p.SerieFactura.Trim().Equals(serie, StringComparison.OrdinalIgnoreCase) ||
                    p.Folio.Trim().StartsWith(serie, StringComparison.OrdinalIgnoreCase)).ToList();

                if (partidasSerie.Count == 0)
                {
                    return new ApiResponse<TicketConsultaResponseDto>
                    {
                        Success = false,
                        Message = "La serie capturada no coincide con el ticket."
                    };
                }
            }

            var gruposTicket = partidasSerie
                .GroupBy(p => new { p.Sucursal, p.Fecha, p.Orden, p.Folio })
                .ToList();

            var grupoSeleccionado = gruposTicket.Count == 1
                ? gruposTicket[0].ToList()
                : (dto.Total.HasValue && dto.Total.Value > 0
                    ? (gruposTicket.FirstOrDefault(g => Math.Abs(g.Sum(x => x.Total) - dto.Total.Value) <= 0.05m) ?? gruposTicket[0])
                    : gruposTicket[0]).ToList();

            if (grupoSeleccionado.All(p => p.Baja.Trim() == "*"))
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "El ticket fue eliminado."
                };
            }

            var activas = grupoSeleccionado.Where(p => p.Baja.Trim() != "*").ToList();
            if (activas.Count == 0)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "El ticket fue eliminado."
                };
            }

            var first = activas[0];
            if (!string.IsNullOrWhiteSpace(first.Uuid) && string.IsNullOrWhiteSpace(first.EsGlobal))
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "El Ticket ya ha sido Facturado."
                };
            }

            if (!EsCorreoExentoMesCurso(mailUsuario))
            {
                string mesActual = DateTime.Now.ToString("yyyy/MM", CultureInfo.InvariantCulture);
                if (!first.Fecha.Contains(mesActual, StringComparison.OrdinalIgnoreCase))
                {
                    return new ApiResponse<TicketConsultaResponseDto>
                    {
                        Success = false,
                        Message = "Solo se facturan tickets, del mes en curso."
                    };
                }
            }

            var facturables = activas
                .Where(p => !p.Tipo.Trim().Equals("Servicio", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (facturables.Count == 0)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = "No se encuentra el ticket."
                };
            }

            decimal totalTicket = Math.Round(facturables.Sum(p => p.Total), 2, MidpointRounding.AwayFromZero);
            if (dto.Total.HasValue && dto.Total.Value > 0 && Math.Abs(dto.Total.Value - totalTicket) > 0.05m)
            {
                return new ApiResponse<TicketConsultaResponseDto>
                {
                    Success = false,
                    Message = $"El total capturado ({dto.Total.Value:0.00}) no coincide con el total del ticket ({totalTicket:0.00})."
                };
            }

            foreach (var p in facturables)
            {
                if (string.IsNullOrWhiteSpace(p.ClaveProdServDesc))
                {
                    return new ApiResponse<TicketConsultaResponseDto>
                    {
                        Success = false,
                        Message = $"Clave de Sat incorrecta, en Producto: {p.Producto} - {p.Descripcion} , comuniquese con personal de Soporte PITS al Tel. 55 5523 9816 Ext 101"
                    };
                }
            }

            var formaPagoInfo = await _factTicketRepository.ObtenerFormaPagoOrdenAsync(first.Sucursal, first.Orden, first.Fecha);
            string formaPagoSat = formaPagoInfo?.Forma_Pago_SAT.Trim() ?? string.Empty;
            string formaPagoDesc = formaPagoInfo?.FormaPago_Desc.Trim() ?? string.Empty;
            if (formaPagoInfo != null &&
                formaPagoInfo.Forma_Pago_Descripcion.Trim().Equals("EFECTIVO", StringComparison.OrdinalIgnoreCase))
            {
                formaPagoSat = "01";
                formaPagoDesc = "Efectivo";
            }

            decimal subtotalTicket = Math.Round(facturables.Sum(p => p.Subtotal), 2, MidpointRounding.AwayFromZero);
            decimal ivaTicket = Math.Round(facturables.Sum(p => p.ImpIva), 2, MidpointRounding.AwayFromZero);
            decimal descuentoTicket = Math.Round(facturables.Sum(p => p.Descuento), 2, MidpointRounding.AwayFromZero);

            bool requiereNc = facturables.Any(p =>
                !string.IsNullOrWhiteSpace(p.Uuid) &&
                p.EsGlobal.Trim().Equals("S", StringComparison.OrdinalIgnoreCase));

            var response = new TicketConsultaResponseDto
            {
                Folio = first.Folio.Trim(),
                Serie = string.IsNullOrWhiteSpace(serie) ? first.SerieFactura.Trim() : serie,
                SerieFacturaSucursal = first.SerieFactura.Trim(),
                Transaccion = first.Transaccion,
                Sucursal = first.Sucursal,
                SucursalNombre = first.SucursalNombre.Trim(),
                Fecha = first.Fecha.Trim(),
                FormaPagoSat = formaPagoSat,
                FormaPagoDescripcion = formaPagoDesc,
                RequiereNotaCredito = requiereNc,
                UuidGlobal = first.UuidGlobal.Trim(),
                Subtotal = subtotalTicket,
                Iva = ivaTicket,
                Descuento = descuentoTicket,
                Total = totalTicket,
                Partidas = facturables.Select(MapearPartidaDto).ToList()
            };

            return new ApiResponse<TicketConsultaResponseDto>
            {
                Success = true,
                Message = "Ticket encontrado y validado correctamente.",
                Data = response
            };
        }

        private async Task<(bool Success, string Message, ValidarTicketsResponseDto? Response, List<OrdenTicketDetalle> PartidasDetalle)> ValidarGrupoTicketsAsync(
            List<ConsultarTicketRequestDto> solicitudes,
            int? numeroCliente,
            string mailUsuario)
        {
            var foliosVistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var todasDetalle = new List<OrdenTicketDetalle>();
            int sucursalBase = 0;
            string sucursalNombre = string.Empty;
            string serieFactura = string.Empty;
            string formaPagoSat = string.Empty;
            string formaPagoDesc = string.Empty;
            bool requiereNc = false;

            foreach (var sol in solicitudes)
            {
                var consulta = await ObtenerYValidarTicketIndividualAsync(new ConsultarTicketRequestDto
                {
                    Folio = sol.Folio,
                    Serie = sol.Serie,
                    Total = sol.Total,
                    Transaccion = sol.Transaccion,
                    NumeroCliente = numeroCliente ?? sol.NumeroCliente,
                    MailUsuario = mailUsuario
                }, mailUsuario);

                if (!consulta.Success || consulta.Data == null)
                {
                    return (false, consulta.Message, null, new List<OrdenTicketDetalle>());
                }

                if (!foliosVistos.Add(consulta.Data.Folio))
                {
                    return (false, $"Ya se añadió el Ticket: {consulta.Data.Folio}", null, new List<OrdenTicketDetalle>());
                }

                if (sucursalBase == 0)
                {
                    sucursalBase = consulta.Data.Sucursal;
                    sucursalNombre = consulta.Data.SucursalNombre;
                    serieFactura = consulta.Data.SerieFacturaSucursal;
                    formaPagoSat = consulta.Data.FormaPagoSat;
                    formaPagoDesc = consulta.Data.FormaPagoDescripcion;
                }
                else if (consulta.Data.Sucursal != sucursalBase)
                {
                    return (false, "Los tickets, deben pertenecer a la misma Sucursal.", null, new List<OrdenTicketDetalle>());
                }

                if (consulta.Data.RequiereNotaCredito)
                {
                    requiereNc = true;
                }

                var detalle = await _factTicketRepository.ObtenerPartidasPorFoliosCandidatosAsync(
                    new List<string> { consulta.Data.Folio },
                    sol.Transaccion);

                var facturablesDetalle = detalle.Where(p =>
                    p.Sucursal == consulta.Data.Sucursal &&
                    p.Fecha == consulta.Data.Fecha &&
                    p.Baja.Trim() != "*" &&
                    !p.Tipo.Trim().Equals("Servicio", StringComparison.OrdinalIgnoreCase)).ToList();

                todasDetalle.AddRange(facturablesDetalle);
            }

            var response = new ValidarTicketsResponseDto
            {
                Sucursal = sucursalBase,
                SucursalNombre = sucursalNombre,
                SerieFactura = serieFactura,
                FormaPagoSat = formaPagoSat,
                FormaPagoDescripcion = formaPagoDesc,
                RequiereNotaCredito = requiereNc,
                SubtotalTotal = Math.Round(todasDetalle.Sum(p => p.Subtotal), 2, MidpointRounding.AwayFromZero),
                IvaTotal = Math.Round(todasDetalle.Sum(p => p.ImpIva), 2, MidpointRounding.AwayFromZero),
                DescuentoTotal = Math.Round(todasDetalle.Sum(p => p.Descuento), 2, MidpointRounding.AwayFromZero),
                GranTotal = Math.Round(todasDetalle.Sum(p => p.Total), 2, MidpointRounding.AwayFromZero),
                Partidas = todasDetalle.Select(MapearPartidaDto).ToList()
            };

            return (true, "Validación exitosa.", response, todasDetalle);
        }

        private async Task<string> ResolverMailUsuarioAsync(string? mailUsuario, int? numeroCliente)
        {
            if (!string.IsNullOrWhiteSpace(mailUsuario))
            {
                return mailUsuario.Trim();
            }

            if (numeroCliente.HasValue && numeroCliente.Value > 0)
            {
                return (await _clienteFiscalRepository.ObtenerMailAccesoAsync(numeroCliente.Value) ?? string.Empty).Trim();
            }

            return string.Empty;
        }

        private static bool EsCorreoExentoMesCurso(string mail)
        {
            return mail.Equals("BFF@INTER-OP.COM.MX", StringComparison.OrdinalIgnoreCase) ||
                   mail.Equals("prueba381@gmail.com", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsRfcValido(string rfc)
        {
            return Regex.IsMatch(rfc, @"^([A-ZÑ&]{3,4})(\d{6})([A-Z\d]{3})$");
        }

        private static string QuitaDobleEspacio(string? valor)
        {
            return Regex.Replace((valor ?? string.Empty).Trim(), " {2,}", " ");
        }

        private static ClienteFiscalDto MapearClienteDto(Cliente cliente, string mailAcceso)
        {
            return new ClienteFiscalDto
            {
                NumeroCliente = cliente.Numero_Cliente,
                RazonSocial = cliente.Razon_Social.Trim(),
                RFC = cliente.RFC.Trim(),
                Direccion = cliente.Direccion.Trim(),
                NumeroExt = cliente.Numero_Ext.Trim(),
                NumeroInt = cliente.Numero_Int.Trim(),
                Colonia = cliente.Colonia.Trim(),
                Ciudad = cliente.Ciudad.Trim(),
                Delegacion = cliente.Delegacion.Trim(),
                Estado = cliente.Estado.Trim(),
                CodigoPostal = cliente.Codigo_Postal.Trim(),
                Telefono = cliente.Telefono.Trim(),
                Mail = string.IsNullOrWhiteSpace(mailAcceso) ? cliente.Mail.Trim() : mailAcceso.Trim(),
                RegimenFiscal = cliente.SAT_RegimenFiscal.Trim(),
                FormaPago = cliente.SAT_FORMA_PAGO.Trim(),
                UsoCfdi = cliente.Sat_Uso_Cfdi.Trim(),
                ValidadoReceptor = cliente.ValidadoReceptor.Trim()
            };
        }

        private static TicketPartidaDto MapearPartidaDto(OrdenTicketDetalle p)
        {
            return new TicketPartidaDto
            {
                Numero = p.Numero,
                Orden = p.Orden,
                Folio = p.Folio.Trim(),
                Transaccion = p.Transaccion,
                Sucursal = p.Sucursal,
                SucursalNombre = p.SucursalNombre.Trim(),
                SerieFactura = p.SerieFactura.Trim(),
                Fecha = p.Fecha.Trim(),
                Turno = p.Turno,
                Tipo = p.Tipo.Trim(),
                Producto = p.Producto,
                Descripcion = p.Descripcion.Trim(),
                ClaveProdServSat = p.ClaveProdServ_SAT.Trim(),
                ClaveProdServDesc = p.ClaveProdServDesc.Trim(),
                UnidadMedidaSat = p.Unidad_Medida_SAT.Trim(),
                UnidadMedida = p.Unidad_Medida.Trim(),
                Exento = p.Exento.Trim(),
                Cantidad = p.Cantidad,
                Precio = p.Precio,
                PorcentajeIva = p.Porcentaje_Iva,
                PrecioSinIva = p.PrecioSinIva,
                Subtotal = p.Subtotal,
                Iva = p.ImpIva,
                Descuento = p.Descuento,
                Total = p.Total,
                TotalFacturar = p.TotalFacturar,
                BaseIva16 = p.Base_Iva16,
                BaseIva0 = p.Base_Iva0,
                UuidGlobal = p.UuidGlobal.Trim(),
                EsGlobal = p.EsGlobal.Trim()
            };
        }
    }
}
