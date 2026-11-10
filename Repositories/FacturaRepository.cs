using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IFacturaRepository
    {
        Task<IEnumerable<CfdiFactura>> ConsultarFacturasClienteAsync(int idCliente);
        Task<CfdiFactura?> ObtenerFacturaPorIdAsync(string idCfdi);
        Task<CfdiFactura?> ObtenerFacturaPorUuidAsync(string uuid);
        Task<bool> ActualizarEstatusCancelacionAsync(string uuid, int estatusCfdi, string estatusSat);
    }

    public class FacturaRepository : IFacturaRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public FacturaRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<IEnumerable<CfdiFactura>> ConsultarFacturasClienteAsync(int idCliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT a.idCFDI, a.Serie, a.Folio, a.RfcReceptor, a.idCliente, a.Sub_Total, a.Iva_Importe, a.Total, a.Fecha_Timbre, a.Orden, a.Sucursal, a.uuid,
                                        ISNULL((SELECT Nombre FROM Sucursal WHERE Numero = a.Sucursal), '') AS SucursalDesc,
                                        a.Estatus_CFDI, a.Estatus_SAT, a.TipoDeComprobante
                                 FROM CFDI_Factura a
                                 WHERE a.idCliente = @IdCliente AND a.Estatus_CFDI = 0
                                 ORDER BY a.Fecha_Timbre DESC";
            return await conn.QueryAsync<CfdiFactura>(sql, new { IdCliente = idCliente });
        }

        public async Task<CfdiFactura?> ObtenerFacturaPorIdAsync(string idCfdi)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 a.*, ISNULL((SELECT Nombre FROM Sucursal WHERE Numero = a.Sucursal), '') AS SucursalDesc
                                 FROM CFDI_Factura a
                                 WHERE a.idCFDI = @IdCFDI";
            return await conn.QueryFirstOrDefaultAsync<CfdiFactura>(sql, new { IdCFDI = idCfdi.Trim() });
        }

        public async Task<CfdiFactura?> ObtenerFacturaPorUuidAsync(string uuid)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 a.*, ISNULL((SELECT Nombre FROM Sucursal WHERE Numero = a.Sucursal), '') AS SucursalDesc
                                 FROM CFDI_Factura a
                                 WHERE a.uuid = @Uuid";
            return await conn.QueryFirstOrDefaultAsync<CfdiFactura>(sql, new { Uuid = uuid.Trim() });
        }

        public async Task<bool> ActualizarEstatusCancelacionAsync(string uuid, int estatusCfdi, string estatusSat)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"UPDATE CFDI_Factura
                                 SET Estatus_CFDI = @EstatusCfdi,
                                     Estatus_SAT = @EstatusSat
                                 WHERE uuid = @Uuid";
            int affected = await conn.ExecuteAsync(sql, new { EstatusCfdi = estatusCfdi, EstatusSat = estatusSat, Uuid = uuid.Trim() });
            return affected > 0;
        }
    }
}
