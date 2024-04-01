using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using System.ComponentModel;
using AudioToolbox;

namespace MystiqueNative.iOS
{
    public partial class CityPointsViewController : UITableViewController
    {
        public static UIStoryboard StoryboardMenu = UIStoryboard.FromName("Main", null);
        public static UIViewController initialViewController;
        UIWindow window;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            #region GESTURES

            UITapGestureRecognizer tap = new UITapGestureRecognizer(ScanAsync);
            CapturarPuntosView.AddGestureRecognizer(tap);
            #endregion
            #region SET SEGUES
            CanjearPuntosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
                PerformSegue("CANJEAR_SEGUE", this);
            }));
            MiSaldoView.AddGestureRecognizer(tap = new UITapGestureRecognizer(() =>
            {
                AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
                PerformSegue("MISALDO_SEGUE", this);
            }
            ));

            RecompensasView.AddGestureRecognizer(tap = new UITapGestureRecognizer(() =>
            {
                AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
                PerformSegue("LISTA_RECOMPENSAS_SEGUE", this);
            }
            ));
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;
           // AppDelegate.CityPoints.ValidarRegistroCompleto();
            AppDelegate.CityPoints.ObtenerEstadoCuenta();

        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
        }

        private void CityPoints_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AgregarStatus")
            {
                if (AppDelegate.CityPoints.AgregarStatus)
                {
                    BeginInvokeOnMainThread(() =>
                    {

                        if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                        {
                            var okAlertController = UIAlertController.Create("City Points", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                            {
                                PerformSegue("MISALDO_SEGUE", this);
                            }));
                            PresentViewController(okAlertController, true, null);
                        }

                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                        {
                            var okAlertController = UIAlertController.Create("City Points", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            PresentViewController(okAlertController, true, null);

                            SystemSound.Vibrate.PlaySystemSound();
                            AppDelegate.Auth.ErrorMessage = string.Empty;

                        }
                    });

                }

            }
            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.Auth.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        View.EndEditing(true);


                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {

                    });
                }
            }
        }

        public async void ScanAsync()
        {
            if (AppDelegate.CityPoints.PuedeCanjear)
            {
                //window = new UIWindow(UIScreen.MainScreen.Bounds);
                //initialViewController = StoryboardMenu.InstantiateInitialViewController() as UIViewController;
                //window.RootViewController = initialViewController;
                window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }
                var scanner = new ZXing.Mobile.MobileBarcodeScanner(vc);
                var result = await scanner.Scan();

                if (result != null)
                {
                    AppDelegate.CityPoints.AgregarPuntos(result.Text);
                }
            }
            else
            {
                var okAlertController = UIAlertController.Create("City Points", AppDelegate.CityPoints.MensajePuedeCanjear, UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var PerfilAction = UIAlertAction.Create("Ir a Mi Perfil", UIAlertActionStyle.Default, (OK) =>
                {
                    PerformSegue("MI_PERFIL_SEGUE", this);
                });
                okAlertController.AddAction(okAction);
                okAlertController.AddAction(PerfilAction);
                okAlertController.PreferredAction = PerfilAction;

                PresentViewController(okAlertController, true, null);
            }

        }

        public CityPointsViewController(IntPtr handle) : base(handle)
        {
        }
    }
}