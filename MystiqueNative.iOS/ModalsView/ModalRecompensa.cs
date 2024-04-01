using Foundation;
using System;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace MystiqueNative.iOS
{
    public partial class ModalRecompensa : UIViewController
    {
         public string codigoQR { get; set; }
        public string NombreRecompensaD { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NombreRecompensa.Text = NombreRecompensaD;
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

            if (codigoQR == null)
            {
                DismissViewController(true, null);
            }
            else
            {
                qrImage.Image =
                QR(codigoQR, 600, 600, 4, ZXing.BarcodeFormat.QR_CODE);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

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
        public ModalRecompensa (IntPtr handle) : base (handle)
        {
        }

        partial void CloseModalButton_TouchUpInside(UIButton sender)
        {
            DismissViewController(true,null);
        }
        //public static UIStoryboard StoryboardRecompensas = UIStoryboard.FromName("Recompensas", null);

        public static UIStoryboard StoryboardMenu = UIStoryboard.FromName("Menu", null);
        public static UIViewController initialViewController;
        UIWindow window;
        //partial void IrARecompensas_TouchUpInside(UIButton sender)
        //{
        //    //RecompensasTableViewController ListaRecompensas = StoryboardRecompensas.InstantiateViewController("RECOMPENSASID") as RecompensasTableViewController;
        //    //window = new UIWindow(UIScreen.MainScreen.Bounds);
        //    //initialViewController = StoryboardMenu.InstantiateInitialViewController() as UIViewController;
        //    //window.RootViewController = initialViewController;
        //    //window.MakeKeyAndVisible();
        //    //DismissViewController(true, ()=> {
        //    //    this.NavigationController.PushViewController(ListaRecompensas, true);
        //    //});
        //    DismissModalViewController(true);
        //    PerformSegue("TO_RECOMPENSAS", this);
        //}
    }
}