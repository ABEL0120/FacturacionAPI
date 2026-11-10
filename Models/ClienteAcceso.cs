namespace FacturacionAPI.Models
{
    public class ClienteAcceso
    {
        public int Numero_Cliente { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
