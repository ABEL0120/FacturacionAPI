using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;
using FacturacionAPI.Repositories;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface ICfdiConsultaService
    {
        Task<ApiResponse<List<CfdiConsultaItemDto>>> ConsultarAsync(int numeroCliente, string? serie, string? folio);
        Task<CfdiArchivoResultDto> ObtenerArchivoAsync(string idCfdi, string tipo, int? numeroCliente = null);
        Task<ApiResponse<bool>> EnviarCorreoAsync(string idCfdi, int numeroCliente, string? correoDestino = null);
    }

    public class CfdiConsultaService : ICfdiConsultaService
    {
        private readonly ICfdiConsultaRepository _cfdiConsultaRepository;
        private readonly IFactTicketRepository _factTicketRepository;
        private readonly IClienteFiscalRepository _clienteFiscalRepository;

        public CfdiConsultaService(
            ICfdiConsultaRepository cfdiConsultaRepository,
            IFactTicketRepository factTicketRepository,
            IClienteFiscalRepository clienteFiscalRepository)
        {
            _cfdiConsultaRepository = cfdiConsultaRepository;
            _factTicketRepository = factTicketRepository;
            _clienteFiscalRepository = clienteFiscalRepository;
        }

        public async Task<ApiResponse<List<CfdiConsultaItemDto>>> ConsultarAsync(int numeroCliente, string? serie, string? folio)
        {
            if (numeroCliente <= 0)
            {
                return new ApiResponse<List<CfdiConsultaItemDto>>
                {
                    Success = true,
                    Message = "Sin cliente fiscal asociado.",
                    Data = new List<CfdiConsultaItemDto>()
                };
            }

            var lista = await _cfdiConsultaRepository.ConsultarCfdisAsync(numeroCliente, serie, folio);
            return new ApiResponse<List<CfdiConsultaItemDto>>
            {
                Success = true,
                Message = lista.Count > 0 ? "Comprobantes consultados correctamente." : "No se encontraron comprobantes CFDI 4.0.",
                Data = lista
            };
        }

        public async Task<CfdiArchivoResultDto> ObtenerArchivoAsync(string idCfdi, string tipo, int? numeroCliente = null)
        {
            string ext = NormalizarExtension(tipo);
            if (string.IsNullOrEmpty(ext))
            {
                return new CfdiArchivoResultDto
                {
                    Success = false,
                    Message = "Tipo de archivo no válido. Utiliza pdf o xml."
                };
            }

            if (string.IsNullOrWhiteSpace(idCfdi))
            {
                return new CfdiArchivoResultDto
                {
                    Success = false,
                    Message = "Identificador CFDI obligatorio."
                };
            }

            var compania = await _factTicketRepository.ObtenerCompaniaActivaAsync();
            if (compania == null)
            {
                return new CfdiArchivoResultDto
                {
                    Success = false,
                    Message = "No se encontró información de la compañía emisora."
                };
            }

            var cfdi = await _cfdiConsultaRepository.ObtenerCfdiPorIdAsync(idCfdi, numeroCliente);
            if (cfdi == null)
            {
                return new CfdiArchivoResultDto
                {
                    Success = false,
                    Message = "No existe el Archivo"
                };
            }

            string? rutaArchivo = ResolverRutaExistente(compania, cfdi, ext);
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                return new CfdiArchivoResultDto
                {
                    Success = false,
                    Message = "No existe el Archivo"
                };
            }

            byte[] bytes = await File.ReadAllBytesAsync(rutaArchivo);
            return new CfdiArchivoResultDto
            {
                Success = true,
                Message = "Archivo obtenido correctamente.",
                FileName = Path.GetFileName(rutaArchivo),
                ContentType = ext == ".pdf" ? "application/pdf" : "application/xml",
                Bytes = bytes
            };
        }

        public async Task<ApiResponse<bool>> EnviarCorreoAsync(string idCfdi, int numeroCliente, string? correoDestino = null)
        {
            if (string.IsNullOrWhiteSpace(idCfdi))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Identificador CFDI obligatorio.",
                    Data = false
                };
            }

            var compania = await _factTicketRepository.ObtenerCompaniaActivaAsync();
            if (compania == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No se encontró información de la compañía emisora.",
                    Data = false
                };
            }

            var cfdi = await _cfdiConsultaRepository.ObtenerCfdiPorIdAsync(idCfdi, numeroCliente > 0 ? numeroCliente : null);
            if (cfdi == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No existe el Archivo",
                    Data = false
                };
            }

            string? rutaXml = ResolverRutaExistente(compania, cfdi, ".xml");
            if (string.IsNullOrWhiteSpace(rutaXml))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No existe el Archivo",
                    Data = false
                };
            }

            string? rutaPdf = ResolverRutaExistente(compania, cfdi, ".pdf");
            if (string.IsNullOrWhiteSpace(rutaPdf) && string.IsNullOrWhiteSpace(rutaXml))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No hay archivos para enviar",
                    Data = false
                };
            }

            string destino = !string.IsNullOrWhiteSpace(correoDestino)
                ? correoDestino.Trim()
                : (numeroCliente > 0 ? (await _clienteFiscalRepository.ObtenerMailAccesoAsync(numeroCliente) ?? string.Empty).Trim() : string.Empty);

            if (string.IsNullOrWhiteSpace(destino))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "El cliente no cuenta con un correo electrónico registrado.",
                    Data = false
                };
            }

            string correoRemitente = DesEncripta(compania.Mail).Trim();
            string claveRemitente = DesEncripta(compania.Mail_Contrasena).Trim();
            if (string.IsNullOrWhiteSpace(correoRemitente) || string.IsNullOrWhiteSpace(claveRemitente))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No se ha podido enviar el correo",
                    Data = false
                };
            }

            try
            {
                using var mail = new MailMessage();
                mail.From = new MailAddress(correoRemitente, compania.Razon_Social.Trim(), Encoding.UTF8);
                mail.Subject = $"Envío de factura electronica Número {cfdi.IdCfdi}";
                mail.Body = "Envío de Factura ";
                mail.IsBodyHtml = true;
                mail.To.Add(destino);

                if (!string.IsNullOrWhiteSpace(rutaPdf) && File.Exists(rutaPdf))
                {
                    byte[] pdfBytes = await File.ReadAllBytesAsync(rutaPdf);
                    var pdfAttachment = new Attachment(new MemoryStream(pdfBytes), Path.GetFileName(rutaPdf), "application/pdf");
                    mail.Attachments.Add(pdfAttachment);
                }

                byte[] xmlBytes = await File.ReadAllBytesAsync(rutaXml);
                var xmlAttachment = new Attachment(new MemoryStream(xmlBytes), Path.GetFileName(rutaXml), "text/xml");
                mail.Attachments.Add(xmlAttachment);

                using var smtpServer = new SmtpClient("smtp.googlemail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(correoRemitente, claveRemitente),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };

                await smtpServer.SendMailAsync(mail);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Los archivos se han enviado a su correo",
                    Data = true
                };
            }
            catch
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "No se ha podido enviar el correo",
                    Data = false
                };
            }
        }

        private static string NormalizarExtension(string? tipo)
        {
            string limpio = (tipo ?? string.Empty).Trim().ToLowerInvariant();
            if (limpio == "pdf" || limpio == ".pdf") return ".pdf";
            if (limpio == "xml" || limpio == ".xml") return ".xml";
            return string.Empty;
        }

        private static string? ResolverRutaExistente(Compania compania, CfdiArchivoFacturaData cfdi, string extension)
        {
            string directorioBase = (compania.Directorio_CFDI ?? string.Empty).Trim();
            string rfcEmisor = (compania.RFC ?? string.Empty).Trim();
            string serie = (cfdi.Serie ?? string.Empty).Trim();
            string folioFormateado = cfdi.Folio.ToString("D7", CultureInfo.InvariantCulture);
            string nombreArchivo = $"{rfcEmisor}{serie}{folioFormateado}{extension}";

            if (string.IsNullOrWhiteSpace(directorioBase) || string.IsNullOrWhiteSpace(rfcEmisor))
            {
                return null;
            }

            string rutaPrincipal = Path.Combine(directorioBase, "Fact_Cfdi_XML_PDF", rfcEmisor, nombreArchivo);
            if (File.Exists(rutaPrincipal))
            {
                return rutaPrincipal;
            }

            string rutaAlterna = Path.Combine(directorioBase, nombreArchivo);
            if (File.Exists(rutaAlterna))
            {
                return rutaAlterna;
            }

            return null;
        }

        private static string DesEncripta(string pass)
        {
            string limpio = (pass ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(limpio))
            {
                return string.Empty;
            }

            if (limpio.Contains('@') || limpio.Length % 2 != 0)
            {
                return limpio;
            }

            const string clave = "BFMAFB";
            var sb = new StringBuilder(limpio.Length / 2);
            try
            {
                for (int i = 0, j = 0; i + 1 < limpio.Length; i += 2, j++)
                {
                    byte hexVal = Convert.ToByte(limpio.Substring(i, 2), 16);
                    char codigo = clave[j % clave.Length];
                    sb.Append((char)(codigo ^ hexVal));
                }
                return sb.ToString();
            }
            catch
            {
                return limpio;
            }
        }
    }
}
