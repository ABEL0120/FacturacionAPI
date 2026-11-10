using System;
using System.Globalization;

namespace FacturacionAPI.Helpers
{
    public static class NumberToWordsHelper
    {
        public static string ConvertirNumeroALetras(decimal importe, string nombreMoneda = "PESOS", string abreviatura = "M.N.")
        {
            if (importe == 0 || Math.Abs(importe) < 0.01m)
            {
                return $"CERO {nombreMoneda} 00/100 {abreviatura}";
            }

            long parteEntera = (long)Math.Floor(importe);
            int parteDecimal = (int)Math.Round((importe - parteEntera) * 100);

            string textoEntero = ConvertirEnteroALetras(parteEntera);
            string centavos = parteDecimal.ToString("D2");

            return $"{textoEntero} {nombreMoneda} {centavos}/100 {abreviatura}".Trim();
        }

        private static string ConvertirEnteroALetras(long numero)
        {
            if (numero == 0) return "CERO";
            if (numero == 1) return "UN";

            if (numero < 0)
                return "MENOS " + ConvertirEnteroALetras(Math.Abs(numero));

            string resultado = "";

            if (numero >= 1000000)
            {
                long millones = numero / 1000000;
                numero %= 1000000;
                resultado += millones == 1 ? "UN MILLON " : ConvertirEnteroALetras(millones) + " MILLONES ";
            }

            if (numero >= 1000)
            {
                long miles = numero / 1000;
                numero %= 1000;
                resultado += miles == 1 ? "MIL " : ConvertirEnteroALetras(miles) + " MIL ";
            }

            if (numero >= 100)
            {
                long centenas = numero / 100;
                numero %= 100;

                string[] centenasArr = { "", numero == 0 ? "CIEN" : "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };
                resultado += centenasArr[centenas] + " ";
            }

            if (numero >= 20)
            {
                long decenas = numero / 10;
                long unidades = numero % 10;

                string[] decenasArr = { "", "", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
                if (decenas == 2 && unidades > 0)
                {
                    string[] veintiArr = { "", "VEINTIUNO", "VEINTIDOS", "VEINTITRES", "VEINTICUATRO", "VEINTICINCO", "VEINTISEIS", "VEINTISIETE", "VEINTIOCHO", "VEINTINUEVE" };
                    resultado += veintiArr[unidades] + " ";
                }
                else
                {
                    resultado += decenasArr[decenas];
                    if (unidades > 0)
                    {
                        string[] unidadesArr = { "", "Y UNO", "Y DOS", "Y TRES", "Y CUATRO", "Y CINCO", "Y SEIS", "Y SIETE", "Y OCHO", "Y NUEVE" };
                        resultado += " " + unidadesArr[unidades];
                    }
                    resultado += " ";
                }
            }
            else if (numero >= 10)
            {
                string[] diezA19 = { "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISEIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
                resultado += diezA19[numero - 10] + " ";
            }
            else if (numero > 0)
            {
                string[] unidadesArr = { "", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
                resultado += unidadesArr[numero] + " ";
            }

            return resultado.Trim();
        }
    }
}

