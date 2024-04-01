using Foundation;
using System;
using UIKit;
using System.IO;
using ZXing.Mobile;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;


namespace MystiqueNative.iOS
{
    public partial class ModalQR : UIViewController
    {

        public string codigoQR { get; set; }
        public string codigoBARCODE { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //var x = codigoQR;

            if (codigoBARCODE == null)
            {

            }
            else
            {
                qrImage.Image =
                QR(codigoBARCODE, 200, 100, 1, ZXing.BarcodeFormat.CODE_128);
            }


            if (codigoQR == null)
            {

            }
            else
            {
                qrImage.Image =
                QR(codigoQR, 500, 500, 3, ZXing.BarcodeFormat.QR_CODE);
            }

            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

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


        public ModalQR(IntPtr handle) : base(handle)
        {
        }

        public ModalQR()
        {
        }

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissModalViewController(true);
        }


    }
}