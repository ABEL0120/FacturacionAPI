using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace FacturacionAPI.Helpers
{
    public static class BDHelper
    {
        public static async Task<int> ObtenerIdMax(SqlConnection conn, string tabla, string columna = "Numero")
        {
            if (string.IsNullOrWhiteSpace(tabla))
            {
                return 0;
            }
            string query = $"SELECT ISNULL(MAX({columna}), 0) FROM {tabla}";
            using var cmd = new SqlCommand(query, conn);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public static async Task<int> ObtenerIdMaxFiltro(SqlConnection conn, string tabla, string filtro, string columna = "Numero")
        {
            if (string.IsNullOrWhiteSpace(tabla) || string.IsNullOrWhiteSpace(filtro))
            {
                return 0;
            }
            string query = $"SELECT ISNULL(MAX({columna}), 0) FROM {tabla} WHERE {filtro}";
            using var cmd = new SqlCommand(query, conn);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        public static string FormatearFechaHora(DateTime fecha)
        {
            return fecha.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static string FormatearFecha(DateTime fecha)
        {
            return fecha.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }
    }
}

