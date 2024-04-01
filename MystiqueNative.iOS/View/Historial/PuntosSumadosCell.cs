using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class PuntosSumadosCell : UITableViewCell
    {
        public string fecha { get; set; }
        public string puntos { get; set; }
        public nint tag { get; set; }
        private string noticket;
        private string montocompra;
        public string NoTicket { get => noticket; set { noticket = value; NoTicketLabel.Text = value; } }
        public string MontoCompra { get => montocompra; set { montocompra = value; MontoCompraLabel.Text = value; } }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.ViewStack.Tag = tag;
            PuntosLabel.AdjustsFontSizeToFitWidth = true;
        }
        public string Fecha { get => fecha; set { fecha = value; FechaLabel.Text = value; } }
        public string Puntos { get => puntos; set { puntos = value; PuntosLabel.Text = value; } }

        public PuntosSumadosCell(IntPtr handle) : base(handle)
        {
        }
    }
}