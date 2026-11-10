namespace FacturacionAPI.Models
{
    public class CfdiFactura
    {
        public string idCFDI { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int Folio { get; set; }
        public string RfcReceptor { get; set; } = string.Empty;
        public int idCliente { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Iva_Importe { get; set; }
        public decimal Total { get; set; }
        public string Fecha_Timbre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public int Sucursal { get; set; }
        public string SucursalDesc { get; set; } = string.Empty;
        public string uuid { get; set; } = string.Empty;
        public int Estatus_CFDI { get; set; }
        public string Estatus_SAT { get; set; } = string.Empty;
        public string TipoDeComprobante { get; set; } = string.Empty;
    }
}

