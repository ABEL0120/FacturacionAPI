using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.DTOs
{
    public class ClienteDto
    {
        public int NumeroCliente { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string NumeroExt { get; set; } = string.Empty;
        public string NumeroInt { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string UsoCfdi { get; set; } = string.Empty;
        public string ValidadoReceptor { get; set; } = string.Empty;
    }

    public class CrearClienteDto
    {
        [Required]
        public int NumeroCliente { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 12)]
        public string RFC { get; set; } = string.Empty;

        [Required]
        public string RazonSocial { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;
        public string NumeroExt { get; set; } = string.Empty;
        public string NumeroInt { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string CodigoPostal { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string UsoCfdi { get; set; } = string.Empty;
    }

    public class ActualizarClienteDto
    {
        [Required]
        public int NumeroCliente { get; set; }

        [Required]
        public string RFC { get; set; } = string.Empty;

        [Required]
        public string RazonSocial { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;
        public string NumeroExt { get; set; } = string.Empty;
        public string NumeroInt { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Delegacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        [Required]
        public string CodigoPostal { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string UsoCfdi { get; set; } = string.Empty;
    }

    public class ClienteRfcItemDto
    {
        public int NumeroCliente { get; set; }
        public string RFC { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
    }
}

