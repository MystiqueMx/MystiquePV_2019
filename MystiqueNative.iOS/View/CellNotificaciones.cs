using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class CellNotificaciones : UITableViewCell
    {

        private string label;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            TextoNotificacion.AdjustsFontSizeToFitWidth = true;

        }
        public string Label { get => label; set { label = value; TextoNotificacion.Text = value; } }
        public CellNotificaciones (IntPtr handle) : base (handle)
        {
        }
    }
}