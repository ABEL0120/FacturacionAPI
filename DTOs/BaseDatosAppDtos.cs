namespace FacturacionAPI.DTOs
{
    public class BaseDatosAppDto
    {
        public int Numero { get; set; }
        public string IdApp { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Servidor { get; set; } = string.Empty;
        public string NombreBase { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Acceso { get; set; } = string.Empty;
    }
}

