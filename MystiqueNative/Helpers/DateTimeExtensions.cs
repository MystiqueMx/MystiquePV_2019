using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Helpers
{
    public static class DateTimeExtensions
    {
        //public static string ToFormatoEspanyol(this DateTime date)
        //{

        //}
        public static string MakePrettyDate(this DateTime then)
        {
            var seconds =(long) (DateTime.Now - then).TotalSeconds / 1000;
            var minutes = seconds / 60;
            var hours = minutes / 60;
            var days = hours / 24;

            string friendly = null;
            long num = 0;
            if (days > 0)
            {
                num = days;
                friendly = days + " dias";
            }
            else if (hours > 0)
            {
                num = hours;
                friendly = hours + " horas";
            }
            else if (minutes > 0)
            {
                num = minutes;
                friendly = minutes + " minutos";
            }
            else
            {
                num = seconds;
                friendly = seconds + " segundos";
            }
            if (num > 1)
            {
                friendly += "s";
            }
            return friendly;
        }
        public static string MakeWalletDate(int dias, int horas, int minutos)
        {
            var diasAsString = string.Empty;
            if (dias > 0)
            {
                diasAsString = dias + " dias, ";
            }
            var horasAsString = horas < 10 ? "0" + horas : horas.ToString();
            var minutosAsString = minutos < 10 ? "0" + minutos : minutos.ToString();
            return $"{diasAsString}{horasAsString}:{minutosAsString} hrs";
        }
    }
}
