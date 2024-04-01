using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MenuIcons : UIImageView
    {
        public override void DrawRect(CGRect area, UIViewPrintFormatter formatter)
        {
            base.DrawRect(area, formatter);

            
            TintColor = UIColor.FromRGB(red: 128, green: 189, blue: 1);
        }
        public MenuIcons (IntPtr handle) : base (handle)
        {
        }
    }
}