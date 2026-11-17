using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.DTOs;

namespace FacturacionAPI.Repositories
{
    public interface ICfdiConsultaRepository
    {
        Task<List<CfdiConsultaItemDto>> ConsultarCfdisAsync(int numeroCliente, string? serie, string? folio);
        Task<CfdiArchivoFacturaData?> ObtenerCfdiPorIdAsync(string idCfdi, int? numeroCliente = null);
    }

    public class CfdiConsultaRepository : ICfdiConsultaRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public CfdiConsultaRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<List<CfdiConsultaItemDto>> ConsultarCfdisAsync(int numeroCliente, string? serie, string? folio)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();

            string serieLimpia = (serie ?? string.Empty).Trim().ToUpperInvariant();
            string folioLimpio = (folio ?? string.Empty).Trim();

            string sql = @"SELECT RTRIM(ISNULL(a.idCFDI, '')) AS IdCfdi,
                    RTRIM(ISNULL(a.Serie, '')) AS Serie,
                    CAST(ISNULL(a.Folio, 0) AS bigint) AS Folio,
                    RTRIM(ISNULL(a.RfcReceptor, '')) AS RfcReceptor,
                    CAST(ISNULL(a.Sub_Total, 0) AS decimal(18,4)) AS SubTotal,
                    CAST(ISNULL(a.Iva_Importe, 0) AS decimal(18,4)) AS IvaImporte,
                    CAST(ISNULL(a.Total, 0) AS decimal(18,4)) AS Total,
                    RTRIM(ISNULL(a.Fecha_Timbre, '')) AS FechaTimbre,
                    RTRIM(ISNULL(a.Fecha, '')) AS Fecha,
                    CAST(ISNULL(a.Orden, 0) AS decimal(18,0)) AS Orden,
                    CAST(ISNULL(a.Sucursal, 0) AS bigint) AS Sucursal,
                    RTRIM(ISNULL(a.uuid, '')) AS Uuid,
                    RTRIM(ISNULL((SELECT TOP 1 s.Nombre FROM Sucursal s WHERE s.Numero = a.Sucursal), '')) AS SucursalDesc
                FROM CFDI_Factura a
                WHERE a.idCliente = @IdCliente
                  AND a.Estatus_CFDI = 0";

            var parameters = new DynamicParameters();
            parameters.Add("IdCliente", numeroCliente);

            if (!string.IsNullOrWhiteSpace(serieLimpia))
            {
                sql += " AND UPPER(RTRIM(ISNULL(a.Serie, ''))) LIKE @Serie";
                parameters.Add("Serie", $"%{serieLimpia}%");
            }

            if (!string.IsNullOrWhiteSpace(folioLimpio))
            {
                if (long.TryParse(folioLimpio, out long folioNum))
                {
                    sql += " AND CAST(ISNULL(a.Folio, 0) AS bigint) = @FolioNum";
                    parameters.Add("FolioNum", folioNum);
                }
                else
                {
                    sql += " AND UPPER(RTRIM(ISNULL(a.idCFDI, ''))) LIKE @FolioText";
                    parameters.Add("FolioText", $"%{folioLimpio.ToUpperInvariant()}%");
                }
            }

            sql += " ORDER BY a.Fecha_Timbre DESC, a.Folio DESC";

            var result = await conn.QueryAsync<CfdiConsultaItemDto>(sql, parameters);
            return result.ToList();
        }

        public async Task<CfdiArchivoFacturaData?> ObtenerCfdiPorIdAsync(string idCfdi, int? numeroCliente = null)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            string sql = @"SELECT TOP 1 RTRIM(ISNULL(idCFDI, '')) AS IdCfdi,
                    RTRIM(ISNULL(Serie, '')) AS Serie,
                    CAST(ISNULL(Folio, 0) AS bigint) AS Folio,
                    CAST(ISNULL(idCliente, 0) AS int) AS IdCliente,
                    RTRIM(ISNULL(RfcReceptor, '')) AS RfcReceptor,
                    RTRIM(ISNULL(uuid, '')) AS Uuid
                FROM CFDI_Factura
                WHERE idCFDI = @IdCfdi";

            if (numeroCliente.HasValue && numeroCliente.Value > 0)
            {
                sql += " AND idCliente = @NumeroCliente";
            }

            return await conn.QueryFirstOrDefaultAsync<CfdiArchivoFacturaData>(sql, new
            {
                IdCfdi = (idCfdi ?? string.Empty).Trim(),
                NumeroCliente = numeroCliente ?? 0
            });
        }
    }
}
