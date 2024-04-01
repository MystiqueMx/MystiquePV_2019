using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class UnderlineUITextfield : UITextField
    {
        public UnderlineUITextfield (IntPtr handle) : base (handle)
        {
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            var border = new CALayer();
            nfloat width = 1.5f;
            border.BorderColor = UIColor.FromRGB(200,200,200).CGColor;
            border.Frame = new CoreGraphics.CGRect(0,Frame.Size.Height - width,Frame.Size.Width,Frame.Size.Height);
            border.BorderWidth = width;
            Layer.AddSublayer(border);
            Layer.MasksToBounds = true;
        }
    }
}