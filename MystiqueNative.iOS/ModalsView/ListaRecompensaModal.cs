using Foundation;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ListaRecompensaModal : UIViewController
    {
        public string codigoQR { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ModalBackground.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

            if (codigoQR == null)
            {

            }
            else
            {
                qrImage.Image =
                QR(codigoQR, 500, 500, 4, ZXing.BarcodeFormat.QR_CODE);
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
        public ListaRecompensaModal (IntPtr handle) : base (handle)
        {
        }

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissModalViewController(true);
        }
    }
}