using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Humanizer;

namespace MystiqueNative.Models.Menu
{
    public enum FiltroRestaurante
    {
        [Description("Ambos")]
        Todos = 0,

        [Description("Recoger en Sucursal")]
        Sucursal = 1,

        [Description("A Domicilio")]
        Domicilio = 2
    }

    public static class HelperFiltroRestaurante
    {
        public static readonly List<string> FiltrosRestaurante = new List<string>
        {
            FiltroRestaurante.Todos.Humanize(),
            FiltroRestaurante.Sucursal.Humanize(),
            FiltroRestaurante.Domicilio.Humanize()
        };
    }
    
}
