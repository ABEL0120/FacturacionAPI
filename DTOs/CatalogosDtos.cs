using System.Collections.Generic;

namespace FacturacionAPI.DTOs
{
    public class CatalogoItemDto
    {
        public string Numero { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ApFisica { get; set; }
        public string? ApMoral { get; set; }
    }

    public class MonedaCatalogoDto
    {
        public string Numero { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public int Decimales { get; set; }
    }

    public class FactTicketInitResponseDto
    {
        public bool EsPrueba { get; set; }
        public string EmisorRfc { get; set; } = string.Empty;
        public string EmisorRazonSocial { get; set; } = string.Empty;
        public string EmisorCp { get; set; } = string.Empty;
        public string EmisorRegimenFiscal { get; set; } = string.Empty;
        public List<ClienteRfcItemDto> RfcsCliente { get; set; } = new();
        public ClienteFiscalDto? ClienteSeleccionado { get; set; }
        public List<MonedaCatalogoDto> Monedas { get; set; } = new();
        public List<CatalogoItemDto> RegimenesFiscales { get; set; } = new();
        public List<CatalogoItemDto> UsosCfdi { get; set; } = new();
        public List<CatalogoItemDto> FormasPago { get; set; } = new();
        public List<CatalogoItemDto> EstadosMexico { get; set; } = new();
    }
}

