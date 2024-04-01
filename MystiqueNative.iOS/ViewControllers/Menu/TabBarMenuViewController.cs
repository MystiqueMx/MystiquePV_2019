using Foundation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class TabBarMenuViewController : UITabBarController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //TabBar.UnselectedItemTintColor = new UIColor(red: 0.90f, green: 0.90f, blue: 0.90f, alpha: 1.0f);
        }
        public TabBarMenuViewController (IntPtr handle) : base (handle)
        {
        }
    }
}