using CoreGraphics;
using Foundation;
using System;
using UIKit;
using System.Resources;

namespace MystiqueNative.iOS
{
    public partial class SucursalesCell : UITableViewCell
    {
        private string label;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
          
           if (UIScreen.MainScreen.Bounds.Size.Height == 568)
            {
                labelSucursal.Font = UIFont.BoldSystemFontOfSize(23);
                Console.WriteLine("iPhone 5s");
               
            }

           
            labelSucursal.AdjustsFontSizeToFitWidth = true;
        }

        //public string DetectDevice()
        //{
        //    switch (UIScreen.MainScreen.NativeBounds.Height)
        //    {
        //        case 960:
        //            return "iPhone4";
        //        case 1136:
        //            return "iPhone5";
        //        case 1334:
        //            return "iPhone6";
        //        case 1920: //fallthrough
        //            return "iPhone6Plus";
        //        case 2208:
        //            return "iPhone6Plus";
        //        default:
        //            return "Desconocido";


        //    }
        //}
      

        public string Label { get => label; set { label = value; labelSucursal.Text = value; } }
        public SucursalesCell (IntPtr handle) : base (handle)
        {
        }
    }
}