using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MystiqueNative.Helpers
{
    public static class CiudadesHelper
    {
        public static List<string> Ciudades { get; } = new List<string> (){ "Seleccionar ciudad", "Mexicali", "Tijuana" };
        public static Dictionary<string, int> CiudadesToInt { get; } = new Dictionary<string, int>() { { "Seleccionar ciudad", 0 }, { "Mexicali", 1 }, { "Tijuana", 2 }, };
        public static Dictionary<int, string> IntToCiudades { get; } = new Dictionary<int, string>() { { 0, "Seleccionar ciudad" }, { 1, "Mexicali" }, { 2, "Tijuana" }, };
    }
}
