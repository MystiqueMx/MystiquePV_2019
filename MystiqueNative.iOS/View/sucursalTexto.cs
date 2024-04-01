using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class sucursalTexto : UILabel
    {

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            AdjustsFontSizeToFitWidth = true;
        }
        public sucursalTexto (IntPtr handle) : base (handle)
        {
        }
    }
}