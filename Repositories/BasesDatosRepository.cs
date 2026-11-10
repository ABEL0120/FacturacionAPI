using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IBasesDatosRepository
    {
        Task<IEnumerable<BaseDatosApp>> ObtenerBasesPorAppAsync(string idApp);
        Task<BaseDatosApp?> ObtenerPrimeraBasePorAppAsync(string idApp);
    }

    public class BasesDatosRepository : IBasesDatosRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public BasesDatosRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<IEnumerable<BaseDatosApp>> ObtenerBasesPorAppAsync(string idApp)
        {
            using IDbConnection conn = _dbConnectionData.CreateServidoresConnection();
            const string sql = "SELECT * FROM Bases_Datos_APPs WHERE idAPP = @IdApp ORDER BY Nombre ASC";
            return await conn.QueryAsync<BaseDatosApp>(sql, new { IdApp = idApp.Trim() });
        }

        public async Task<BaseDatosApp?> ObtenerPrimeraBasePorAppAsync(string idApp)
        {
            using IDbConnection conn = _dbConnectionData.CreateServidoresConnection();
            const string sql = "SELECT TOP 1 * FROM Bases_Datos_APPs WHERE idAPP = @IdApp";
            return await conn.QueryFirstOrDefaultAsync<BaseDatosApp>(sql, new { IdApp = idApp.Trim() });
        }
    }
}
