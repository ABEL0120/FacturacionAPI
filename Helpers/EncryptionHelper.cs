using System;
using System.Text;

namespace FacturacionAPI.Helpers
{
    public static class EncryptionHelper
    {
        private const string ClaveDefecto = "BFMAFB";

        public static string Encriptar(string texto, string clave = ClaveDefecto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            var resultado = new StringBuilder();
            for (int i = 0; i < texto.Length; i++)
            {
                char car = texto[i];
                char codigo = clave[i % clave.Length];
                int xorVal = (int)codigo ^ (int)car;
                resultado.Append(xorVal.ToString("X2"));
            }
            return resultado.ToString();
        }

        public static string Desencriptar(string pass, string clave = ClaveDefecto)
        {
            if (string.IsNullOrEmpty(pass))
                return string.Empty;

            var resultado = new StringBuilder();
            int j = 0;
            for (int i = 0; i < pass.Length; i += 2)
            {
                if (i + 1 >= pass.Length) break;
                string hexPair = pass.Substring(i, 2);
                int byteVal = Convert.ToInt32(hexPair, 16);
                char codigo = clave[j % clave.Length];
                resultado.Append((char)((int)codigo ^ byteVal));
                j++;
            }
            return resultado.ToString();
        }
    }
}

