using Foundation;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ModalMembresiaBAR : UIViewController
    {
        public string QRorBAR;
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
                if (string.IsNullOrEmpty(QRorBAR))
                {
                    qrImage.Image =
                    QR(AppDelegate.Auth.Usuario.GuidMembresia, 500, 400, 5, ZXing.BarcodeFormat.CODE_128);
                }
                else
                {
                    qrImage.Image =
                    QR(AppDelegate.Auth.Usuario.GuidMembresia, 500, 500, 3, ZXing.BarcodeFormat.QR_CODE);
                }
                
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
        public ModalMembresiaBAR (IntPtr handle) : base (handle)
        {
        }

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }
    }
}