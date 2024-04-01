using Foundation;
using SidebarNavigation;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MenuViewController : UIViewController
    {
        public SidebarController SidebarController { get; private set; }
        public NavController NavController { get; private set; }
        public MenuViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var introController = (MenuViewController)Storyboard.InstantiateViewController("IntroController");
            var menuController = (LeftMenuViewController)Storyboard.InstantiateViewController("MenuController");

            NavController = new NavController();
            NavController.PushViewController(introController, false);
            SidebarController = new SidebarController(this,NavController, menuController);

            SidebarController.MenuWidth = (int)UIScreen.MainScreen.Bounds.Width - 50;
            SidebarController.ReopenOnRotate = false;
            SidebarController.MenuLocation = MenuLocations.Left;
        }
    }
}