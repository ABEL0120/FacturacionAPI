using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Orden>> ObtenerOrdenesTicketAsync(string folio, long transaccion);
        Task<IEnumerable<Orden>> ObtenerValidacionTicketAsync(string folio, long transaccion);
        Task<Compania?> ObtenerCompaniaAsync();
        Task<int> ObtenerSiguienteFolioFacturaAsync(string serie);
        Task<bool> RegistrarFacturaTicketsAsync(CfdiFactura factura, IEnumerable<Orden> ordenes);
    }

    public class TicketRepository : ITicketRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public TicketRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<IEnumerable<Orden>> ObtenerValidacionTicketAsync(string folio, long transaccion)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT ISNULL(c.EsGlobal, '') AS EsGlobal, a.*, a.Total AS Importe,
                                        b.Nombre AS Descripcion, b.ClaveProdServ_SAT, b.Unidad_Medida_SAT, b.Unidad_Medida, b.Exento,
                                        ROUND(a.Precio / (a.Porcentaje_Iva * 0.01 + 1), 6) AS PrecioSinIva, a.Subtotal, a.Iva AS ImpIva
                                 FROM Ordenes a
                                 LEFT JOIN Productos b ON a.Producto = b.Numero
                                 LEFT JOIN CFDI_Factura c ON a.uuid = c.uuid
                                 WHERE a.Folio = @Folio AND a.Transaccion = @Transaccion AND a.Producto > 0";
            return await conn.QueryAsync<Orden>(sql, new { Folio = folio.Trim(), Transaccion = transaccion });
        }

        public async Task<IEnumerable<Orden>> ObtenerOrdenesTicketAsync(string folio, long transaccion)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT c.EsGlobal, c.uuid AS uuidGlobal, a.*,
                                        b.Nombre AS Descripcion, b.ClaveProdServ_SAT, ISNULL(d.Descripcion, '') AS ClaveProdServDesc,
                                        b.Unidad_Medida_SAT, b.Unidad_Medida, b.Exento, a.Total AS Importe,
                                        ROUND(a.Precio / (a.Porcentaje_Iva * 0.01 + 1), 6) AS PrecioSinIva, a.Subtotal, a.Iva AS ImpIva,
                                        a.Pago_Total_Vale AS Descuento,
                                        (a.Subtotal + a.Iva - a.Pago_Total_Vale) AS TotalFacturar
                                 FROM Ordenes a
                                 LEFT JOIN Productos b ON a.Producto = b.Numero
                                 LEFT JOIN CFDI_Factura c ON a.idCFDI = c.idCFDI AND c.EsGlobal = 'S'
                                 LEFT JOIN SAT_CLAVE_PROD_SERV d ON b.ClaveProdServ_SAT = d.Numero
                                 WHERE a.Folio = @Folio AND a.Transaccion = @Transaccion
                                   AND (a.uuid = '' OR c.EsGlobal = 'S')
                                   AND a.Producto > 0
                                   AND a.Tipo <> 'Servicio'
                                   AND a.Baja <> '*'
                                 ORDER BY a.Numero";
            return await conn.QueryAsync<Orden>(sql, new { Folio = folio.Trim(), Transaccion = transaccion });
        }

        public async Task<Compania?> ObtenerCompaniaAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 a.*, ISNULL(b.Descripcion, '') AS RegimenFiscalDesc
                                 FROM Compañias a
                                 LEFT JOIN SAT_REGIMEN_FISCAL b ON a.CveRegimenFiscal = b.Numero";
            return await conn.QueryFirstOrDefaultAsync<Compania>(sql);
        }

        public async Task<int> ObtenerSiguienteFolioFacturaAsync(string serie)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT ISNULL(MAX(Folio), 0) + 1 FROM CFDI_Factura WHERE Serie = @Serie";
            return await conn.ExecuteScalarAsync<int>(sql, new { Serie = serie.Trim() });
        }

        public async Task<bool> RegistrarFacturaTicketsAsync(CfdiFactura factura, IEnumerable<Orden> ordenes)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                const string sqlFactura = @"INSERT INTO CFDI_Factura (
                                                idCFDI, Serie, Folio, idCliente, RfcReceptor, Sub_Total, Total,
                                                Iva_Importe, Estatus_CFDI, Fecha, uuid, Fecha_Timbre, Moneda,
                                                Metodo_Pago, Es_Prueba, TipoDeComprobante, Sucursal, Tipo_Documento
                                            ) VALUES (
                                                @idCFDI, @Serie, @Folio, @idCliente, @RfcReceptor, @Sub_Total, @Total,
                                                @Iva_Importe, @Estatus_CFDI, @Fecha, @uuid, @Fecha_Timbre, @Moneda,
                                                @Metodo_Pago, 'N', @TipoDeComprobante, @Sucursal, 1
                                            )";
                await conn.ExecuteAsync(sqlFactura, factura, transaction);

                const string sqlDetalle = @"INSERT INTO CFDI_Factura_Detalle (
                                                UUID, Folio, Sucursal, Tipo, Fecha_Timbrado, Fecha_Orden, Total, Pago_Total_Vale
                                            ) VALUES (
                                                @UUID, @Folio, @Sucursal, @Tipo, @Fecha_Timbrado, @Fecha_Orden, @Total, @Pago_Total_Vale
                                            )";

                const string sqlUpdateOrden = @"UPDATE Ordenes
                                                SET idCFDI = @idCFDI,
                                                    uuid = @uuid
                                                WHERE Sucursal = @Sucursal
                                                  AND Fecha = @Fecha
                                                  AND Orden = @Orden
                                                  AND Numero = @Numero";

                var foliosProcesados = new HashSet<string>();

                foreach (var ord in ordenes)
                {
                    if (foliosProcesados.Add(ord.Folio))
                    {
                        await conn.ExecuteAsync(sqlDetalle, new
                        {
                            UUID = factura.uuid,
                            Folio = ord.Folio,
                            Sucursal = ord.Sucursal,
                            Tipo = ord.Tipo,
                            Fecha_Timbrado = factura.Fecha_Timbre.Length >= 10 ? factura.Fecha_Timbre.Substring(0, 10) : factura.Fecha_Timbre,
                            Fecha_Orden = ord.Fecha.Length >= 10 ? ord.Fecha.Substring(0, 10) : ord.Fecha,
                            Total = ord.TotalFacturar > 0 ? ord.TotalFacturar : ord.Total,
                            Pago_Total_Vale = ord.Descuento
                        }, transaction);
                    }

                    await conn.ExecuteAsync(sqlUpdateOrden, new
                    {
                        idCFDI = factura.idCFDI,
                        uuid = factura.uuid,
                        Sucursal = ord.Sucursal,
                        Fecha = ord.Fecha,
                        Orden = ord.Numero,
                        Numero = ord.Numero
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
