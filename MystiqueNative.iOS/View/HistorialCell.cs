using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class HistoriaCell : UITableViewCell
    {
        
        private string label;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            
     
        }
        public HistoriaCell (IntPtr handle) : base (handle)
        {
        }

     
    }
}