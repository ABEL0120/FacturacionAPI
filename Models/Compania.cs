namespace FacturacionAPI.Models
{
    public class Compania
    {
        public short Numero_Compania { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Num_Ext { get; set; } = string.Empty;
        public string Num_Int { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string CP { get; set; } = string.Empty;
        public string CveRegimenFiscal { get; set; } = string.Empty;
        public string RegimenFiscalDesc { get; set; } = string.Empty;
        public string Numero_Certificado { get; set; } = string.Empty;
        public string Ubica_Certificado { get; set; } = string.Empty;
        public string Ubica_Llave { get; set; } = string.Empty;
        public string A_Password { get; set; } = string.Empty;
        public string Timbrado_Password { get; set; } = string.Empty;
        public string Prueba { get; set; } = "N";
        public string Serie_Factura { get; set; } = string.Empty;
        public string Serie_NotaCredito { get; set; } = string.Empty;
        public string Directorio_CFDI { get; set; } = string.Empty;
        public int Factura_FolioInicio { get; set; }
    }
}

