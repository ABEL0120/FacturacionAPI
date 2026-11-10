using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FacturacionAPI.Data
{
    public interface IDbConnectionData
    {
        IDbConnection CreateConnection();
        IDbConnection CreateServidoresConnection();
    }

    public class DbConnectionData : IDbConnectionData
    {
        private readonly string _connectionString;
        private readonly string _servidoresConnectionString;

        public DbConnectionData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Cadena de conexión 'SqlServer' no configurada.");

            _servidoresConnectionString = configuration.GetConnectionString("ServidoresBd")
                ?? _connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbConnection CreateServidoresConnection()
        {
            return new SqlConnection(_servidoresConnectionString);
        }
    }
}
