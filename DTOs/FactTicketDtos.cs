using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.DTOs
{
    public class ConsultarTicketRequestDto
    {
        [Required(ErrorMessage = "El folio del ticket es obligatorio.")]
        public string Folio { get; set; } = string.Empty;

        public string Serie { get; set; } = string.Empty;

        public decimal? Total { get; set; }

        public int? Transaccion { get; set; }
        public int? NumeroCliente { get; set; }
        public string? MailUsuario { get; set; }
    }

    public class ValidarTicketsRequestDto
    {
        [Required(ErrorMessage = "Debe enviar al menos un ticket.")]
        public List<ConsultarTicketRequestDto> Tickets { get; set; } = new();

        public int? NumeroCliente { get; set; }
        public string? MailUsuario { get; set; }
    }

    public class TicketPartidaDto
    {
        public int Numero { get; set; }
        public int Orden { get; set; }
        public string Folio { get; set; } = string.Empty;
        public int Transaccion { get; set; }
        public int Sucursal { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string SerieFactura { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public int Turno { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Producto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ClaveProdServSat { get; set; } = string.Empty;
        public string ClaveProdServDesc { get; set; } = string.Empty;
        public string UnidadMedidaSat { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public string Exento { get; set; } = "N";
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal PrecioSinIva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public decimal TotalFacturar { get; set; }
        public decimal BaseIva16 { get; set; }
        public decimal BaseIva0 { get; set; }
        public string UuidGlobal { get; set; } = string.Empty;
        public string EsGlobal { get; set; } = string.Empty;
    }

    public class TicketConsultaResponseDto
    {
        public string Folio { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string SerieFacturaSucursal { get; set; } = string.Empty;
        public int Transaccion { get; set; }
        public int Sucursal { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string FormaPagoSat { get; set; } = string.Empty;
        public string FormaPagoDescripcion { get; set; } = string.Empty;
        public bool RequiereNotaCredito { get; set; }
        public string UuidGlobal { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public List<TicketPartidaDto> Partidas { get; set; } = new();
    }

    public class ValidarTicketsResponseDto
    {
        public int Sucursal { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string SerieFactura { get; set; } = string.Empty;
        public string FormaPagoSat { get; set; } = string.Empty;
        public string FormaPagoDescripcion { get; set; } = string.Empty;
        public bool RequiereNotaCredito { get; set; }
        public decimal SubtotalTotal { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal GranTotal { get; set; }
        public List<TicketPartidaDto> Partidas { get; set; } = new();
    }

    public class ClienteRfcItemDto
    {
        public int NumeroCliente { get; set; }
        public string RFC { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
    }

    public class ClienteFiscalDto
    {
        public int NumeroCliente { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string NumeroExt { get; set; } = string.Empty;
        public string NumeroInt { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string UsoCfdi { get; set; } = string.Empty;
        public string ValidadoReceptor { get; set; } = string.Empty;
    }

    public class BuscarRfcResponseDto
    {
        public bool Existe { get; set; }
        public bool RequiereRegistro { get; set; }
        public string RFC { get; set; } = string.Empty;
        public ClienteFiscalDto? Cliente { get; set; }
    }

    public class RegistrarRfcRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El número de cliente es inválido.")]
        public int NumeroCliente { get; set; }

        [Required(ErrorMessage = "El RFC es obligatorio.")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener entre 12 y 13 caracteres.")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(300, ErrorMessage = "La razón social no puede exceder los 300 caracteres.")]
        public string RazonSocial { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La dirección no puede exceder los 100 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número exterior no puede exceder los 20 caracteres.")]
        public string NumeroExt { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número interior no puede exceder los 20 caracteres.")]
        public string NumeroInt { get; set; } = string.Empty;

        [StringLength(60, ErrorMessage = "La colonia no puede exceder los 60 caracteres.")]
        public string Colonia { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La delegación no puede exceder los 100 caracteres.")]
        public string Delegacion { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La ciudad no puede exceder los 50 caracteres.")]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El estado no puede exceder los 100 caracteres.")]
        public string Estado { get; set; } = "Coahuila";

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        [StringLength(6, MinimumLength = 5, ErrorMessage = "El código postal debe tener 5 dígitos.")]
        public string CodigoPostal { get; set; } = string.Empty;

        [Required(ErrorMessage = "El régimen fiscal es obligatorio.")]
        [StringLength(3, ErrorMessage = "El régimen fiscal debe tener 3 dígitos.")]
        public string RegimenFiscal { get; set; } = "612";

        [StringLength(3, ErrorMessage = "La forma de pago debe tener 2 o 3 dígitos.")]
        public string FormaPago { get; set; } = "01";

        [Required(ErrorMessage = "El uso de CFDI es obligatorio.")]
        [StringLength(4, ErrorMessage = "El uso de CFDI no es válido.")]
        public string UsoCfdi { get; set; } = "G03";

        [StringLength(50, ErrorMessage = "El teléfono no puede exceder los 50 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(80, ErrorMessage = "El correo no puede exceder los 80 caracteres.")]
        public string Mail { get; set; } = string.Empty;
    }

    public class TicketSeleccionadoDto
    {
        [Required(ErrorMessage = "El folio del ticket es obligatorio.")]
        public string Folio { get; set; } = string.Empty;

        public string Serie { get; set; } = string.Empty;

        public decimal? Total { get; set; }

        public int? Transaccion { get; set; }
    }

    public class GenerarFacturaTicketRequestDto
    {
        [Range(0, int.MaxValue)]
        public int NumeroCliente { get; set; }

        [Required(ErrorMessage = "El RFC del receptor es obligatorio.")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener entre 12 y 13 caracteres.")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El régimen fiscal es obligatorio.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El régimen fiscal debe tener 3 dígitos.")]
        public string RegimenFiscal { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de gasto (Uso de CFDI) es obligatorio.")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "El uso de CFDI debe tener 3 o 4 caracteres.")]
        public string UsoCfdi { get; set; } = string.Empty;

        public string? FormaPago { get; set; }
        public string Moneda { get; set; } = "MXN";
        public decimal TipoCambio { get; set; } = 1.0000m;
        public string? Mail { get; set; }

        public string? RazonSocial { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Direccion { get; set; }
        public string? NumeroExt { get; set; }
        public string? NumeroInt { get; set; }
        public string? Colonia { get; set; }
        public string? Ciudad { get; set; }
        public string? Delegacion { get; set; }
        public string? Estado { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un ticket.")]
        public List<TicketSeleccionadoDto> Tickets { get; set; } = new();
    }

    public class EmisorCfdiDto
    {
        public string Rfc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public string RegimenFiscalDesc { get; set; } = string.Empty;
        public string LugarExpedicion { get; set; } = string.Empty;
    }

    public class ReceptorCfdiDto
    {
        public int NumeroCliente { get; set; }
        public string Rfc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string DomicilioFiscalReceptor { get; set; } = string.Empty;
        public string RegimenFiscalReceptor { get; set; } = string.Empty;
        public string RegimenFiscalReceptorDesc { get; set; } = string.Empty;
        public string UsoCfdi { get; set; } = string.Empty;
        public string UsoCfdiDesc { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
    }

    public class ConceptoCfdiDto
    {
        public long Secuencia { get; set; }
        public string ClaveProdServ { get; set; } = string.Empty;
        public string NoIdentificacion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string ClaveUnidad { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal Descuento { get; set; }
        public string ObjetoImp { get; set; } = "02";
    }

    public class TrasladoCfdiDto
    {
        public long Secuencia { get; set; }
        public string NoIdentificacion { get; set; } = string.Empty;
        public decimal Base { get; set; }
        public string Impuesto { get; set; } = string.Empty;
        public string TipoFactor { get; set; } = string.Empty;
        public decimal TasaOCuota { get; set; }
        public decimal Importe { get; set; }
    }

    public class NotaCreditoPreviaDto
    {
        public string IdCfdi { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int FolioTentativo { get; set; }
        public string TipoDeComprobante { get; set; } = "E";
        public string TipoRelacion { get; set; } = "01";
        public string UuidsRelacionados { get; set; } = string.Empty;
        public string NoCertificado { get; set; } = string.Empty;
        public string Certificado { get; set; } = string.Empty;
        public string Sello { get; set; } = string.Empty;
        public bool FirmaOmitida { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string CadenaOriginal { get; set; } = string.Empty;
        public string XmlSinTimbrar { get; set; } = string.Empty;
    }

    public class FacturaTicketResponseDto
    {
        public bool Timbrado { get; set; }
        public string EstadoTimbrado { get; set; } = "PENDIENTE_TIMBRADOR";
        public bool FirmaOmitida { get; set; }
        public bool UsandoOverrideLocal { get; set; }
        public bool EsPrueba { get; set; }
        public string IdCfdi { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int FolioTentativo { get; set; }
        public string FechaEmision { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string FormaPagoDescripcion { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = "PUE";
        public string Moneda { get; set; } = "MXN";
        public decimal TipoCambio { get; set; } = 1m;
        public string TipoDeComprobante { get; set; } = "I";
        public string Exportacion { get; set; } = "01";
        public string LugarExpedicion { get; set; } = string.Empty;
        public string NoCertificado { get; set; } = string.Empty;
        public string Certificado { get; set; } = string.Empty;
        public string Sello { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string ImporteConLetra { get; set; } = string.Empty;
        public EmisorCfdiDto Emisor { get; set; } = new();
        public ReceptorCfdiDto Receptor { get; set; } = new();
        public List<ConceptoCfdiDto> Conceptos { get; set; } = new();
        public List<TrasladoCfdiDto> Traslados { get; set; } = new();
        public string CadenaOriginal { get; set; } = string.Empty;
        public string XmlSinTimbrar { get; set; } = string.Empty;
        public NotaCreditoPreviaDto? NotaCreditoPrevia { get; set; }
    }

    public class FirmarCadenaRequestDto
    {
        [Required(ErrorMessage = "La cadena original es obligatoria.")]
        public string CadenaOriginal { get; set; } = string.Empty;
    }

    public class FirmarCadenaResponseDto
    {
        public string NoCertificado { get; set; } = string.Empty;
        public string Certificado { get; set; } = string.Empty;
        public string Sello { get; set; } = string.Empty;
        public bool FirmaValida { get; set; }
        public string CadenaOriginal { get; set; } = string.Empty;
        public string RutaLlave { get; set; } = string.Empty;
        public string RutaCertificado { get; set; } = string.Empty;
        public bool UsandoOverrideLocal { get; set; }
    }
}

