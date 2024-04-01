using Foundation;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ModalMembresiaQR : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

            if (string.IsNullOrEmpty(AppDelegate.Auth.Usuario.GuidMembresia))
            {
                DismissViewController(true, null);
            }
            else
            {
                qrImage.Image =
                QR(AppDelegate.Auth.Usuario.GuidMembresia, 300, 300, 3, ZXing.BarcodeFormat.QR_CODE);
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

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }
        public ModalMembresiaQR (IntPtr handle) : base (handle)
        {
        }
    }
}