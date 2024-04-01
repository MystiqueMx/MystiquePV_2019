using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class NotificationCell : UITableViewCell
    {
        private string label;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

           notificacionLabel.AdjustsFontSizeToFitWidth = true;

        }
        public NotificationCell(IntPtr handle) : base(handle)
        {
        }

        public string Label { get => label; set { label = value; notificacionLabel.Text = value; } }

    
    }
}