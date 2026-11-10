using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static FacturacionAPI.Helpers.ResponseHelper;

namespace FacturacionAPI.Services
{
    public interface ISatValidadorService
    {
        Task<ApiResponse<bool>> ValidarRfcSatAsync(string rfc);
    }

    public class SatValidadorService : ISatValidadorService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SatValidadorService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResponse<bool>> ValidarRfcSatAsync(string rfc)
        {
            string rfcLimpio = (rfc ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(rfcLimpio) || (rfcLimpio.Length != 12 && rfcLimpio.Length != 13))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "El RFC debe tener 12 o 13 caracteres.",
                    Data = false
                };
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                string url = $"https://acciones.comercio-digital.mx/validador/rfcValido?usr=AAA010101AAA&pwd=PWD&rfc={rfcLimpio}";
                string payload = $"usr=AAA010101AAA\npwd=PWD\nrfc={rfcLimpio}\n";
                using var content = new StringContent(payload, Encoding.UTF8, "text/plain");

                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"Error al consultar validador SAT: HTTP {(int)response.StatusCode}",
                        Data = false
                    };
                }

                string resultado = (await response.Content.ReadAsStringAsync()).Trim();

                if (resultado.Equals("SI", StringComparison.OrdinalIgnoreCase))
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "El RFC se encuentra registrado en el SAT.",
                        Data = true
                    };
                }

                if (resultado.Equals("NO", StringComparison.OrdinalIgnoreCase))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "El RFC no existe en las listas oficiales del SAT.",
                        Data = false
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = resultado,
                    Data = false
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Excepción al validar RFC en SAT: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
