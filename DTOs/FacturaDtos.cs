using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.DTOs
{
    public class FacturaItemDto
    {
        public string IdCFDI { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int Folio { get; set; }
        public string RfcReceptor { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IvaImporte { get; set; }
        public decimal Total { get; set; }
        public string FechaTimbre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public int Sucursal { get; set; }
        public string SucursalDesc { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public int EstatusCfdi { get; set; }
        public string EstatusSat { get; set; } = string.Empty;
        public string TipoDeComprobante { get; set; } = string.Empty;
    }

    public class CancelarFacturaRequestDto
    {
        [Required]
        public string Uuid { get; set; } = string.Empty;

        [Required]
        public string MotivoCancelacion { get; set; } = string.Empty;

        public string? UuidSustituye { get; set; }
    }

    public class CancelarFacturaResponseDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Acuse { get; set; } = string.Empty;
    }
}

