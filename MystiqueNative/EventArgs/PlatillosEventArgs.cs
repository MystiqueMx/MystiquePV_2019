using System;
using System.Collections.Generic;
using System.Text;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Orden;
using MystiqueNative.Models.Platillos;

namespace MystiqueNative.EventsArgs
{
    public class SubplatilloCompletado : BaseEventArgs
    {
        public SeleccionPlatilloMultiNivel Seleccion { get; set; }
    }

    public class SeleccionPlatilloMultiNivel
    {
        public KeyValuePair<int, string> IdNivel1 { get; set; }
        public KeyValuePair<int, string> IdNivel2 { get; set; }
        public KeyValuePair<int, string> IdNivel3 { get; set; }
    }

    public class PlatilloTerminadoEventArgs : BaseEventArgs
    {
        public PlatilloCarrito Platillo { get; set; }
        public int Posicion { get; set; }
    }
    public class CargarSubmenuEventArgs : BaseEventArgs
    {
        public List<BasePlatilloMultiNivel> Contenido { get; set; }
        public NivelesPlatillo Nivel { get; set; }
    }
    public class RegistrarPedidoEventArgs : BaseEventArgs
    {
        public int NumeroOrdenesActivas { get; set; }
        public TipoReparto TipoDeReparto { get; set; }
        public MetodoPago MetodoDePago { get; set; }
        public decimal Total { get; set; }
    }
    public class DetallePedidoEventArgs : BaseEventArgs
    {
        public Orden OrdenSeleccionada { get; set; }
    }
}
