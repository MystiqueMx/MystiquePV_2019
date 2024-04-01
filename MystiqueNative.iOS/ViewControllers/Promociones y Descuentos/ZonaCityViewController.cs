using CoreLocation;
using Foundation;
using MystiqueNative.iOS.Helpers;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ZonaCityViewController : UIViewController
    {
        #region Computed Properties
        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public static LocationManager Manager { get; set; }
        #endregion

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Longitud.Hidden = true;
            Latitud.Hidden = true;
            ZonaTitle.AdjustsFontSizeToFitWidth = true;

            //UIApplication.Notifications.ObserveDidBecomeActive((sender, args) => {
            //    Manager.LocationUpdated += HandleLocationChanged;
            //});
            //if (ZonaCitySwitch.On)
            //{
            //    Manager = new LocationManager();
            //    Manager.StartLocationUpdates();
            //}

            //else
            //{

            //}
            //UIApplication.Notifications.ObserveDidEnterBackground((sender, args) => {
            //    Manager.LocationUpdated -= HandleLocationChanged;
            //});
        }
        #region Override Methods
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        #endregion
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(6,181,158);
        }
        public ZonaCityViewController (IntPtr handle) : base (handle)
        {
            // As soon as the app is done launching, begin generating location updates in the location manager
         
        }

        #region Public Methods
        public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
        {
            // Handle foreground updates
            CLLocation location = e.Location;
            Longitud.Text = location.Coordinate.Longitude.ToString();
            Latitud.Text = location.Coordinate.Latitude.ToString();

            Console.WriteLine("foreground updated");
        }
        #endregion
    }
}