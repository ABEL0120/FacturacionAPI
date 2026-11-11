using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public class RegistroCfdiPersistenciaData
    {
        public string Serie { get; set; } = string.Empty;
        public int Folio { get; set; }
        public int IdCliente { get; set; }
        public string UsoCfdi { get; set; } = string.Empty;
        public string Moneda { get; set; } = "MXN";
        public string TipoDeComprobante { get; set; } = "I";
        public string VersionCfdi { get; set; } = "4.0";
        public string EsPrueba { get; set; } = "N";
        public decimal SubTotal { get; set; }
        public decimal IvaImporte { get; set; }
        public decimal Total { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = "PUE";
        public string SatTipoRelacion { get; set; } = string.Empty;
        public string UuidRelacionados { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public decimal TipoCambio { get; set; } = 1m;
        public string SatExportacion { get; set; } = "01";
        public int Orden { get; set; }
        public int Sucursal { get; set; }
        public string OrdenFecha { get; set; } = string.Empty;
        public string RfcReceptor { get; set; } = string.Empty;
        public string RelGlobal { get; set; } = string.Empty;
        public decimal BaseIva16 { get; set; }
        public decimal BaseIva0 { get; set; }
        public decimal PagoTotalVale { get; set; }
    }

    public class RegistrarFacturaResultado
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string IdCfdiFactura { get; set; } = string.Empty;
        public int FolioFactura { get; set; }
        public string IdCfdiNotaCredito { get; set; } = string.Empty;
        public int FolioNotaCredito { get; set; }
    }

    public interface IFactTicketRepository
    {
        Task<Compania?> ObtenerCompaniaActivaAsync();
        Task<List<OrdenTicketDetalle>> ObtenerPartidasPorFoliosCandidatosAsync(List<string> foliosCandidatos, int? transaccion);
        Task<List<OrdenTicketDetalle>> ObtenerPartidasPorOrdenAsync(long orden, int? transaccion);
        Task<FormaPagoOrdenInfo?> ObtenerFormaPagoOrdenAsync(int sucursal, int orden, string fecha);
        Task<string?> ObtenerSerieFacturaSucursalAsync(int sucursal);
        Task<string?> VerificarFolioYaTimbradoIndividualAsync(string folio);
        Task<int> ObtenerSiguienteFolioPreviewAsync(string serie);
        Task<RegistrarFacturaResultado> RegistrarFacturaGeneradaAsync(
            string serieFactura,
            string? serieNotaCredito,
            List<OrdenTicketDetalle> partidasDetalle,
            List<OrdenTicketDetalle> partidasGlobales,
            Func<int, int?, (bool Success, string Message, RegistroCfdiPersistenciaData? Factura, RegistroCfdiPersistenciaData? NotaCredito)> prepararYFirmar);
    }

    public class FactTicketRepository : IFactTicketRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public FactTicketRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<Compania?> ObtenerCompaniaActivaAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1
                                        a.Numero_Compañia AS Numero_Compania,
                                        RTRIM(ISNULL(a.Razon_Social, '')) AS Razon_Social,
                                        RTRIM(ISNULL(a.RFC, '')) AS RFC,
                                        RTRIM(ISNULL(a.Direccion, '')) AS Direccion,
                                        RTRIM(ISNULL(a.Num_Ext, '')) AS Num_Ext,
                                        RTRIM(ISNULL(a.Num_Int, '')) AS Num_Int,
                                        RTRIM(ISNULL(a.Colonia, '')) AS Colonia,
                                        RTRIM(ISNULL(a.Delegacion, '')) AS Delegacion,
                                        RTRIM(ISNULL(a.Ciudad, '')) AS Ciudad,
                                        RTRIM(ISNULL(a.Estado, '')) AS Estado,
                                        RTRIM(ISNULL(a.Pais, '')) AS Pais,
                                        RTRIM(ISNULL(a.CP, '')) AS CP,
                                        RTRIM(ISNULL(a.CveRegimenFiscal, '')) AS CveRegimenFiscal,
                                        RTRIM(ISNULL(b.Descripcion, '')) AS RegimenFiscalDesc,
                                        RTRIM(ISNULL(a.Numero_Certificado, '')) AS Numero_Certificado,
                                        RTRIM(ISNULL(a.Ubica_Certificado, '')) AS Ubica_Certificado,
                                        RTRIM(ISNULL(a.Ubica_Llave, '')) AS Ubica_Llave,
                                        RTRIM(ISNULL(a.A_Password, '')) AS A_Password,
                                        RTRIM(ISNULL(a.Timbrado_Password, '')) AS Timbrado_Password,
                                        RTRIM(ISNULL(a.Prueba, 'N')) AS Prueba,
                                        RTRIM(ISNULL(a.Serie_Factura, '')) AS Serie_Factura,
                                        RTRIM(ISNULL(a.Serie_NotaCredito, '')) AS Serie_NotaCredito,
                                        RTRIM(ISNULL(a.Directorio_CFDI, '')) AS Directorio_CFDI,
                                        ISNULL(a.Factura_FolioInicio, 1) AS Factura_FolioInicio
                                 FROM [Compañias] a
                                 LEFT JOIN SAT_REGIMEN_FISCAL b ON a.CveRegimenFiscal = b.Numero";
            return await conn.QueryFirstOrDefaultAsync<Compania>(sql);
        }

        public async Task<List<OrdenTicketDetalle>> ObtenerPartidasPorFoliosCandidatosAsync(List<string> foliosCandidatos, int? transaccion)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            string sql = @"SELECT
                               a.Orden,
                               a.Numero,
                               RTRIM(ISNULL(a.Tipo, '')) AS Tipo,
                               RTRIM(ISNULL(a.Fecha, '')) AS Fecha,
                               a.Sucursal,
                               RTRIM(ISNULL(s.Nombre, '')) AS SucursalNombre,
                               RTRIM(ISNULL(s.SerieFactura, '')) AS SerieFactura,
                               a.Turno,
                               RTRIM(ISNULL(a.Folio, '')) AS Folio,
                               ISNULL(a.Transaccion, 0) AS Transaccion,
                               a.Producto,
                               RTRIM(ISNULL(b.Nombre, '')) AS Descripcion,
                               RTRIM(ISNULL(b.ClaveProdServ_SAT, '')) AS ClaveProdServ_SAT,
                               RTRIM(ISNULL(d.Descripcion, '')) AS ClaveProdServDesc,
                               RTRIM(ISNULL(b.Unidad_Medida_SAT, '')) AS Unidad_Medida_SAT,
                               RTRIM(ISNULL(b.Unidad_Medida, '')) AS Unidad_Medida,
                               RTRIM(ISNULL(b.Exento, 'N')) AS Exento,
                               CAST(ISNULL(a.Cantidad, 0) AS decimal(18,4)) AS Cantidad,
                               CAST(ISNULL(a.Precio, 0) AS decimal(18,6)) AS Precio,
                               CAST(ROUND(ISNULL(a.Precio, 0) / NULLIF((ISNULL(a.Porcentaje_Iva, 0) * 0.01 + 1), 0), 6) AS decimal(18,6)) AS PrecioSinIva,
                               CAST(ISNULL(a.Subtotal, 0) AS decimal(18,4)) AS Subtotal,
                               CAST(ISNULL(a.Iva, 0) AS decimal(18,4)) AS ImpIva,
                               CAST(ISNULL(a.Porcentaje_Iva, 0) AS decimal(18,4)) AS Porcentaje_Iva,
                               CAST(ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS Descuento,
                               CAST(ISNULL(a.Total, 0) AS decimal(18,4)) AS Total,
                               CAST(ISNULL(a.Subtotal, 0) + ISNULL(a.Iva, 0) - ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS TotalFacturar,
                               CAST(ISNULL(a.Base_Iva16, 0) AS decimal(18,4)) AS Base_Iva16,
                               CAST(ISNULL(a.Base_Iva0, 0) AS decimal(18,4)) AS Base_Iva0,
                               CAST(ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS Pago_Total_Vale,
                               RTRIM(ISNULL(a.Baja, '')) AS Baja,
                               RTRIM(ISNULL(a.idCFDI, '')) AS IdCFDI,
                               RTRIM(ISNULL(a.uuid, '')) AS Uuid,
                               RTRIM(ISNULL(c.EsGlobal, '')) AS EsGlobal,
                               RTRIM(ISNULL(c.UuidGlobal, '')) AS UuidGlobal
                           FROM Ordenes a
                           LEFT JOIN Productos b ON a.Producto = b.Numero
                           LEFT JOIN Sucursal s ON a.Sucursal = s.Numero
                           LEFT JOIN SAT_CLAVE_PROD_SERV d ON b.ClaveProdServ_SAT = d.Numero
                           OUTER APPLY (
                               SELECT TOP 1 cf.EsGlobal, cf.uuid AS UuidGlobal
                               FROM CFDI_Factura cf
                               WHERE (a.idCFDI <> '' AND cf.idCFDI = a.idCFDI)
                                  OR (a.uuid <> '' AND cf.uuid = a.uuid)
                           ) c
                           WHERE a.Folio IN @Folios
                             AND a.Producto > 0";

            if (transaccion.HasValue && transaccion.Value > 0)
            {
                sql += " AND a.Transaccion = @Transaccion";
            }

            sql += " ORDER BY a.Numero";

            var res = await conn.QueryAsync<OrdenTicketDetalle>(sql, new
            {
                Folios = foliosCandidatos,
                Transaccion = transaccion ?? 0
            });
            return res.ToList();
        }

        public async Task<List<OrdenTicketDetalle>> ObtenerPartidasPorOrdenAsync(long orden, int? transaccion)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            string sql = @"SELECT
                               a.Orden,
                               a.Numero,
                               RTRIM(ISNULL(a.Tipo, '')) AS Tipo,
                               RTRIM(ISNULL(a.Fecha, '')) AS Fecha,
                               a.Sucursal,
                               RTRIM(ISNULL(s.Nombre, '')) AS SucursalNombre,
                               RTRIM(ISNULL(s.SerieFactura, '')) AS SerieFactura,
                               a.Turno,
                               RTRIM(ISNULL(a.Folio, '')) AS Folio,
                               ISNULL(a.Transaccion, 0) AS Transaccion,
                               a.Producto,
                               RTRIM(ISNULL(b.Nombre, '')) AS Descripcion,
                               RTRIM(ISNULL(b.ClaveProdServ_SAT, '')) AS ClaveProdServ_SAT,
                               RTRIM(ISNULL(d.Descripcion, '')) AS ClaveProdServDesc,
                               RTRIM(ISNULL(b.Unidad_Medida_SAT, '')) AS Unidad_Medida_SAT,
                               RTRIM(ISNULL(b.Unidad_Medida, '')) AS Unidad_Medida,
                               RTRIM(ISNULL(b.Exento, 'N')) AS Exento,
                               CAST(ISNULL(a.Cantidad, 0) AS decimal(18,4)) AS Cantidad,
                               CAST(ISNULL(a.Precio, 0) AS decimal(18,6)) AS Precio,
                               CAST(ROUND(ISNULL(a.Precio, 0) / NULLIF((ISNULL(a.Porcentaje_Iva, 0) * 0.01 + 1), 0), 6) AS decimal(18,6)) AS PrecioSinIva,
                               CAST(ISNULL(a.Subtotal, 0) AS decimal(18,4)) AS Subtotal,
                               CAST(ISNULL(a.Iva, 0) AS decimal(18,4)) AS ImpIva,
                               CAST(ISNULL(a.Porcentaje_Iva, 0) AS decimal(18,4)) AS Porcentaje_Iva,
                               CAST(ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS Descuento,
                               CAST(ISNULL(a.Total, 0) AS decimal(18,4)) AS Total,
                               CAST(ISNULL(a.Subtotal, 0) + ISNULL(a.Iva, 0) - ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS TotalFacturar,
                               CAST(ISNULL(a.Base_Iva16, 0) AS decimal(18,4)) AS Base_Iva16,
                               CAST(ISNULL(a.Base_Iva0, 0) AS decimal(18,4)) AS Base_Iva0,
                               CAST(ISNULL(a.Pago_Total_Vale, 0) AS decimal(18,4)) AS Pago_Total_Vale,
                               RTRIM(ISNULL(a.Baja, '')) AS Baja,
                               RTRIM(ISNULL(a.idCFDI, '')) AS IdCFDI,
                               RTRIM(ISNULL(a.uuid, '')) AS Uuid,
                               RTRIM(ISNULL(c.EsGlobal, '')) AS EsGlobal,
                               RTRIM(ISNULL(c.UuidGlobal, '')) AS UuidGlobal
                           FROM Ordenes a
                           LEFT JOIN Productos b ON a.Producto = b.Numero
                           LEFT JOIN Sucursal s ON a.Sucursal = s.Numero
                           LEFT JOIN SAT_CLAVE_PROD_SERV d ON b.ClaveProdServ_SAT = d.Numero
                           OUTER APPLY (
                               SELECT TOP 1 cf.EsGlobal, cf.uuid AS UuidGlobal
                               FROM CFDI_Factura cf
                               WHERE (a.idCFDI <> '' AND cf.idCFDI = a.idCFDI)
                                  OR (a.uuid <> '' AND cf.uuid = a.uuid)
                           ) c
                           WHERE a.Orden = @Orden
                             AND a.Producto > 0";

            if (transaccion.HasValue && transaccion.Value > 0)
            {
                sql += " AND a.Transaccion = @Transaccion";
            }

            sql += " ORDER BY a.Fecha DESC, a.Numero";

            var res = await conn.QueryAsync<OrdenTicketDetalle>(sql, new
            {
                Orden = orden,
                Transaccion = transaccion ?? 0
            });
            return res.ToList();
        }

        public async Task<FormaPagoOrdenInfo?> ObtenerFormaPagoOrdenAsync(int sucursal, int orden, string fecha)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1
                                        b.Forma_Pago,
                                        RTRIM(ISNULL(a.Forma_Pago_SAT, '')) AS Forma_Pago_SAT,
                                        RTRIM(ISNULL(sfp.Descripcion, '')) AS FormaPago_Desc,
                                        RTRIM(ISNULL(b.Forma_Pago_Descripcion, '')) AS Forma_Pago_Descripcion
                                 FROM Ordenes_Pagos b
                                 LEFT JOIN Forma_Pago a ON a.Numero = b.Forma_Pago
                                 LEFT JOIN SAT_FORMA_PAGO sfp ON sfp.Numero = a.Forma_Pago_SAT
                                 WHERE b.Sucursal = @Sucursal
                                   AND b.Orden = @Orden
                                   AND b.Fecha = @Fecha";
            return await conn.QueryFirstOrDefaultAsync<FormaPagoOrdenInfo>(sql, new
            {
                Sucursal = sucursal,
                Orden = orden,
                Fecha = fecha
            });
        }

        public async Task<string?> ObtenerSerieFacturaSucursalAsync(int sucursal)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT TOP 1 RTRIM(ISNULL(SerieFactura, '')) FROM Sucursal WHERE Numero = @Sucursal";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Sucursal = sucursal });
        }

        public async Task<string?> VerificarFolioYaTimbradoIndividualAsync(string folio)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(a.Folio)
                                 FROM Ordenes a
                                 LEFT JOIN CFDI_Factura c ON (a.idCFDI <> '' AND c.idCFDI = a.idCFDI)
                                                          OR (a.uuid <> '' AND c.uuid = a.uuid)
                                 WHERE a.Folio = @Folio
                                   AND ISNULL(a.Baja, '') <> '*'
                                   AND a.Producto > 0
                                   AND a.Tipo <> 'Servicio'
                                   AND (ISNULL(a.idCFDI, '') <> '' OR ISNULL(a.uuid, '') <> '')
                                   AND ISNULL(c.EsGlobal, '') <> 'S'";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Folio = folio });
        }

        public async Task<int> ObtenerSiguienteFolioPreviewAsync(string serie)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT CAST(ISNULL(MAX(Folio), 0) + 1 AS int) FROM CFDI_Factura WHERE Serie = @Serie";
            return await conn.ExecuteScalarAsync<int>(sql, new { Serie = serie });
        }

        public async Task<RegistrarFacturaResultado> RegistrarFacturaGeneradaAsync(
            string serieFactura,
            string? serieNotaCredito,
            List<OrdenTicketDetalle> partidasDetalle,
            List<OrdenTicketDetalle> partidasGlobales,
            Func<int, int?, (bool Success, string Message, RegistroCfdiPersistenciaData? Factura, RegistroCfdiPersistenciaData? NotaCredito)> prepararYFirmar)
        {
            if (partidasDetalle.Count == 0)
            {
                return new RegistrarFacturaResultado
                {
                    Success = false,
                    Message = "No hay partidas para facturar."
                };
            }

            using IDbConnection conn = _dbConnectionData.CreateConnection();
            conn.Open();
            using IDbTransaction tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                const string sqlVerificarOrden = @"SELECT TOP 1 RTRIM(a.Folio)
                                                   FROM Ordenes a WITH (UPDLOCK, HOLDLOCK)
                                                   LEFT JOIN CFDI_Factura c ON (a.idCFDI <> '' AND c.idCFDI = a.idCFDI)
                                                                            OR (a.uuid <> '' AND c.uuid = a.uuid)
                                                   WHERE a.Sucursal = @Sucursal
                                                     AND a.Fecha = @Fecha
                                                     AND a.Orden = @Orden
                                                     AND ISNULL(a.Baja, '') <> '*'
                                                     AND a.Producto > 0
                                                     AND a.Tipo <> 'Servicio'
                                                     AND (ISNULL(a.idCFDI, '') <> '' OR ISNULL(a.uuid, '') <> '')
                                                     AND ISNULL(c.EsGlobal, '') <> 'S'";

                foreach (var grupoTicket in partidasDetalle.GroupBy(p => new { p.Sucursal, p.Fecha, p.Orden }))
                {
                    string? folioFacturado = await conn.QueryFirstOrDefaultAsync<string>(
                        sqlVerificarOrden,
                        new
                        {
                            grupoTicket.Key.Sucursal,
                            grupoTicket.Key.Fecha,
                            grupoTicket.Key.Orden
                        },
                        tx);

                    if (!string.IsNullOrWhiteSpace(folioFacturado))
                    {
                        tx.Rollback();
                        return new RegistrarFacturaResultado
                        {
                            Success = false,
                            Message = "El Ticket ya ha sido Facturado."
                        };
                    }
                }

                const string sqlSiguienteFolio = @"SELECT CAST(ISNULL(MAX(Folio), 0) + 1 AS int)
                                                   FROM CFDI_Factura WITH (UPDLOCK, HOLDLOCK)
                                                   WHERE Serie = @Serie";

                int folioFactura = await conn.ExecuteScalarAsync<int>(sqlSiguienteFolio, new { Serie = serieFactura }, tx);
                int? folioNc = !string.IsNullOrWhiteSpace(serieNotaCredito) && partidasGlobales.Count > 0
                    ? await conn.ExecuteScalarAsync<int>(sqlSiguienteFolio, new { Serie = serieNotaCredito }, tx)
                    : null;

                var firmado = prepararYFirmar(folioFactura, folioNc);
                if (!firmado.Success || firmado.Factura == null)
                {
                    tx.Rollback();
                    return new RegistrarFacturaResultado
                    {
                        Success = false,
                        Message = firmado.Message
                    };
                }

                const string sqlInsertCfdiFactura = @"INSERT INTO CFDI_Factura (
                                                          idCFDI, Serie, Folio, idCliente, UsoCFDI, Moneda,
                                                          TipoDeComprobante, Version_CFDI, Estatus_CFDI, Fecha_Timbre, uuid, Es_Prueba,
                                                          Sub_Total, Total, Forma_Pago, Metodo_Pago, Sat_Tipo_Relacion, UUID_Relacionados,
                                                          Tipo_Documento, Fecha, Iva_Importe, Concepto_Poliza, Tipo_Cambio, SatExportacion,
                                                          Orden, RfcReceptor, Sucursal, Orden_Fecha, EsGlobal, Ultimo_Correo_Enviado,
                                                          Rel_Global, Base_Iva16, Base_Iva0, Pago_Total_Vale
                                                      ) VALUES (
                                                          @IdCfdi, @Serie, @Folio, @IdCliente, @UsoCfdi, @Moneda,
                                                          @TipoDeComprobante, @VersionCfdi, 0, '', '', @EsPrueba,
                                                          @SubTotal, @Total, @FormaPago, @MetodoPago, @SatTipoRelacion, @UuidRelacionados,
                                                          1, @Fecha, @IvaImporte, '', @TipoCambio, @SatExportacion,
                                                          @Orden, @RfcReceptor, @Sucursal, @OrdenFecha, '', '',
                                                          @RelGlobal, @BaseIva16, @BaseIva0, @PagoTotalVale
                                                      )";

                string idCfdiNc = string.Empty;
                int folioNcFinal = 0;

                if (firmado.NotaCredito != null && folioNc.HasValue)
                {
                    idCfdiNc = ConstruirIdCfdi(firmado.NotaCredito.Serie, firmado.NotaCredito.Folio);
                    folioNcFinal = firmado.NotaCredito.Folio;

                    await conn.ExecuteAsync(sqlInsertCfdiFactura, ConstruirParametrosCfdi(idCfdiNc, firmado.NotaCredito), tx);

                    const string sqlInsertRelacionados = @"INSERT INTO CFDI_Relacionados (idCFDI, UUID_Rel, satTipoRelacion)
                                                           VALUES (@IdCfdi, @UuidRel, '01')";

                    foreach (string uuidRel in partidasGlobales
                        .Select(p => p.Uuid.Trim())
                        .Where(u => !string.IsNullOrWhiteSpace(u))
                        .Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        await conn.ExecuteAsync(
                            sqlInsertRelacionados,
                            new { IdCfdi = idCfdiNc, UuidRel = Recortar(uuidRel, 45) },
                            tx);
                    }

                    const string sqlInsertCfdiDetalle = @"INSERT INTO CFDI_Detalle (idCFDI, sec, idOrdenes)
                                                          VALUES (@IdCfdi, @Sec, @IdOrdenes)";

                    int sec = 0;
                    foreach (var partidaGlobal in partidasGlobales)
                    {
                        sec++;
                        string idOrdenes = Recortar(
                            $"{partidaGlobal.Orden}-{partidaGlobal.Numero}-{partidaGlobal.Tipo.Trim()}-{partidaGlobal.Fecha.Trim()}-{partidaGlobal.Sucursal}-{partidaGlobal.Turno}",
                            70);
                        await conn.ExecuteAsync(
                            sqlInsertCfdiDetalle,
                            new { IdCfdi = idCfdiNc, Sec = sec, IdOrdenes = idOrdenes },
                            tx);
                    }
                }

                string idCfdiFactura = ConstruirIdCfdi(firmado.Factura.Serie, firmado.Factura.Folio);
                await conn.ExecuteAsync(sqlInsertCfdiFactura, ConstruirParametrosCfdi(idCfdiFactura, firmado.Factura), tx);

                const string sqlActualizarOrden = @"UPDATE Ordenes
                                                    SET idCFDI = @IdCfdi,
                                                        uuid = ''
                                                    WHERE Sucursal = @Sucursal
                                                      AND Fecha = @Fecha
                                                      AND Orden = @Orden
                                                      AND Turno = @Turno
                                                      AND Numero = @Numero
                                                      AND Tipo = @Tipo
                                                      AND ISNULL(Baja, '') <> '*'";

                foreach (var partida in partidasDetalle)
                {
                    int actualizadas = await conn.ExecuteAsync(
                        sqlActualizarOrden,
                        new
                        {
                            IdCfdi = idCfdiFactura,
                            partida.Sucursal,
                            partida.Fecha,
                            partida.Orden,
                            partida.Turno,
                            partida.Numero,
                            partida.Tipo
                        },
                        tx);

                    if (actualizadas == 0)
                    {
                        tx.Rollback();
                        return new RegistrarFacturaResultado
                        {
                            Success = false,
                            Message = "No se encontró el ticket para marcarlo como facturado."
                        };
                    }
                }

                tx.Commit();

                return new RegistrarFacturaResultado
                {
                    Success = true,
                    Message = string.Empty,
                    IdCfdiFactura = idCfdiFactura,
                    FolioFactura = firmado.Factura.Folio,
                    IdCfdiNotaCredito = idCfdiNc,
                    FolioNotaCredito = folioNcFinal
                };
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private static string ConstruirIdCfdi(string serie, int folio)
        {
            return Recortar($"{serie.Trim()}{folio.ToString(CultureInfo.InvariantCulture)}", 30);
        }

        private static object ConstruirParametrosCfdi(string idCfdi, RegistroCfdiPersistenciaData data)
        {
            return new
            {
                IdCfdi = idCfdi,
                Serie = Recortar(data.Serie, 20),
                Folio = Convert.ToDouble(data.Folio),
                data.IdCliente,
                UsoCfdi = Recortar(data.UsoCfdi, 4),
                Moneda = Recortar(data.Moneda, 5),
                TipoDeComprobante = Recortar(data.TipoDeComprobante, 2),
                VersionCfdi = Recortar(data.VersionCfdi, 5),
                EsPrueba = Recortar(data.EsPrueba, 1),
                SubTotal = Convert.ToDouble(data.SubTotal),
                Total = Convert.ToDouble(data.Total),
                FormaPago = Recortar(data.FormaPago, 3),
                MetodoPago = Recortar(data.MetodoPago, 3),
                SatTipoRelacion = Recortar(data.SatTipoRelacion, 5),
                UuidRelacionados = data.UuidRelacionados ?? string.Empty,
                Fecha = Recortar(data.Fecha, 20),
                IvaImporte = Convert.ToDouble(data.IvaImporte),
                TipoCambio = Convert.ToDouble(data.TipoCambio),
                SatExportacion = Recortar(data.SatExportacion, 2),
                data.Orden,
                RfcReceptor = Recortar(data.RfcReceptor, 13),
                data.Sucursal,
                OrdenFecha = Recortar(data.OrdenFecha, 10),
                RelGlobal = Recortar(data.RelGlobal, 45),
                data.BaseIva16,
                data.BaseIva0,
                data.PagoTotalVale
            };
        }

        private static string Recortar(string? valor, int maxLength)
        {
            string limpio = (valor ?? string.Empty).Trim();
            return limpio.Length > maxLength ? limpio.Substring(0, maxLength) : limpio;
        }
    }
}

