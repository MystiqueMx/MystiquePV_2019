using Foundation;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class TabBarInicioViewController : UITabBarController
    {
        public TabBarInicioViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           // this.TabBarController.ViewControllers[1].TabBarItem.BadgeValue = "2";
          
         TabBar.UnselectedItemTintColor = new UIColor(red: 0.88f, green: 0.88f, blue: 0.88f, alpha: 1.0f);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged += Notificaciones_CollectionChanged;
            AppDelegate.ObtenerNotificaciones.ObtenerNotificaciones();

           
        }

     

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged -= Notificaciones_CollectionChanged;
        }
        private void Notificaciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AppDelegate.ObtenerNotificaciones.NotificacionesNuevas > 0)
            {
                this.TabBarController.ViewControllers[1].TabBarItem.BadgeValue = AppDelegate.ObtenerNotificaciones.NotificacionesNuevas.ToString();
            }
        }

      
    }
}