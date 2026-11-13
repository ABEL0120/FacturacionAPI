using System.Data;
using System.Threading.Tasks;
using Dapper;
using FacturacionAPI.Data;
using FacturacionAPI.DTOs;
using FacturacionAPI.Models;

namespace FacturacionAPI.Repositories
{
    public interface IClienteAccesoRepository
    {
        Task<ClienteAcceso?> ObtenerPorMailOUsuarioAsync(string usuario);
        Task<bool> ExisteMailAsync(string mail);
        Task<bool> ExisteRfcAsync(string rfc);
        Task<int> ObtenerSiguienteNumeroClienteAsync();
        Task<bool> RegistrarClienteConAccesoAsync(ClienteAcceso acceso, Cliente cliente);
        Task<CuentaRecuperacionValidaDto?> ValidarCuentaPorRfcYMailAsync(string rfc, string mail);
        Task<bool> ActualizarContrasenaPorNumeroClienteAsync(int numeroCliente, string nuevaContrasena);
        Task<bool> ActualizarContrasenaPorMailAsync(string mail, string nuevaContrasena);
    }

    public class ClienteAccesoRepository : IClienteAccesoRepository
    {
        private readonly IDbConnectionData _dbConnectionData;

        public ClienteAccesoRepository(IDbConnectionData dbConnectionData)
        {
            _dbConnectionData = dbConnectionData;
        }

        public async Task<ClienteAcceso?> ObtenerPorMailOUsuarioAsync(string usuario)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 Numero_Cliente, Razon_Social, Mail, Contraseña, Telefono
                FROM Cliente_Acceso WHERE (Mail = @Usuario OR Telefono = @Usuario) AND Contraseña <> ''";
            return await conn.QueryFirstOrDefaultAsync<ClienteAcceso>(sql, new { Usuario = usuario.Trim() });
        }

        public async Task<bool> ExisteMailAsync(string mail)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Cliente_Acceso WHERE Mail = @Mail AND Mail <> ''";
            int total = await conn.ExecuteScalarAsync<int>(sql, new { Mail = mail.Trim() });
            return total > 0;
        }

        public async Task<bool> ExisteRfcAsync(string rfc)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Clientes WHERE RFC = @RFC";
            int total = await conn.ExecuteScalarAsync<int>(sql, new { RFC = rfc.Trim().ToUpperInvariant() });
            return total > 0;
        }

        public async Task<int> ObtenerSiguienteNumeroClienteAsync()
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = "SELECT ISNULL(MAX(Numero_Cliente), 0) + 1 FROM Clientes";
            return await conn.ExecuteScalarAsync<int>(sql);
        }

        public async Task<bool> RegistrarClienteConAccesoAsync(ClienteAcceso acceso, Cliente cliente)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                const string sqlAcceso = @"INSERT INTO Cliente_Acceso (Numero_Cliente, Razon_Social, Mail, Contraseña, Telefono)
                    VALUES (@Numero_Cliente, @Razon_Social, @Mail, @Contraseña, @Telefono)";
                await conn.ExecuteAsync(sqlAcceso, acceso, transaction);

                const string sqlCliente = @"INSERT INTO Clientes (Numero_Cliente, Razon_Social, RFC, Direccion, Numero_Ext, Numero_Int, Colonia, Ciudad, Delegacion,
                        Estado, Codigo_Postal, CP, Telefono, Contraseña, SAT_RegimenFiscal, SAT_FORMA_PAGO, Sat_Uso_Cfdi, ValidadoReceptor)
                    VALUES (@Numero_Cliente, @Razon_Social, @RFC, @Direccion, @Numero_Ext, @Numero_Int, @Colonia, @Ciudad, @Delegacion,
                        @Estado, @Codigo_Postal, @Codigo_Postal, @Telefono, @Contraseña, @SAT_RegimenFiscal, @SAT_FORMA_PAGO, @Sat_Uso_Cfdi, @ValidadoReceptor)";
                await conn.ExecuteAsync(sqlCliente, new
                {
                    cliente.Numero_Cliente,
                    cliente.Razon_Social,
                    cliente.RFC,
                    cliente.Direccion,
                    cliente.Numero_Ext,
                    cliente.Numero_Int,
                    cliente.Colonia,
                    cliente.Ciudad,
                    cliente.Delegacion,
                    cliente.Estado,
                    cliente.Codigo_Postal,
                    cliente.Telefono,
                    Contraseña = acceso.Contraseña,
                    cliente.SAT_RegimenFiscal,
                    cliente.SAT_FORMA_PAGO,
                    cliente.Sat_Uso_Cfdi,
                    cliente.ValidadoReceptor
                }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<CuentaRecuperacionValidaDto?> ValidarCuentaPorRfcYMailAsync(string rfc, string mail)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            const string sql = @"SELECT TOP 1 a.Numero_Cliente AS NumeroCliente, a.RFC, b.Mail, a.Razon_Social AS RazonSocial
                FROM Clientes a INNER JOIN Cliente_Acceso b ON a.Numero_Cliente = b.Numero_Cliente
                WHERE a.Numero_Cliente > 0 AND b.Mail = @Mail AND a.RFC = @RFC";
            return await conn.QueryFirstOrDefaultAsync<CuentaRecuperacionValidaDto>(sql, new
            {
                RFC = rfc.Trim().ToUpperInvariant(),
                Mail = mail.Trim()
            });
        }

        public async Task<bool> ActualizarContrasenaPorNumeroClienteAsync(int numeroCliente, string nuevaContrasena)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                const string sqlAcceso = "UPDATE Cliente_Acceso SET Contraseña = @Contraseña WHERE Numero_Cliente = @NumeroCliente";
                await conn.ExecuteAsync(sqlAcceso, new { Contraseña = nuevaContrasena, NumeroCliente = numeroCliente }, transaction);

                const string sqlCliente = "UPDATE Clientes SET Contraseña = @Contraseña WHERE Numero_Cliente = @NumeroCliente";
                await conn.ExecuteAsync(sqlCliente, new { Contraseña = nuevaContrasena, NumeroCliente = numeroCliente }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> ActualizarContrasenaPorMailAsync(string mail, string nuevaContrasena)
        {
            using IDbConnection conn = _dbConnectionData.CreateConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            using IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                const string sqlAcceso = "UPDATE Cliente_Acceso SET Contraseña = @Contraseña WHERE Mail = @Mail";
                int rows = await conn.ExecuteAsync(sqlAcceso, new { Contraseña = nuevaContrasena, Mail = mail.Trim() }, transaction);

                const string sqlCliente = "UPDATE Clientes SET Contraseña = @Contraseña WHERE Mail = @Mail";
                await conn.ExecuteAsync(sqlCliente, new { Contraseña = nuevaContrasena, Mail = mail.Trim() }, transaction);

                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
