using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.DTOs;

namespace FacturacionAPI.Repositories
{
    public interface ICatalogosSatRepository
    {
        Task<List<CatalogoItemDto>> ObtenerRegimenesFiscalesAsync();
        Task<List<CatalogoItemDto>> ObtenerUsosCfdiAsync();
        Task<List<CatalogoItemDto>> ObtenerFormasPagoAsync();
        Task<List<MonedaCatalogoDto>> ObtenerMonedasAsync();
        Task<List<CatalogoItemDto>> ObtenerEstadosMexicoAsync();
        Task<CatalogoItemDto?> ObtenerRegimenFiscalPorNumeroAsync(string numero);
        Task<CatalogoItemDto?> ObtenerUsoCfdiPorNumeroAsync(string numero);
        Task<CatalogoItemDto?> ObtenerFormaPagoPorNumeroAsync(string numero);
        Task<MonedaCatalogoDto?> ObtenerMonedaPorNumeroAsync(string numero);
        Task<string?> ResolverNombreEstadoAsync(string estado);
    }

    public class CatalogosSatRepository : ICatalogosSatRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public CatalogosSatRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<List<CatalogoItemDto>> ObtenerRegimenesFiscalesAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(Ap_Fisica) AS ApFisica, RTRIM(Ap_Moral) AS ApMoral
                FROM SAT_REGIMEN_FISCAL WHERE ISNULL(Baja, '') <> '*' ORDER BY Numero";
            var res = await conn.QueryAsync<CatalogoItemDto>(sql);
            return res.ToList();
        }

        public async Task<List<CatalogoItemDto>> ObtenerUsosCfdiAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(Ap_Fisica) AS ApFisica, RTRIM(Ap_Moral) AS ApMoral
                FROM SAT_USO_CFDI WHERE ISNULL(Baja, '') <> '*' ORDER BY Numero";
            var res = await conn.QueryAsync<CatalogoItemDto>(sql);
            return res.ToList();
        }

        public async Task<List<CatalogoItemDto>> ObtenerFormasPagoAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion
                FROM SAT_FORMA_PAGO WHERE ISNULL(Baja, '') <> '*' ORDER BY Numero";
            var res = await conn.QueryAsync<CatalogoItemDto>(sql);
            return res.ToList();
        }

        public async Task<List<MonedaCatalogoDto>> ObtenerMonedasAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(ISNULL(Abreviatura, '')) AS Abreviatura, ISNULL(Decimales, 2) AS Decimales
                FROM SAT_MONEDA WHERE ISNULL(Baja, '') <> '*' ORDER BY Numero";
            var res = await conn.QueryAsync<MonedaCatalogoDto>(sql);
            return res.ToList();
        }

        public async Task<List<CatalogoItemDto>> ObtenerEstadosMexicoAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT RTRIM(Estado) AS Numero, RTRIM(Descripcion) AS Descripcion
                FROM Sat_Estado WHERE Pais = 'MEX' AND ISNULL(Baja, '') <> '*' ORDER BY Descripcion";
            var res = await conn.QueryAsync<CatalogoItemDto>(sql);
            return res.ToList();
        }

        public async Task<CatalogoItemDto?> ObtenerRegimenFiscalPorNumeroAsync(string numero)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(Ap_Fisica) AS ApFisica, RTRIM(Ap_Moral) AS ApMoral
                FROM SAT_REGIMEN_FISCAL WHERE Numero = @Numero";
            return await conn.QueryFirstOrDefaultAsync<CatalogoItemDto>(sql, new { Numero = numero.Trim() });
        }

        public async Task<CatalogoItemDto?> ObtenerUsoCfdiPorNumeroAsync(string numero)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(Ap_Fisica) AS ApFisica, RTRIM(Ap_Moral) AS ApMoral
                FROM SAT_USO_CFDI WHERE Numero = @Numero";
            return await conn.QueryFirstOrDefaultAsync<CatalogoItemDto>(sql, new { Numero = numero.Trim() });
        }

        public async Task<CatalogoItemDto?> ObtenerFormaPagoPorNumeroAsync(string numero)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion
                FROM SAT_FORMA_PAGO WHERE Numero = @Numero";
            return await conn.QueryFirstOrDefaultAsync<CatalogoItemDto>(sql, new { Numero = numero.Trim() });
        }

        public async Task<MonedaCatalogoDto?> ObtenerMonedaPorNumeroAsync(string numero)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(Numero) AS Numero, RTRIM(Descripcion) AS Descripcion, RTRIM(ISNULL(Abreviatura, '')) AS Abreviatura, ISNULL(Decimales, 2) AS Decimales
                FROM SAT_MONEDA WHERE Numero = @Numero";
            return await conn.QueryFirstOrDefaultAsync<MonedaCatalogoDto>(sql, new { Numero = numero.Trim() });
        }

        public async Task<string?> ResolverNombreEstadoAsync(string estado)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(Descripcion)
                FROM Sat_Estado WHERE Pais = 'MEX' AND (Estado = @Estado OR Descripcion = @Estado)";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { Estado = estado.Trim() });
        }
    }
}

