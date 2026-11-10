using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface ICatalogosSatRepository
    {
        Task<IEnumerable<CatalogoItem>> ObtenerRegimenesFiscalesAsync();
        Task<IEnumerable<CatalogoItem>> ObtenerUsosCfdiAsync();
        Task<IEnumerable<CatalogoItem>> ObtenerFormasPagoAsync();
        Task<IEnumerable<CatalogoItem>> ObtenerEstadosAsync();
        Task<IEnumerable<CatalogoItem>> ObtenerMonedasAsync();
        Task<IEnumerable<CatalogoItem>> ObtenerMotivosCancelacionAsync();
    }

    public class CatalogosSatRepository : ICatalogosSatRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public CatalogosSatRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerRegimenesFiscalesAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Numero, Descripcion FROM SAT_REGIMEN_FISCAL ORDER BY Numero";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerUsosCfdiAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Numero, Descripcion FROM SAT_USO_CFDI ORDER BY Numero";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerFormasPagoAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Numero, Descripcion FROM SAT_FORMA_PAGO ORDER BY Numero";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerEstadosAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Estado AS Numero, Descripcion FROM Sat_Estado WHERE Pais = 'MEX' ORDER BY Descripcion";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerMonedasAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Numero, Descripcion FROM SAT_MONEDA ORDER BY Numero";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }

        public async Task<IEnumerable<CatalogoItem>> ObtenerMotivosCancelacionAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT Motivo AS Numero, Descripcion FROM SAT_MotivoCancelacion ORDER BY Motivo";
            return await conn.QueryAsync<CatalogoItem>(sql);
        }
    }
}
