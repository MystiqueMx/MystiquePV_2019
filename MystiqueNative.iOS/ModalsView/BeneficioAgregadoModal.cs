using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class BeneficioAgregadoModal : UIViewController
    {
        public string Mensaje { get; set; }
        public new static UIStoryboard Storyboard = UIStoryboard.FromName("Menu", null);
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           VerWalletButton.Hidden = true; VerWalletButton.UserInteractionEnabled = false;
            fonditojeje.BackgroundColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 0.65f);

        }
        public BeneficioAgregadoModal(IntPtr handle) : base(handle)
        {
        }
  

        partial void CerrarModal_TouchUpInside(UIButton sender)
        {
            DismissViewController(true, null);
        }

        //partial void VerWalletButton_TouchUpInside(UIButton sender)
        //{
        //    //var storyboard = UIStoryboard.FromName("Menu", null);
        //    //var tab = storyboard.InstantiateViewController("WALLET_ID") as WalletViewController;
        //    WalletViewController tab = Storyboard.InstantiateViewController("WALLET_ID") as WalletViewController;

        //    this.NavigationController.PushViewController(tab, true);

        //    //NavigationController.PushViewController(wallet, true);
        //}

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
       
            if (segue.Identifier == "IR_WALLET")
            {
                var tabVC = segue.DestinationViewController as UITabBarController;
                tabVC.SelectedIndex = 3;
            }
        }

        partial void VerWalletButton_TouchUpInside(UIButton sender)
        {
            PerformSegue("IR_WALLET", this);
        }
    }
}