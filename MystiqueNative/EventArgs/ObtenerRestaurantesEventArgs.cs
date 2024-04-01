using System.Collections.Generic;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Menu;

namespace MystiqueNative.EventsArgs
{
    public class ObtenerRestaurantesEventArgs : BaseEventArgs
    {
        public EstatusMenu EstatusRestaurantes { get; internal set; }
        public List<Restaurante> Restaurantes { get; set; }
        public string TiempoParaApertura { get; set; }
    }
}
