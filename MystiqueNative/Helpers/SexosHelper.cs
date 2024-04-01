using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Helpers
{
    public static class SexosHelper
    {
        public static List<string> Genders { get; } = new List<string>() {"Seleccionar sexo", "Masculino", "Femenino"};

        public static Dictionary<string, int> GenderToInt { get; } =
            new Dictionary<string, int>() {{"SEXO (OPCIONAL)", 0}, {"Femenino", 1}, {"Masculino", 2},};

        public static Dictionary<int, string> IntToGender { get; } =
            new Dictionary<int, string>() {{0, "SEXO (OPCIONAL)"}, {1, "Femenino"}, {2, "Masculino"},};
    }
}
