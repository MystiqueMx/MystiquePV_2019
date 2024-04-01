using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ShadowsView : UIStackView
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
           Layer.MasksToBounds = false;
           Layer.ShadowColor = UIColor.Black.CGColor;
           Layer.ShadowOffset = new CGSize(5.0, 20.0);
           Layer.ShadowOpacity = 0.5f;
           Layer.ShadowPath = new CGPath();
        }
        public ShadowsView (IntPtr handle) : base (handle)
        {
        }
    }
}