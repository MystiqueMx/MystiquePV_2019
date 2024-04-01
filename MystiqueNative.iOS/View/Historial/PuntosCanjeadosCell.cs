using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class PuntosCanjeadosCell : UITableViewCell
    {
        public PuntosCanjeadosCell (IntPtr handle) : base (handle)
        {
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            PuntosLabel.AdjustsFontSizeToFitWidth = true;
        }
        public string fecha;
        public string puntos;
        public string producto;
        public string Producto { get => producto; set { producto = value; ProductoLabel.Text = value; } }
        public string Fecha { get => fecha; set { fecha = value; FechaLabel.Text = value; } }
        public string Puntos { get => puntos; set { puntos = value; PuntosLabel.Text = value; } }
    }
}