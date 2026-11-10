using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FacturacionAPI.Models
{
    public class ConceptoFactura40
    {
        public long Secuencia { get; set; }
        public string ClaveProdServ { get; set; } = string.Empty;
        public string NoIdentificacion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string ClaveUnidad { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal Descuento { get; set; }
        public decimal OtroDescuento { get; set; }
        public decimal TasaOCuota { get; set; }
        public decimal TasaIva { get; set; }
        public decimal ImpuestoIva { get; set; }
        public string NumeroPedimento { get; set; } = string.Empty;
        public string CuentaPredialNumero { get; set; } = string.Empty;
        public string ObjetoImp { get; set; } = "02";
    }

    public class ImpuestoConcepto40
    {
        public long Secuencia { get; set; }
        public string NoIdentificacion { get; set; } = string.Empty;
        public decimal Base { get; set; }
        public string Impuesto { get; set; } = string.Empty;
        public string TipoFactor { get; set; } = string.Empty;
        public decimal TasaOCuota { get; set; }
        public decimal Importe { get; set; }
    }

    public class Factura40
    {
        public string RfcProvCertif { get; set; } = string.Empty;
        public string Version { get; set; } = "4.0";
        public string Serie { get; set; } = string.Empty;
        public int Folio { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Sello { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string FormaPagoDesc { get; set; } = string.Empty;
        public string NoCertificado { get; set; } = string.Empty;
        public string Certificado { get; set; } = string.Empty;
        public string CondicionesDePago { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal OtroDescuento { get; set; }
        public string Moneda { get; set; } = "MXN";
        public string MonedaDesc { get; set; } = string.Empty;
        public decimal TipoCambio { get; set; } = 1m;
        public decimal Total { get; set; }
        public string TipoDeComprobante { get; set; } = "I";
        public string TipoDeComprobanteDesc { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = "PUE";
        public string MetodoPagoDesc { get; set; } = "Pago en una sola exhibición";
        public string LugarExpedicion { get; set; } = string.Empty;
        public string M_Letras { get; set; } = string.Empty;
        public string Confirmacion { get; set; } = string.Empty;
        public decimal TotalImpuestosRetenidos { get; set; }
        public decimal TotalImpuestosTrasladados { get; set; }
        public decimal ImpLocRetencion { get; set; }
        public decimal ImpLocTraslado { get; set; }

        public string TipoRelacion { get; set; } = string.Empty;
        public string TipoRelacionDesc { get; set; } = string.Empty;
        public string UuidRelacionados { get; set; } = string.Empty;

        public string EmisorRFC { get; set; } = string.Empty;
        public string EmisorNombre { get; set; } = string.Empty;
        public string EmisorCalle { get; set; } = string.Empty;
        public string EmisorNumExt { get; set; } = string.Empty;
        public string EmisorNumInt { get; set; } = string.Empty;
        public string EmisorColonia { get; set; } = string.Empty;
        public string EmisorDelegacion { get; set; } = string.Empty;
        public string EmisorMunicipio { get; set; } = string.Empty;
        public string EmisorCiudad { get; set; } = string.Empty;
        public string EmisorEstado { get; set; } = string.Empty;
        public string EmisorPais { get; set; } = string.Empty;
        public string EmisorCP { get; set; } = string.Empty;
        public string EmisorRegimenFiscal { get; set; } = string.Empty;
        public string EmisorRegimenFiscalDesc { get; set; } = string.Empty;

        public string RecepRFC { get; set; } = string.Empty;
        public string RecepNombre { get; set; } = string.Empty;
        public string RecepCalle { get; set; } = string.Empty;
        public string RecepColonia { get; set; } = string.Empty;
        public string RecepDelegacion { get; set; } = string.Empty;
        public string RecepCiudad { get; set; } = string.Empty;
        public string RecepEstado { get; set; } = string.Empty;
        public string RecepCP { get; set; } = string.Empty;
        public string RecepPais { get; set; } = string.Empty;
        public string RecepNumExt { get; set; } = string.Empty;
        public string RecepNumInt { get; set; } = string.Empty;
        public string RegimenFiscalReceptor { get; set; } = string.Empty;
        public string RegimenFiscalReceptorDesc { get; set; } = string.Empty;
        public string RecepResidenciaFiscal { get; set; } = string.Empty;
        public string RecepNumRegIdTrib { get; set; } = string.Empty;
        public string RecepUsoCFDI { get; set; } = string.Empty;
        public string RecepUsoCFDIDesc { get; set; } = string.Empty;

        public string XmlTim { get; set; } = string.Empty;
        public string CadenaOriginal { get; set; } = string.Empty;
        public string CadenaOriginalTimbre { get; set; } = string.Empty;
        public string DirXml { get; set; } = string.Empty;
        public string UbicaCertificado { get; set; } = string.Empty;
        public string CertificadoPassword { get; set; } = string.Empty;
        public string UbicaLlave { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;

        public DateTime FechaTimbre { get; set; }
        public string SelloSat { get; set; } = string.Empty;
        public string SerieSat { get; set; } = string.Empty;
        public string Exportacion { get; set; } = "01";
        public string Periodicidad { get; set; } = string.Empty;
        public string Meses { get; set; } = string.Empty;
        public string Anio { get; set; } = string.Empty;

        public List<ConceptoFactura40> Conceptos { get; } = new();
        public List<ImpuestoConcepto40> Retenciones { get; } = new();
        public List<ImpuestoConcepto40> Traslados { get; } = new();

        public void CalculaTotales()
        {
            TotalImpuestosRetenidos = Retenciones.Sum(r => r.Importe);
            TotalImpuestosTrasladados = Traslados.Sum(t => t.Importe);
            SubTotal = Math.Round(Conceptos.Sum(c => c.Importe), 2, MidpointRounding.AwayFromZero);
            Total = Math.Round(
                SubTotal - Descuento - OtroDescuento + (TotalImpuestosTrasladados + ImpLocTraslado) - (TotalImpuestosRetenidos + ImpLocRetencion),
                2,
                MidpointRounding.AwayFromZero);
            if (TipoDeComprobante == "P" || TipoDeComprobante == "T")
            {
                Total = 0m;
            }
        }

        public void CreaCadenaOriginal()
        {
            EsGlobal();
            CalculaTotales();

            var sb = new StringBuilder();
            sb.Append("||").Append(Version);
            sb.Append('|').Append(Serie);
            sb.Append('|').Append(Folio);
            sb.Append('|').Append(Fecha.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
            if (!string.IsNullOrWhiteSpace(FormaPago))
            {
                sb.Append('|').Append(FormaPago.Trim());
            }
            sb.Append('|').Append(NoCertificado.Trim());
            if (TipoDeComprobante != "T" && TipoDeComprobante != "P" && TipoDeComprobante != "N")
            {
                if (!string.IsNullOrWhiteSpace(CondicionesDePago))
                {
                    sb.Append('|').Append(CondicionesDePago.Trim());
                }
            }
            sb.Append('|').Append(SubTotal.ToString("0.00", CultureInfo.InvariantCulture));
            if (Descuento > 0 || OtroDescuento > 0)
            {
                sb.Append('|').Append((Descuento + OtroDescuento).ToString("0.00", CultureInfo.InvariantCulture));
            }
            if (TipoDeComprobante == "T")
            {
                Moneda = "XXX";
            }
            sb.Append('|').Append(Moneda);
            if (Moneda != "MXN" && Moneda != "XXX")
            {
                sb.Append('|').Append(TipoCambio.ToString("0.0000", CultureInfo.InvariantCulture));
            }
            sb.Append('|').Append(Total.ToString("0.00", CultureInfo.InvariantCulture));
            sb.Append('|').Append(TipoDeComprobante);
            sb.Append('|').Append(Exportacion);
            if (!string.IsNullOrWhiteSpace(MetodoPago))
            {
                sb.Append('|').Append(MetodoPago.Trim());
            }
            sb.Append('|').Append(LugarExpedicion.Trim());
            if (!string.IsNullOrWhiteSpace(Confirmacion))
            {
                sb.Append('|').Append(Confirmacion.Trim());
            }

            if (RecepRFC == "XAXX010101000" && !string.IsNullOrWhiteSpace(Periodicidad))
            {
                sb.Append('|').Append(Periodicidad);
                sb.Append('|').Append(Meses);
                sb.Append('|').Append(Anio);
            }

            if (!string.IsNullOrWhiteSpace(TipoRelacionDesc))
            {
                sb.Append('|').Append(TipoRelacion);
                if (!string.IsNullOrWhiteSpace(UuidRelacionados))
                {
                    foreach (var uuidRel in UuidRelacionados.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        sb.Append('|').Append(uuidRel.Trim());
                    }
                }
            }

            sb.Append('|').Append(EmisorRFC.Trim());
            sb.Append('|').Append(EmisorNombre.Trim());
            sb.Append('|').Append(Cod(EmisorRegimenFiscal));

            sb.Append('|').Append(RecepRFC.Trim());
            sb.Append('|').Append(RecepNombre.Trim());
            sb.Append('|').Append(RecepCP.Trim());
            if (!string.IsNullOrWhiteSpace(RecepResidenciaFiscal))
            {
                sb.Append('|').Append(RecepResidenciaFiscal.Trim());
            }
            if (!string.IsNullOrWhiteSpace(RecepNumRegIdTrib))
            {
                sb.Append('|').Append(RecepNumRegIdTrib.Trim());
            }
            sb.Append('|').Append(RegimenFiscalReceptor.Trim());
            sb.Append('|').Append(RecepUsoCFDI.Trim());

            foreach (var c in Conceptos)
            {
                sb.Append('|').Append(c.ClaveProdServ.Trim());
                if (TipoDeComprobante != "P")
                {
                    sb.Append('|').Append(Cod(c.NoIdentificacion));
                }
                sb.Append('|').Append(FormatoLocal(c.Cantidad));
                sb.Append('|').Append(c.ClaveUnidad.Trim());
                if (!string.IsNullOrWhiteSpace(c.Unidad))
                {
                    sb.Append('|').Append(c.Unidad.Trim());
                }
                sb.Append('|').Append(Cod(c.Descripcion.Trim()));
                sb.Append('|').Append(FormatoLocal(c.ValorUnitario));
                sb.Append('|').Append(FormatoLocal(c.Importe));
                if (c.Descuento > 0 || c.OtroDescuento > 0)
                {
                    sb.Append('|').Append((c.Descuento + c.OtroDescuento).ToString("0.00", CultureInfo.InvariantCulture));
                }
                sb.Append('|').Append(c.ObjetoImp);

                foreach (var t in Traslados.Where(x => x.Secuencia == c.Secuencia))
                {
                    sb.Append('|').Append(t.Base.ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(t.Impuesto);
                    sb.Append('|').Append(t.TipoFactor);
                    sb.Append('|').Append(t.TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(t.Importe.ToString("0.0000", CultureInfo.InvariantCulture));
                }

                foreach (var r in Retenciones.Where(x => x.Secuencia == c.Secuencia))
                {
                    sb.Append('|').Append(r.Base.ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(r.Impuesto);
                    sb.Append('|').Append(r.TipoFactor);
                    sb.Append('|').Append(r.TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(r.Importe.ToString("0.0000", CultureInfo.InvariantCulture));
                }

                if (!string.IsNullOrWhiteSpace(c.NumeroPedimento))
                {
                    sb.Append('|').Append(c.NumeroPedimento.Trim());
                }
                if (!string.IsNullOrWhiteSpace(c.CuentaPredialNumero))
                {
                    sb.Append('|').Append(c.CuentaPredialNumero.Trim());
                }
            }

            if (TipoDeComprobante != "T" && TipoDeComprobante != "P")
            {
                ElementoImpuestosCadena(sb);
            }

            sb.Append("||");
            CadenaOriginal = Regex.Replace(sb.ToString(), " {2,}", " ");
        }

        private void ElementoImpuestosCadena(StringBuilder sb)
        {
            if (Retenciones.Count > 0)
            {
                var retIsr = Retenciones.Where(r => r.Impuesto == "001").ToList();
                if (retIsr.Count > 0)
                {
                    sb.Append('|').Append(retIsr[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append("001");
                    sb.Append('|').Append(retIsr.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }

                var retIva = Retenciones.Where(r => r.Impuesto == "002").ToList();
                if (retIva.Count > 0)
                {
                    sb.Append('|').Append("002");
                    sb.Append('|').Append(retIva.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }

                var retIeps = Retenciones.Where(r => r.Impuesto == "003").ToList();
                if (retIeps.Count > 0)
                {
                    sb.Append('|').Append(retIeps[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append("003");
                    sb.Append('|').Append(retIeps.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }
            }

            if (TotalImpuestosRetenidos > 0)
            {
                sb.Append('|').Append(TotalImpuestosRetenidos.ToString("0.00", CultureInfo.InvariantCulture));
            }

            if (Traslados.Count > 0)
            {
                var trasIva0 = Traslados.Where(t => t.Impuesto == "002" && t.TasaOCuota == 0m).ToList();
                if (trasIva0.Count > 0)
                {
                    sb.Append('|').Append(trasIva0.Sum(x => x.Base).ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append("002");
                    sb.Append('|').Append(trasIva0[0].TipoFactor);
                    sb.Append('|').Append(trasIva0[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(trasIva0.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }

                var trasIva16 = Traslados.Where(t => t.Impuesto == "002" && t.TasaOCuota == 0.16m).ToList();
                if (trasIva16.Count > 0)
                {
                    sb.Append('|').Append(trasIva16.Sum(x => x.Base).ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append("002");
                    sb.Append('|').Append(trasIva16[0].TipoFactor);
                    sb.Append('|').Append(trasIva16[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(trasIva16.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }

                var trasIeps = Traslados.Where(t => t.Impuesto == "003").ToList();
                if (trasIeps.Count > 0)
                {
                    sb.Append('|').Append(trasIeps[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    sb.Append('|').Append("003");
                    sb.Append('|').Append(trasIeps[0].TipoFactor);
                    sb.Append('|').Append(trasIeps[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    sb.Append('|').Append(trasIeps.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                }

                sb.Append('|').Append(TotalImpuestosTrasladados.ToString("0.00", CultureInfo.InvariantCulture));
            }
        }

        public bool CreaFirma(out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(CadenaOriginal))
            {
                mensajeError = "La cadena original está vacía.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UbicaLlave) || !File.Exists(UbicaLlave))
            {
                mensajeError = "El key del certificado, no existe en el directorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UbicaCertificado) || !File.Exists(UbicaCertificado))
            {
                mensajeError = "El archivo .cer del certificado no existe en el directorio.";
                return false;
            }

            try
            {
                byte[] bytesCadena = Encoding.UTF8.GetBytes(CadenaOriginal);
                if (bytesCadena.Length == 0)
                {
                    mensajeError = "Error al realizar la digestión.";
                    return false;
                }

                using var cert = new X509Certificate2(UbicaCertificado);
                DateTime ahoraUtc = DateTime.UtcNow;
                if (ahoraUtc < cert.NotBefore.ToUniversalTime() || ahoraUtc > cert.NotAfter.ToUniversalTime())
                {
                    mensajeError = "Error, el certificado ya caducó.";
                    return false;
                }

                string password = CertificadoPassword ?? string.Empty;
                byte[] keyBytes = File.ReadAllBytes(UbicaLlave);

                using var rsa = RSA.Create();
                ImportarLlavePrivada(rsa, keyBytes, password);

                byte[] firma = rsa.SignData(bytesCadena, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                if (firma.Length == 0)
                {
                    mensajeError = "Error al realizar firma.";
                    return false;
                }

                using var rsaPublica = cert.GetRSAPublicKey();
                if (rsaPublica == null || !rsaPublica.VerifyData(bytesCadena, firma, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    mensajeError = "La llave privada (.key) no corresponde al certificado (.cer) asociado.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(NoCertificado))
                {
                    NoCertificado = ExtraerNumeroCertificado(cert);
                }

                Certificado = Convert.ToBase64String(cert.RawData);
                Sello = Convert.ToBase64String(firma);
                return true;
            }
            catch (CryptographicException)
            {
                mensajeError = "Error al leer la clave privada. Tal vez el Password no corresponde al archivo .key asociado.";
                return false;
            }
            catch (Exception ex)
            {
                mensajeError = $"Error al realizar firma: {ex.Message}";
                return false;
            }
        }

        public bool VerificarFirma(out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(UbicaCertificado) || !File.Exists(UbicaCertificado))
            {
                mensajeError = "El archivo .cer del certificado no existe en el directorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(CadenaOriginal) || string.IsNullOrWhiteSpace(Sello))
            {
                mensajeError = "La cadena original o el sello están vacíos.";
                return false;
            }

            try
            {
                using var cert = new X509Certificate2(UbicaCertificado);
                using var rsaPublica = cert.GetRSAPublicKey();
                if (rsaPublica == null)
                {
                    mensajeError = "El certificado no contiene llave pública RSA.";
                    return false;
                }

                byte[] bytesCadena = Encoding.UTF8.GetBytes(CadenaOriginal);
                byte[] firma = Convert.FromBase64String(Sello);
                bool valido = rsaPublica.VerifyData(bytesCadena, firma, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                if (!valido)
                {
                    mensajeError = "El sello generado no coincide con la llave pública del certificado.";
                }

                return valido;
            }
            catch (Exception ex)
            {
                mensajeError = $"Error al verificar la firma: {ex.Message}";
                return false;
            }
        }

        public static string ExtraerNumeroCertificado(X509Certificate2 cert)
        {
            string hex = cert.SerialNumber;
            if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(hex.Length / 2);
            for (int i = 0; i < hex.Length; i += 2)
            {
                sb.Append((char)Convert.ToByte(hex.Substring(i, 2), 16));
            }

            return sb.ToString().Trim();
        }

        private static void ImportarLlavePrivada(RSA rsa, byte[] keyBytes, string password)
        {
            string inicioTexto = Encoding.ASCII.GetString(keyBytes, 0, Math.Min(keyBytes.Length, 32));
            if (inicioTexto.StartsWith("-----", StringComparison.Ordinal))
            {
                string pem = Encoding.UTF8.GetString(keyBytes);
                rsa.ImportFromEncryptedPem(pem, password.AsSpan());
                return;
            }

            try
            {
                rsa.ImportEncryptedPkcs8PrivateKey(Encoding.UTF8.GetBytes(password), keyBytes, out _);
            }
            catch (CryptographicException)
            {
                rsa.ImportEncryptedPkcs8PrivateKey(Encoding.Latin1.GetBytes(password), keyBytes, out _);
            }
        }

        public string CreaXml()
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = false
            };

            using var sw = new Utf8StringWriter();
            using (var writer = XmlWriter.Create(sw, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("cfdi", "Comprobante", "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("xmlns", "cfdi", null, "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd");

                writer.WriteAttributeString("Version", Version);
                writer.WriteAttributeString("Serie", Serie);
                writer.WriteAttributeString("Folio", Folio.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Fecha", Fecha.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Sello", Sello);
                if (!string.IsNullOrWhiteSpace(FormaPago))
                {
                    writer.WriteAttributeString("FormaPago", FormaPago.Trim());
                }
                writer.WriteAttributeString("NoCertificado", NoCertificado.Trim());
                writer.WriteAttributeString("Certificado", Certificado);
                if (TipoDeComprobante != "T" && TipoDeComprobante != "P" && TipoDeComprobante != "N")
                {
                    if (!string.IsNullOrWhiteSpace(CondicionesDePago))
                    {
                        writer.WriteAttributeString("CondicionesDePago", CondicionesDePago.Trim());
                    }
                }
                writer.WriteAttributeString("SubTotal", SubTotal.ToString("0.00", CultureInfo.InvariantCulture));
                if (Descuento > 0 || OtroDescuento > 0)
                {
                    writer.WriteAttributeString("Descuento", (Descuento + OtroDescuento).ToString("0.00", CultureInfo.InvariantCulture));
                }
                if (TipoDeComprobante == "T")
                {
                    Moneda = "XXX";
                }
                writer.WriteAttributeString("Moneda", Moneda);
                if (Moneda != "MXN" && Moneda != "XXX")
                {
                    writer.WriteAttributeString("TipoCambio", TipoCambio.ToString("0.0000", CultureInfo.InvariantCulture));
                }
                writer.WriteAttributeString("Total", Total.ToString("0.00", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("TipoDeComprobante", TipoDeComprobante);
                writer.WriteAttributeString("Exportacion", Exportacion);
                if (!string.IsNullOrWhiteSpace(MetodoPago))
                {
                    writer.WriteAttributeString("MetodoPago", MetodoPago.Trim());
                }
                writer.WriteAttributeString("LugarExpedicion", LugarExpedicion.Trim());
                if (!string.IsNullOrWhiteSpace(Confirmacion))
                {
                    writer.WriteAttributeString("Confirmacion", Confirmacion.Trim());
                }

                if (RecepRFC == "XAXX010101000" && !string.IsNullOrWhiteSpace(Periodicidad))
                {
                    writer.WriteStartElement("cfdi", "InformacionGlobal", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Periodicidad", Periodicidad);
                    writer.WriteAttributeString("Meses", Meses);
                    writer.WriteAttributeString("Año", Anio);
                    writer.WriteEndElement();
                }

                if (!string.IsNullOrWhiteSpace(TipoRelacionDesc))
                {
                    writer.WriteStartElement("cfdi", "CfdiRelacionados", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("TipoRelacion", TipoRelacion);
                    if (!string.IsNullOrWhiteSpace(UuidRelacionados))
                    {
                        foreach (var uuidRel in UuidRelacionados.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            writer.WriteStartElement("cfdi", "CfdiRelacionado", "http://www.sat.gob.mx/cfd/4");
                            writer.WriteAttributeString("UUID", uuidRel.Trim());
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("cfdi", "Emisor", "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("Rfc", EmisorRFC.Trim());
                writer.WriteAttributeString("Nombre", EmisorNombre.Trim());
                writer.WriteAttributeString("RegimenFiscal", Cod(EmisorRegimenFiscal));
                writer.WriteEndElement();

                writer.WriteStartElement("cfdi", "Receptor", "http://www.sat.gob.mx/cfd/4");
                writer.WriteAttributeString("Rfc", RecepRFC.Trim());
                writer.WriteAttributeString("Nombre", RecepNombre.Trim());
                writer.WriteAttributeString("DomicilioFiscalReceptor", RecepCP.Trim());
                writer.WriteAttributeString("RegimenFiscalReceptor", RegimenFiscalReceptor.Trim());
                writer.WriteAttributeString("UsoCFDI", RecepUsoCFDI.Trim());
                writer.WriteEndElement();

                writer.WriteStartElement("cfdi", "Conceptos", "http://www.sat.gob.mx/cfd/4");
                foreach (var c in Conceptos)
                {
                    writer.WriteStartElement("cfdi", "Concepto", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("ClaveProdServ", Cod(c.ClaveProdServ));
                    if (TipoDeComprobante != "P")
                    {
                        writer.WriteAttributeString("NoIdentificacion", Cod(c.NoIdentificacion));
                    }
                    writer.WriteAttributeString("Cantidad", FormatoLocal(c.Cantidad));
                    writer.WriteAttributeString("ClaveUnidad", c.ClaveUnidad.Trim());
                    if (!string.IsNullOrWhiteSpace(c.Unidad))
                    {
                        writer.WriteAttributeString("Unidad", c.Unidad.Trim());
                    }
                    writer.WriteAttributeString("Descripcion", Cod(c.Descripcion.Trim()));
                    writer.WriteAttributeString("ValorUnitario", FormatoLocal(c.ValorUnitario));
                    writer.WriteAttributeString("Importe", FormatoLocal(c.Importe));
                    if (c.Descuento > 0 || c.OtroDescuento > 0)
                    {
                        writer.WriteAttributeString("Descuento", (c.Descuento + c.OtroDescuento).ToString("0.00", CultureInfo.InvariantCulture));
                    }
                    writer.WriteAttributeString("ObjetoImp", c.ObjetoImp);

                    var ct = Traslados.Where(x => x.Secuencia == c.Secuencia).ToList();
                    var cr = Retenciones.Where(x => x.Secuencia == c.Secuencia).ToList();
                    if (ct.Count > 0 || cr.Count > 0)
                    {
                        writer.WriteStartElement("cfdi", "Impuestos", "http://www.sat.gob.mx/cfd/4");
                        if (ct.Count > 0)
                        {
                            writer.WriteStartElement("cfdi", "Traslados", "http://www.sat.gob.mx/cfd/4");
                            foreach (var t in ct)
                            {
                                writer.WriteStartElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/4");
                                writer.WriteAttributeString("Base", t.Base.ToString("0.00", CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("Impuesto", t.Impuesto);
                                writer.WriteAttributeString("TipoFactor", t.TipoFactor);
                                writer.WriteAttributeString("TasaOCuota", t.TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("Importe", t.Importe.ToString("0.0000", CultureInfo.InvariantCulture));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        if (cr.Count > 0)
                        {
                            writer.WriteStartElement("cfdi", "Retenciones", "http://www.sat.gob.mx/cfd/4");
                            foreach (var r in cr)
                            {
                                writer.WriteStartElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/4");
                                writer.WriteAttributeString("Base", r.Base.ToString("0.00", CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("Impuesto", r.Impuesto);
                                writer.WriteAttributeString("TipoFactor", r.TipoFactor);
                                writer.WriteAttributeString("TasaOCuota", r.TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("Importe", r.Importe.ToString("0.0000", CultureInfo.InvariantCulture));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    if (!string.IsNullOrWhiteSpace(c.NumeroPedimento))
                    {
                        writer.WriteStartElement("cfdi", "InformacionAduanera", "http://www.sat.gob.mx/cfd/4");
                        writer.WriteAttributeString("NumeroPedimento", c.NumeroPedimento.Trim());
                        writer.WriteEndElement();
                    }
                    if (!string.IsNullOrWhiteSpace(c.CuentaPredialNumero))
                    {
                        writer.WriteStartElement("cfdi", "CuentaPredial", "http://www.sat.gob.mx/cfd/4");
                        writer.WriteAttributeString("Numero", c.CuentaPredialNumero.Trim());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                if (TipoDeComprobante != "T" && TipoDeComprobante != "P")
                {
                    ElementoImpuestosXml(writer);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }

            return sw.ToString();
        }

        private void ElementoImpuestosXml(XmlWriter writer)
        {
            writer.WriteStartElement("cfdi", "Impuestos", "http://www.sat.gob.mx/cfd/4");
            if (TotalImpuestosRetenidos > 0)
            {
                writer.WriteAttributeString("TotalImpuestosRetenidos", TotalImpuestosRetenidos.ToString("0.00", CultureInfo.InvariantCulture));
            }
            if (Traslados.Count > 0)
            {
                writer.WriteAttributeString("TotalImpuestosTrasladados", TotalImpuestosTrasladados.ToString("0.00", CultureInfo.InvariantCulture));
            }

            if (Retenciones.Count > 0)
            {
                writer.WriteStartElement("cfdi", "Retenciones", "http://www.sat.gob.mx/cfd/4");
                var retIsr = Retenciones.Where(r => r.Impuesto == "001").ToList();
                if (retIsr.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Base", retIsr[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Impuesto", "001");
                    writer.WriteAttributeString("Importe", retIsr.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }

                var retIva = Retenciones.Where(r => r.Impuesto == "002").ToList();
                if (retIva.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Impuesto", "002");
                    writer.WriteAttributeString("Importe", retIva.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }

                var retIeps = Retenciones.Where(r => r.Impuesto == "003").ToList();
                if (retIeps.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Retencion", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Base", retIeps[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Impuesto", "003");
                    writer.WriteAttributeString("Importe", retIeps.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (Traslados.Count > 0)
            {
                writer.WriteStartElement("cfdi", "Traslados", "http://www.sat.gob.mx/cfd/4");

                var trasIva0 = Traslados.Where(t => t.Impuesto == "002" && t.TasaOCuota == 0m).ToList();
                if (trasIva0.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Base", trasIva0.Sum(x => x.Base).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Impuesto", "002");
                    writer.WriteAttributeString("TipoFactor", trasIva0[0].TipoFactor);
                    writer.WriteAttributeString("TasaOCuota", trasIva0[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Importe", trasIva0.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }

                var trasIva16 = Traslados.Where(t => t.Impuesto == "002" && t.TasaOCuota == 0.16m).ToList();
                if (trasIva16.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Base", trasIva16.Sum(x => x.Base).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Impuesto", "002");
                    writer.WriteAttributeString("TipoFactor", trasIva16[0].TipoFactor);
                    writer.WriteAttributeString("TasaOCuota", trasIva16[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Importe", trasIva16.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }

                var trasIeps = Traslados.Where(t => t.Impuesto == "003").ToList();
                if (trasIeps.Count > 0)
                {
                    writer.WriteStartElement("cfdi", "Traslado", "http://www.sat.gob.mx/cfd/4");
                    writer.WriteAttributeString("Base", trasIeps[0].Base.ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Impuesto", "003");
                    writer.WriteAttributeString("TipoFactor", trasIeps[0].TipoFactor);
                    writer.WriteAttributeString("TasaOCuota", trasIeps[0].TasaOCuota.ToString("0.000000", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Importe", trasIeps.Sum(x => x.Importe).ToString("0.00", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void EsGlobal()
        {
            if (RecepRFC == "XAXX010101000")
            {
                RecepCP = LugarExpedicion;
                RegimenFiscalReceptor = "616";
                if (string.IsNullOrWhiteSpace(RecepUsoCFDI))
                {
                    RecepUsoCFDI = "S01";
                }
            }
        }

        private string FormatoLocal(decimal valor)
        {
            return TipoDeComprobante == "P"
                ? Math.Round(valor, 0, MidpointRounding.AwayFromZero).ToString("0", CultureInfo.InvariantCulture)
                : valor.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        public static string Cod(string? cadena)
        {
            if (string.IsNullOrEmpty(cadena))
            {
                return string.Empty;
            }

            string limpio = cadena
                .Replace("Ñ", "N")
                .Replace("ñ", "n")
                .Replace("ó", "o")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ú", "u")
                .Replace("Ó", "O")
                .Replace("Á", "A")
                .Replace("É", "E")
                .Replace("Í", "I")
                .Replace("Ú", "U")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\v", " ");

            limpio = Regex.Replace(limpio, " {2,}", " ");
            var sb = new StringBuilder();
            foreach (char ch in limpio)
            {
                if (ch >= 32 && ch <= 126)
                {
                    sb.Append(ch);
                }
            }
            return Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public static string NumALetras(decimal importe, string nombreMoneda, string abreviatura)
        {
            decimal redondeado = Math.Round(importe, 2, MidpointRounding.AwayFromZero);
            long enteros = (long)Math.Floor(Math.Abs(redondeado));
            int centavos = (int)Math.Round((Math.Abs(redondeado) - enteros) * 100m, 0, MidpointRounding.AwayFromZero);

            string textoEntero = enteros == 0 ? "CERO" : ConvertirEnteroALetras(enteros);
            string moneda = string.IsNullOrWhiteSpace(nombreMoneda) ? "PESOS" : nombreMoneda.Trim().ToUpperInvariant();
            if (moneda == "DOLAR")
            {
                moneda = "DOLARES";
            }
            else if (enteros == 1 && (moneda == "PESO" || moneda == "PESOS" || moneda.Contains("MEXICANO")))
            {
                moneda = "PESO";
            }
            else if (moneda.Contains("MEXICANO"))
            {
                moneda = "PESOS";
            }

            string unidadMonetaria = enteros >= 1_000_000 && enteros % 1_000_000 == 0
                ? $"{textoEntero} DE {moneda}"
                : $"{textoEntero} {moneda}";

            string abrev = string.IsNullOrWhiteSpace(abreviatura) ? "MXN" : abreviatura.Trim().ToUpperInvariant();
            string dec = centavos.ToString("00", CultureInfo.InvariantCulture);
            return $"({unidadMonetaria} {dec}/100 {abrev})";
        }

        private static string ConvertirEnteroALetras(long numero)
        {
            if (numero == 0) return "CERO";
            if (numero < 1000) return ConvertirMenorMil((int)numero);
            if (numero < 1_000_000)
            {
                long miles = numero / 1000;
                int resto = (int)(numero % 1000);
                string textoMiles = miles == 1 ? "UN MIL" : $"{ConvertirMenorMil((int)miles)} MIL";
                return resto > 0 ? $"{textoMiles} {ConvertirMenorMil(resto)}" : textoMiles;
            }
            long millones = numero / 1_000_000;
            long restoMillon = numero % 1_000_000;
            string textoMillon = millones == 1 ? "UN MILLON" : $"{ConvertirEnteroALetras(millones)} MILLONES";
            return restoMillon > 0 ? $"{textoMillon} {ConvertirEnteroALetras(restoMillon)}" : textoMillon;
        }

        private static string ConvertirMenorMil(int numero)
        {
            string[] unidades = { "", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            string[] especiales = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISEIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
            string[] veintis = { "VEINTE", "VEINTIUN", "VEINTIDOS", "VEINTITRES", "VEINTICUATRO", "VEINTICINCO", "VEINTISEIS", "VEINTISIETE", "VEINTIOCHO", "VEINTINUEVE" };
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };

            if (numero == 100) return "CIEN";

            var partes = new List<string>();
            int c = numero / 100;
            int du = numero % 100;
            if (c > 0) partes.Add(centenas[c]);

            if (du >= 10 && du <= 19)
            {
                partes.Add(especiales[du - 10]);
            }
            else if (du >= 20 && du <= 29)
            {
                partes.Add(veintis[du - 20]);
            }
            else
            {
                int d = du / 10;
                int u = du % 10;
                if (d > 0 && u > 0)
                {
                    partes.Add($"{decenas[d]} Y {unidades[u]}");
                }
                else if (d > 0)
                {
                    partes.Add(decenas[d]);
                }
                else if (u > 0)
                {
                    partes.Add(unidades[u]);
                }
            }

            return string.Join(" ", partes);
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => new UTF8Encoding(false);
        }
    }
}

