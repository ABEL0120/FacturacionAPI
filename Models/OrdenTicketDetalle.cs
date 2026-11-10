namespace FacturacionAPI.Models
{
    public class OrdenTicketDetalle
    {
        public int Orden { get; set; }
        public int Numero { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public int Sucursal { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string SerieFactura { get; set; } = string.Empty;
        public int Turno { get; set; }
        public string Folio { get; set; } = string.Empty;
        public int Transaccion { get; set; }
        public int Producto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string ClaveProdServ_SAT { get; set; } = string.Empty;
        public string ClaveProdServDesc { get; set; } = string.Empty;
        public string Unidad_Medida_SAT { get; set; } = string.Empty;
        public string Unidad_Medida { get; set; } = string.Empty;
        public string Exento { get; set; } = "N";
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioSinIva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ImpIva { get; set; }
        public decimal Porcentaje_Iva { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public decimal TotalFacturar { get; set; }
        public decimal Base_Iva16 { get; set; }
        public decimal Base_Iva0 { get; set; }
        public decimal Pago_Total_Vale { get; set; }
        public string Baja { get; set; } = string.Empty;
        public string IdCFDI { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public string EsGlobal { get; set; } = string.Empty;
        public string UuidGlobal { get; set; } = string.Empty;
    }

    public class FormaPagoOrdenInfo
    {
        public int Forma_Pago { get; set; }
        public string Forma_Pago_SAT { get; set; } = string.Empty;
        public string FormaPago_Desc { get; set; } = string.Empty;
        public string Forma_Pago_Descripcion { get; set; } = string.Empty;
    }
}

