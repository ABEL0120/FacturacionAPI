using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FacturacionAPI.Data
{
    public interface IDbConnectionData
    {
        IDbConnection CreateConnection();
    }

    public class DbConnectionData : IDbConnectionData
    {
        private readonly string _connectionString;

        public DbConnectionData(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("Cadena de conexión 'SqlServer' no configurada.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
