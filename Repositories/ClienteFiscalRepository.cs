using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IClienteFiscalRepository
    {
        Task<List<ClienteRfcItemDto>> ObtenerRfcsPorClienteAsync(int numeroCliente);
        Task<Cliente?> ObtenerPorNumeroClienteYRfcAsync(int numeroCliente, string rfc);
        Task<Cliente?> ObtenerPorRfcGeneralAsync(string rfc);
        Task<string?> ObtenerMailAccesoAsync(int numeroCliente);
        Task<bool> ExisteClienteRfcAsync(int numeroCliente, string rfc);
        Task<bool> RegistrarRfcAsync(Cliente cliente);
        Task<bool> ActualizarDatosFiscalesAsync(Cliente cliente);
    }

    public class ClienteFiscalRepository : IClienteFiscalRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public ClienteFiscalRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<List<ClienteRfcItemDto>> ObtenerRfcsPorClienteAsync(int numeroCliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT Numero_Cliente AS NumeroCliente, RTRIM(RFC) AS RFC, RTRIM(ISNULL(Razon_Social, '')) AS RazonSocial
                FROM Clientes WHERE Numero_Cliente = @NumeroCliente AND ISNULL(Baja, '') <> '*' ORDER BY Razon_Social";
            var res = await conn.QueryAsync<ClienteRfcItemDto>(sql, new { NumeroCliente = numeroCliente });
            return res.ToList();
        }

        public async Task<Cliente?> ObtenerPorNumeroClienteYRfcAsync(int numeroCliente, string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 Numero_Cliente, RTRIM(ISNULL(Razon_Social, '')) AS Razon_Social, RTRIM(RFC) AS RFC, RTRIM(ISNULL(Direccion, '')) AS Direccion,
                    RTRIM(COALESCE(NULLIF(Numero_Ext, ''), NULLIF(Num_Ext, ''), '')) AS Numero_Ext, RTRIM(COALESCE(NULLIF(Numero_Int, ''), NULLIF(Num_Int, ''), '')) AS Numero_Int,
                    RTRIM(ISNULL(Colonia, '')) AS Colonia, RTRIM(ISNULL(Ciudad, '')) AS Ciudad, RTRIM(ISNULL(Delegacion, '')) AS Delegacion, RTRIM(ISNULL(Estado, '')) AS Estado,
                    RTRIM(COALESCE(NULLIF(Codigo_Postal, ''), NULLIF(CP, ''), '')) AS Codigo_Postal, RTRIM(ISNULL(Telefono, '')) AS Telefono, RTRIM(ISNULL(Mail, '')) AS Mail,
                    RTRIM(ISNULL(SAT_RegimenFiscal, '')) AS SAT_RegimenFiscal, RTRIM(ISNULL(SAT_FORMA_PAGO, '')) AS SAT_FORMA_PAGO, RTRIM(ISNULL(Sat_Uso_Cfdi, '')) AS Sat_Uso_Cfdi, RTRIM(ISNULL(ValidadoReceptor, '')) AS ValidadoReceptor
                FROM Clientes WHERE Numero_Cliente = @NumeroCliente AND RFC = @RFC";
            return await conn.QueryFirstOrDefaultAsync<Cliente>(sql, new
            {
                NumeroCliente = numeroCliente,
                RFC = rfc.Trim().ToUpperInvariant()
            });
        }

        public async Task<Cliente?> ObtenerPorRfcGeneralAsync(string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 Numero_Cliente, RTRIM(ISNULL(Razon_Social, '')) AS Razon_Social, RTRIM(RFC) AS RFC, RTRIM(ISNULL(Direccion, '')) AS Direccion,
                    RTRIM(COALESCE(NULLIF(Numero_Ext, ''), NULLIF(Num_Ext, ''), '')) AS Numero_Ext, RTRIM(COALESCE(NULLIF(Numero_Int, ''), NULLIF(Num_Int, ''), '')) AS Numero_Int,
                    RTRIM(ISNULL(Colonia, '')) AS Colonia, RTRIM(ISNULL(Ciudad, '')) AS Ciudad, RTRIM(ISNULL(Delegacion, '')) AS Delegacion, RTRIM(ISNULL(Estado, '')) AS Estado,
                    RTRIM(COALESCE(NULLIF(Codigo_Postal, ''), NULLIF(CP, ''), '')) AS Codigo_Postal, RTRIM(ISNULL(Telefono, '')) AS Telefono, RTRIM(ISNULL(Mail, '')) AS Mail,
                    RTRIM(ISNULL(SAT_RegimenFiscal, '')) AS SAT_RegimenFiscal, RTRIM(ISNULL(SAT_FORMA_PAGO, '')) AS SAT_FORMA_PAGO, RTRIM(ISNULL(Sat_Uso_Cfdi, '')) AS Sat_Uso_Cfdi, RTRIM(ISNULL(ValidadoReceptor, '')) AS ValidadoReceptor
                FROM Clientes WHERE RFC = @RFC ORDER BY CASE WHEN Numero_Cliente > 0 THEN 0 ELSE 1 END, Numero_Cliente DESC";
            return await conn.QueryFirstOrDefaultAsync<Cliente>(sql, new { RFC = rfc.Trim().ToUpperInvariant() });
        }

        public async Task<string?> ObtenerMailAccesoAsync(int numeroCliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 RTRIM(ISNULL(Mail, ''))
                FROM Cliente_Acceso WHERE Numero_Cliente = @NumeroCliente";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { NumeroCliente = numeroCliente });
        }

        public async Task<bool> ExisteClienteRfcAsync(int numeroCliente, string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT COUNT(1)
                FROM Clientes WHERE Numero_Cliente = @NumeroCliente AND RFC = @RFC";
            int count = await conn.ExecuteScalarAsync<int>(sql, new
            {
                NumeroCliente = numeroCliente,
                RFC = rfc.Trim().ToUpperInvariant()
            });
            return count > 0;
        }

        public async Task<bool> RegistrarRfcAsync(Cliente cliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            string numExtCorto = Recortar(cliente.Numero_Ext, 20);
            string numIntCorto = Recortar(cliente.Numero_Int, 20);
            string cpCorto = Recortar(cliente.Codigo_Postal, 6);

            const string sql = @"INSERT INTO Clientes (Numero_Cliente, Razon_Social, RFC, Direccion, Numero_Ext, Numero_Int, Num_Ext, Num_Int, Colonia, Ciudad,
                    Delegacion, Estado, Codigo_Postal, CP, Telefono, Mail, SAT_RegimenFiscal, SAT_FORMA_PAGO, Sat_Uso_Cfdi, ValidadoReceptor)
                VALUES (@Numero_Cliente, @Razon_Social, @RFC, @Direccion, @Numero_Ext, @Numero_Int, @NumExtCorto, @NumIntCorto, @Colonia, @Ciudad,
                    @Delegacion, @Estado, @Codigo_Postal, @CpCorto, @Telefono, @Mail, @SAT_RegimenFiscal, @SAT_FORMA_PAGO, @Sat_Uso_Cfdi, @ValidadoReceptor)";
            int rows = await conn.ExecuteAsync(sql, new
            {
                cliente.Numero_Cliente,
                cliente.Razon_Social,
                cliente.RFC,
                cliente.Direccion,
                cliente.Numero_Ext,
                cliente.Numero_Int,
                NumExtCorto = numExtCorto,
                NumIntCorto = numIntCorto,
                cliente.Colonia,
                cliente.Ciudad,
                cliente.Delegacion,
                cliente.Estado,
                cliente.Codigo_Postal,
                CpCorto = cpCorto,
                cliente.Telefono,
                cliente.Mail,
                cliente.SAT_RegimenFiscal,
                cliente.SAT_FORMA_PAGO,
                cliente.Sat_Uso_Cfdi,
                cliente.ValidadoReceptor
            });
            return rows > 0;
        }

        public async Task<bool> ActualizarDatosFiscalesAsync(Cliente cliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            string numExtCorto = Recortar(cliente.Numero_Ext, 20);
            string numIntCorto = Recortar(cliente.Numero_Int, 20);
            string cpCorto = Recortar(cliente.Codigo_Postal, 6);

            const string sql = @"UPDATE Clientes SET Razon_Social = @Razon_Social, Direccion = @Direccion, Numero_Ext = @Numero_Ext, Numero_Int = @Numero_Int,
                    Num_Ext = @NumExtCorto, Num_Int = @NumIntCorto, Colonia = @Colonia, Codigo_Postal = @Codigo_Postal, CP = @CpCorto,
                    Estado = @Estado, Delegacion = @Delegacion, Ciudad = @Ciudad, Sat_Uso_Cfdi = @Sat_Uso_Cfdi, SAT_RegimenFiscal = @SAT_RegimenFiscal, ValidadoReceptor = @ValidadoReceptor
                WHERE Numero_Cliente = @Numero_Cliente AND RFC = @RFC";
            int rows = await conn.ExecuteAsync(sql, new
            {
                cliente.Numero_Cliente,
                cliente.Razon_Social,
                cliente.RFC,
                cliente.Direccion,
                cliente.Numero_Ext,
                cliente.Numero_Int,
                NumExtCorto = numExtCorto,
                NumIntCorto = numIntCorto,
                cliente.Colonia,
                cliente.Codigo_Postal,
                CpCorto = cpCorto,
                cliente.Estado,
                cliente.Delegacion,
                cliente.Ciudad,
                cliente.Sat_Uso_Cfdi,
                cliente.SAT_RegimenFiscal,
                cliente.ValidadoReceptor
            });
            return rows > 0;
        }

        private static string Recortar(string valor, int maxLength)
        {
            string limpio = (valor ?? string.Empty).Trim();
            return limpio.Length > maxLength ? limpio.Substring(0, maxLength) : limpio;
        }
    }
}

