 using FFImageLoading;
using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ModalImagenBeneficio : UIViewController
    {
        public string URLImagenBeneficioCode { get; set; }
        public string DescripcionBeneficioString { get; set; }
   
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DescripcionBeneficio.Text = DescripcionBeneficioString;
            DescripcionBeneficio.AdjustsFontSizeToFitWidth = true;
            //float PI = (float)Math.PI;
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);
            //ModalView.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.0f);

            //if (UIScreen.MainScreen.Bounds.Size.Height < 568)
            //{
            //    //ImagenRecompensa.Frame = new CoreGraphics.CGRect(ImagenRecompensa.Frame.X, ImagenRecompensa.Frame.X,
            //    //                                                        ImagenRecompensa.Frame.Width,350);
            //    NSLayoutConstraint.Create(BottomQRImage, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, 1, 30).Active = true;


            //}

            //CGRect orig = DescripcionBeneficio.Frame;
            //DescripcionBeneficio.Transform = CGAffineTransform.MakeRotation(90*PI / 180);
            //DescripcionBeneficio.Text = DescripcionBeneficioString;
            //DescripcionBeneficio.Frame = orig;
            
            if (string.IsNullOrEmpty(
            URLImagenBeneficioCode))
            {
                DismissModalViewController(true);
            }
            else {

                qrImage.Image =
               QR(URLImagenBeneficioCode, 500, 500, 2, ZXing.BarcodeFormat.QR_CODE);

            }

        }

        public static UIImage QR(string data, int w, int h, int m, ZXing.BarcodeFormat format)
        {
           
            var gen = new ZXing.Mobile.BarcodeWriter
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
        public ModalImagenBeneficio (IntPtr handle) : base (handle)
        {
            
        }
      
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
         
        }

        public ModalImagenBeneficio()
        {
        }

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissModalViewController(true);
        }


    }
}