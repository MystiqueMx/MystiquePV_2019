using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Helpers
{
    public static class TipoComentariosHelper
    {
        public static List<string> Descripcion { get; } = new List<string>() {"Sugerencia", "Comentario", "Queja"};

        public static Dictionary<string, int> DescripcionToId { get; } =
            new Dictionary<string, int>() {{"Sugerencia", 1}, {"Comentario", 2}, {"Queja", 3},};
    }
}
