using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.DTOs
{
    public class ConsultaTicketRequestDto
    {
        [Required(ErrorMessage = "El folio del ticket es obligatorio.")]
        public string Folio { get; set; } = string.Empty;

        [Required(ErrorMessage = "La referencia/transacción es obligatoria.")]
        public long Transaccion { get; set; }
    }

    public class TicketPartidaDto
    {
        public int Numero { get; set; }
        public string Folio { get; set; } = string.Empty;
        public long Transaccion { get; set; }
        public int Sucursal { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public int Producto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ClaveProdServSat { get; set; } = string.Empty;
        public string ClaveProdServDesc { get; set; } = string.Empty;
        public string UnidadMedidaSat { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal Cantidad { get; set; } = 1;
        public decimal Precio { get; set; }
        public decimal PorcentajeIva { get; set; }
        public decimal PrecioSinIva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }

    public class ResumenTicketDto
    {
        public string Folio { get; set; } = string.Empty;
        public long Transaccion { get; set; }
        public int Sucursal { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public List<TicketPartidaDto> Partidas { get; set; } = new();
    }

    public class ValidarTicketsRequestDto
    {
        [Required]
        public List<ConsultaTicketRequestDto> Tickets { get; set; } = new();
    }

    public class ValidacionTicketsResponseDto
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int Sucursal { get; set; }
        public decimal SubtotalTotal { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal GranTotal { get; set; }
        public List<TicketPartidaDto> PartidasAgrupadas { get; set; } = new();
    }

    public class FacturarTicketsRequestDto
    {
        [Required]
        public int NumeroCliente { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 12)]
        public string RFC { get; set; } = string.Empty;

        [Required]
        public string RazonSocial { get; set; } = string.Empty;

        public string UsoCfdi { get; set; } = "G03";
        public string RegimenFiscal { get; set; } = "612";
        public string FormaPago { get; set; } = "01";
        public string MetodoPago { get; set; } = "PUE";
        public string Serie { get; set; } = "A";

        [Required]
        public List<ConsultaTicketRequestDto> Tickets { get; set; } = new();
    }

    public class FacturaGeneradaDto
    {
        public string IdCFDI { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int Folio { get; set; }
        public string Uuid { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string FechaTimbrado { get; set; } = string.Empty;
        public int Sucursal { get; set; }
        public string RfcReceptor { get; set; } = string.Empty;
        public string RazonSocialReceptor { get; set; } = string.Empty;
    }
}
