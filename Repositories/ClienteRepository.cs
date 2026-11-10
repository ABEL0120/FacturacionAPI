using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObtenerPorIdAsync(int numeroCliente);
        Task<Cliente?> ObtenerPorRfcAsync(int numeroCliente, string rfc);
        Task<IEnumerable<Cliente>> ObtenerRfcsPorClienteAsync(int numeroCliente);
        Task<int> ObtenerSiguienteNumeroClienteAsync();
        Task<bool> ExisteClienteRfcAsync(int numeroCliente, string rfc);
        Task<bool> InsertarClienteAsync(Cliente cliente);
        Task<bool> ActualizarClienteAsync(Cliente cliente);
    }

    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public ClienteRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int numeroCliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT TOP 1 * FROM Clientes WHERE Numero_Cliente = @NumeroCliente";
            return await conn.QueryFirstOrDefaultAsync<Cliente>(sql, new { NumeroCliente = numeroCliente });
        }

        public async Task<Cliente?> ObtenerPorRfcAsync(int numeroCliente, string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT TOP 1 * FROM Clientes WHERE Numero_Cliente = @NumeroCliente AND RFC = @RFC";
            return await conn.QueryFirstOrDefaultAsync<Cliente>(sql, new { NumeroCliente = numeroCliente, RFC = rfc.Trim() });
        }

        public async Task<IEnumerable<Cliente>> ObtenerRfcsPorClienteAsync(int numeroCliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT * FROM Clientes WHERE Numero_Cliente = @NumeroCliente ORDER BY Razon_Social ASC";
            return await conn.QueryAsync<Cliente>(sql, new { NumeroCliente = numeroCliente });
        }

        public async Task<int> ObtenerSiguienteNumeroClienteAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT ISNULL(MAX(Numero_Cliente), 0) + 1 FROM Clientes";
            return await conn.ExecuteScalarAsync<int>(sql);
        }

        public async Task<bool> ExisteClienteRfcAsync(int numeroCliente, string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Clientes WHERE Numero_Cliente = @NumeroCliente AND RFC = @RFC";
            int total = await conn.ExecuteScalarAsync<int>(sql, new { NumeroCliente = numeroCliente, RFC = rfc.Trim() });
            return total > 0;
        }

        public async Task<bool> InsertarClienteAsync(Cliente cliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"INSERT INTO Clientes (
                                    Numero_Cliente, Razon_Social, RFC, Direccion, Numero_Ext, Numero_Int,
                                    Colonia, Ciudad, Delegacion, Estado, Codigo_Postal, Telefono, Mail,
                                    SAT_RegimenFiscal, SAT_FORMA_PAGO, Sat_Uso_Cfdi, ValidadoReceptor
                                 ) VALUES (
                                    @Numero_Cliente, @Razon_Social, @RFC, @Direccion, @Numero_Ext, @Numero_Int,
                                    @Colonia, @Ciudad, @Delegacion, @Estado, @Codigo_Postal, @Telefono, @Mail,
                                    @SAT_RegimenFiscal, @SAT_FORMA_PAGO, @Sat_Uso_Cfdi, @ValidadoReceptor
                                 )";
            int affected = await conn.ExecuteAsync(sql, cliente);
            return affected > 0;
        }

        public async Task<bool> ActualizarClienteAsync(Cliente cliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"UPDATE Clientes SET
                                    Razon_Social = @Razon_Social,
                                    Direccion = @Direccion,
                                    Numero_Ext = @Numero_Ext,
                                    Numero_Int = @Numero_Int,
                                    Colonia = @Colonia,
                                    Ciudad = @Ciudad,
                                    Delegacion = @Delegacion,
                                    Estado = @Estado,
                                    Codigo_Postal = @Codigo_Postal,
                                    Telefono = @Telefono,
                                    Mail = @Mail,
                                    SAT_RegimenFiscal = @SAT_RegimenFiscal,
                                    SAT_FORMA_PAGO = @SAT_FORMA_PAGO,
                                    Sat_Uso_Cfdi = @Sat_Uso_Cfdi
                                 WHERE Numero_Cliente = @Numero_Cliente AND RFC = @RFC";
            int affected = await conn.ExecuteAsync(sql, cliente);
            return affected > 0;
        }
    }
}
