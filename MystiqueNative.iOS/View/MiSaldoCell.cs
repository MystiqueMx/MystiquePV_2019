using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MiSaldoCell : UITableViewCell
    {
        private string labelfecha;
        private string labelpuntos;
        private UIImage image;
        public MiSaldoCell (IntPtr handle) : base (handle)
        {
        }

        public string LabelFecha { get => labelfecha; set { labelfecha = value; FechaRegistro.Text = value; } }
        public string LabelPuntos { get => labelpuntos; set { labelpuntos = value; Puntos.Text = value; } }
        public UIImage Image { get => image; set { image = value; ImagenChevron.Image = value; } }
    }
}