using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Helpers.Facturacion.Helpers
{
    public static class NumaletExtensions
    {
        public static string ToCardinalString(this int n) => new Numalet { LetraCapital = true, Decimales = 2, }.ToCustomCardinal(n);
        public static string ToCardinalString(this float n) => new Numalet { LetraCapital = true, Decimales = 2, }.ToCustomCardinal(n);
        public static string ToCardinalString(this double n) => new Numalet { LetraCapital = true, Decimales = 2, }.ToCustomCardinal(n);
        public static string ToCardinalString(this decimal n) => new Numalet { LetraCapital = true, Decimales = 2,  }.ToCustomCardinal(n);
    }
}