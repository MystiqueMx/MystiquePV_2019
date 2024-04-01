using FFImageLoading;
using Foundation;
using MystiqueNative.Models;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ModalWallet : UIViewController
    {
        public string CodigoQRURL { get; set; }
        public BeneficiosSucursal Beneficios { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);
            //ImagenBeneficio
            if (!string.IsNullOrEmpty(CodigoQRURL))
            {
                ImagenBeneficio.Image =
                QR(CodigoQRURL, 300, 300, 3, ZXing.BarcodeFormat.QR_CODE);
                
            }
            else
            {
                DismissViewController(true, null);
            }
           
          
        }

        public static UIImage QR(string data, int w, int h, int m, ZXing.BarcodeFormat format)
        {
            var gen = new BarcodeWriter
            {
                Format = format,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = h,
                    Width = w,
                    Margin = m
                }
            };
            return gen.Write(data);
        }
        public ModalWallet (IntPtr handle) : base (handle)
        {
        }

        partial void CloseModal_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }
    }
}