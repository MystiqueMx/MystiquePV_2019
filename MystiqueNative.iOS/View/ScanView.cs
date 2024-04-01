using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ScanView : UIView
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

        }

        
        public ScanView (IntPtr handle) : base (handle)
        {
        }
    }
}