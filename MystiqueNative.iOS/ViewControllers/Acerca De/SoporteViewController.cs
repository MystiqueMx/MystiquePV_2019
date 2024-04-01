using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class SoporteViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DescripcionLabel.Text = MystiqueApp.DescripcionSoporte;
            TelefonoLabel.Text = MystiqueApp.TelefonoSoporte;
            CorreoLabel.Text = MystiqueApp.EmailSoporte;
            TerminosLabel.Text = MystiqueApp.TerminosSoporte;

            TerminosLabel.AdjustsFontSizeToFitWidth = true;

            VersionLabel.Text = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString() 
                          +" ("+ NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString()+ ")";

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
          
        }
        public SoporteViewController (IntPtr handle) : base (handle)
        {
        }
    }
}