using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IFactTicketRepository
    {
        Task<Compania?> ObtenerCompaniaActivaAsync();
        Task<List<OrdenTicketDetalle>> ObtenerPartidasPorFoliosCandidatosAsync(List<string> foliosCandidatos, int? transaccion);
        Task<List<OrdenTicketDetalle>> ObtenerPartidasPorOrdenAsync(long orden, int? transaccion);
        Task<FormaPagoOrdenInfo?> ObtenerFormaPagoOrdenAsync(int sucursal, int orden, string fecha);
        Task<string?> ObtenerSerieFacturaSucursalAsync(int sucursal);
        Task<string?> VerificarFolioYaTimbradoIndividualAsync(string folio);
        Task<int> ObtenerSiguienteFolioPreviewAsync(string serie);
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
                                 LEFT JOIN CFDI_Factura c ON a.idCFDI = c.idCFDI
                                 WHERE a.Folio = @Folio
                                   AND ISNULL(a.idCFDI, '') <> ''
                                   AND ISNULL(c.EsGlobal, '') = ''";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Folio = folio });
        }

        public async Task<int> ObtenerSiguienteFolioPreviewAsync(string serie)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT CAST(ISNULL(MAX(Folio), 0) + 1 AS int) FROM CFDI_Factura WHERE Serie = @Serie";
            return await conn.ExecuteScalarAsync<int>(sql, new { Serie = serie });
        }
    }
}

