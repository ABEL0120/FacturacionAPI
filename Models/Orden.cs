using System;

namespace FacturacionAPI.Models
{
    public class Orden
    {
        public int Numero { get; set; }
        public string Folio { get; set; } = string.Empty;
        public long Transaccion { get; set; }
        public int Sucursal { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public int Producto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ClaveProdServ_SAT { get; set; } = string.Empty;
        public string ClaveProdServDesc { get; set; } = string.Empty;
        public string Unidad_Medida_SAT { get; set; } = string.Empty;
        public string Unidad_Medida { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Porcentaje_Iva { get; set; }
        public decimal PrecioSinIva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal ImpIva { get; set; }
        public decimal Total { get; set; }
        public decimal Importe { get; set; }
        public decimal Pago_Total_Vale { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalFacturar { get; set; }
        public string uuid { get; set; } = string.Empty;
        public string uuidGlobal { get; set; } = string.Empty;
        public string idCFDI { get; set; } = string.Empty;
        public string EsGlobal { get; set; } = string.Empty;
        public string Baja { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Exento { get; set; } = string.Empty;
    }
}

