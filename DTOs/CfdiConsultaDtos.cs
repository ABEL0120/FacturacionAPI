namespace FacturacionAPI.DTOs
{
    public class CfdiConsultaItemDto
    {
        public string IdCfdi { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public long Folio { get; set; }
        public string RfcReceptor { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal IvaImporte { get; set; }
        public decimal Total { get; set; }
        public string FechaTimbre { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public decimal Orden { get; set; }
        public long Sucursal { get; set; }
        public string SucursalDesc { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
    }

    public class CfdiArchivoFacturaData
    {
        public string IdCfdi { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public long Folio { get; set; }
        public int IdCliente { get; set; }
        public string RfcReceptor { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
    }

    public class CfdiArchivoResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = System.Array.Empty<byte>();
    }

    public class EnviarCorreoCfdiRequestDto
    {
        public int NumeroCliente { get; set; }
        public string? CorreoDestino { get; set; }
    }
}
