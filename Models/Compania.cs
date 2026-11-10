namespace FacturacionAPI.Models
{
    public class Compania
    {
        public int Numero_Compañia { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CveRegimenFiscal { get; set; } = string.Empty;
        public string RegimenFiscalDesc { get; set; } = string.Empty;
        public string Directorio_CFDI { get; set; } = string.Empty;
        public string Ubica_Certificado { get; set; } = string.Empty;
        public string Ubica_Llave { get; set; } = string.Empty;
        public string Timbrado_Password { get; set; } = string.Empty;
        public string A_Password { get; set; } = string.Empty;
        public string Prueba { get; set; } = "N";
    }
}

