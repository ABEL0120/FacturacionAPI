using System.ComponentModel.DataAnnotations;

namespace FacturacionAPI.DTOs
{
    public class LoginRequestDto
    {
        private string _contrasena = string.Empty;

        [Required(ErrorMessage = "El correo, usuario o teléfono es obligatorio.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contraseña
        {
            get => _contrasena;
            set => _contrasena = value;
        }

        public string Contrasena
        {
            get => _contrasena;
            set => _contrasena = string.IsNullOrWhiteSpace(_contrasena) ? value : _contrasena;
        }
    }

    public class LoginResponseDto
    {
        public int NumeroCliente { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool EsBfmafb { get; set; }
        public bool EsUsuario { get; set; }
        public string ModuloDestino { get; set; } = string.Empty;
    }

    public class RegistroClienteRequestDto
    {
        private string _contrasena = string.Empty;
        private string _confirmarContrasena = string.Empty;

        [Required(ErrorMessage = "El RFC es obligatorio.")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener entre 12 y 13 caracteres.")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(300, ErrorMessage = "La razón social no puede exceder los 300 caracteres.")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(80, ErrorMessage = "El correo no puede exceder los 80 caracteres.")]
        public string Mail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(13, MinimumLength = 3, ErrorMessage = "La contraseña debe tener entre 3 y 13 caracteres.")]
        public string Contraseña
        {
            get => _contrasena;
            set => _contrasena = value;
        }

        public string Contrasena
        {
            get => _contrasena;
            set => _contrasena = string.IsNullOrWhiteSpace(_contrasena) ? value : _contrasena;
        }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContraseña
        {
            get => _confirmarContrasena;
            set => _confirmarContrasena = value;
        }

        public string ConfirmarContrasena
        {
            get => _confirmarContrasena;
            set => _confirmarContrasena = string.IsNullOrWhiteSpace(_confirmarContrasena) ? value : _confirmarContrasena;
        }

        [StringLength(50, ErrorMessage = "El teléfono no puede exceder los 50 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La dirección no puede exceder los 100 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número exterior no puede exceder los 20 caracteres.")]
        public string NumeroExt { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El número interior no puede exceder los 20 caracteres.")]
        public string NumeroInt { get; set; } = string.Empty;

        [StringLength(60, ErrorMessage = "La colonia no puede exceder los 60 caracteres.")]
        public string Colonia { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La ciudad no puede exceder los 50 caracteres.")]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La delegación no puede exceder los 100 caracteres.")]
        public string Delegacion { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El estado no puede exceder los 100 caracteres.")]
        public string Estado { get; set; } = "COAHUILA";

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        [StringLength(6, MinimumLength = 5, ErrorMessage = "El código postal debe tener 5 dígitos.")]
        public string CodigoPostal { get; set; } = string.Empty;

        [StringLength(3, ErrorMessage = "El régimen fiscal debe tener 3 dígitos.")]
        public string RegimenFiscal { get; set; } = "612";

        [StringLength(3, ErrorMessage = "La forma de pago debe tener 2 o 3 dígitos.")]
        public string FormaPago { get; set; } = "01";

        [StringLength(3, ErrorMessage = "El uso de CFDI debe tener 3 dígitos.")]
        public string UsoCfdi { get; set; } = "G03";
    }

    public class ValidarCuentaRecuperacionRequestDto
    {
        [Required(ErrorMessage = "El RFC es obligatorio.")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
        public string Mail { get; set; } = string.Empty;
    }

    public class CuentaRecuperacionValidaDto
    {
        public int NumeroCliente { get; set; }
        public string RFC { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
    }

    public class RecuperarContrasenaRequestDto
    {
        [Required(ErrorMessage = "El RFC es obligatorio.")]
        public string RFC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido.")]
        public string Mail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(13, MinimumLength = 3, ErrorMessage = "La nueva contraseña debe tener entre 3 y 13 caracteres.")]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarNuevaContrasena { get; set; } = string.Empty;
    }

    public class CambiarContrasenaRequestDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Mail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(13, MinimumLength = 3, ErrorMessage = "La nueva contraseña debe tener entre 3 y 13 caracteres.")]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarNuevaContrasena { get; set; } = string.Empty;
    }
}
