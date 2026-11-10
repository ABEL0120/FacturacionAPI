namespace FacturacionAPI.Models
{
    public class Cliente
    {
        public int Numero_Cliente { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Numero_Ext { get; set; } = string.Empty;
        public string Numero_Int { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Codigo_Postal { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string SAT_RegimenFiscal { get; set; } = string.Empty;
        public string SAT_FORMA_PAGO { get; set; } = string.Empty;
        public string Sat_Uso_Cfdi { get; set; } = string.Empty;
        public string ValidadoReceptor { get; set; } = "S";
    }
}

