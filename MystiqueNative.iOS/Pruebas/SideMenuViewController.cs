using Foundation;
using SidebarNavigation;
using System;
using UIKit;
using Xamarin.SideMenu;

namespace MystiqueNative.iOS
{
    public partial class SideMenuViewController : UIViewController
    {
        #region Constructor
        public SideMenuViewController(IntPtr handle) : base(handle)
        {
        }

        #endregion

        #region Declaraciones
        public SidebarController SidebarController { get; private set; }
        public NavController NavController { get; private set; }
        #endregion
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //_sideMenuManager = new SideMenuManager();

            //SetupSideMenu();
            //SetDefaults();

            var introController = (SideMenuViewController)Storyboard.InstantiateViewController("IntroController");
            var menuController = (MenuContentViewController)Storyboard.InstantiateViewController("MenuController");

            NavController = new NavController();
            NavController.PushViewController(introController, false);
            SidebarController = new SidebarController(this.NavigationController, NavController, menuController);

            SidebarController.MenuWidth = (int)UIScreen.MainScreen.Bounds.Width - 50;
            SidebarController.ReopenOnRotate = false;
            SidebarController.MenuLocation = MenuLocations.Left;
        }
        void SetupSideMenu()
        {
            //_sideMenuManager.LeftNavigationController = new UISideMenuNavigationController(_sideMenuManager, new MenuContentViewController(), leftSide: true);
            //_sideMenuManager.AddScreenEdgePanGesturesToPresent(toView: this.NavigationController?.View);
        }
        void SetDefaults()
        {
            //_sideMenuManager.BlurEffectStyle = null;
            //_sideMenuManager.AnimationFadeStrength = 100;
            //_sideMenuManager.ShadowOpacity = 0;
            //_sideMenuManager.AnimationTransformScaleFactor = 1;
            //_sideMenuManager.FadeStatusBar = false;
            //_sideMenuManager.AnimationBackgroundColor = UIColor.FromRGB(165, 163, 250);
        }
        #region Eventos
      
        #endregion

    }
}