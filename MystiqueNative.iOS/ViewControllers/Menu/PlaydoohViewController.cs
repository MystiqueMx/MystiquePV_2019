using CoreGraphics;
using Firebase.Analytics;
using Foundation;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class PlaydoohViewController : UIViewController
    {
        public static UIViewController initialViewController;
        NSObject Appversion = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
        UIWindow window;
        UILabel label = new UILabel(new CGRect(x: 22, y: 0, width: 15, height: 15));
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetNavigationBarLogo();
            AppDelegate.ObtenerNotificaciones.ObtenerNotificaciones();
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged += Notificaciones_CollectionChanged;

            MiSaldo.AdjustsFontSizeToFitWidth = true;
            label.Layer.BorderColor = UIColor.Clear.CGColor;
            label.Layer.BorderWidth = 2;
            label.Layer.CornerRadius = label.Bounds.Size.Height / 2;
            label.TextAlignment = UITextAlignment.Center;
            label.Layer.MasksToBounds = true;
            // label.SizeToFit();
            label.Font = UIFont.SystemFontOfSize(10);
            //label.Font.WithSize(0.1f);
            label.TextColor = UIColor.White;
            label.BackgroundColor = UIColor.Red;
            //Inicio.Dismiss();
            // buttonvgjhgj
            //var rightButton = new UIButton(new CGRect(x: 0, y: 0, width: 18, height: 18));
            //rightButton.SetBackgroundImage(UIImage.FromBundle("Notification-24x24@2x"), UIControlState.Normal);
            // rightButton.AddTarget(null,UIControlEvent.TouchUpInside);
            NotificacionesBtn.AddSubview(label);
            //var rightBarButtomItem = new UIBarButtonItem(rightButton);
            //NavigationItem.RightBarButtonItem = rightBarButtomItem;

            #region SET SEGUES
            BeneficiosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
                {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
                };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_PROMOCIONES) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);

                PerformSegue("LISTABENEFICIOS_SEGUE", this);

            }));

            CapturarPuntosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                //PerformSegue("SumarPuntosSegue", this);

                ScanAsync();
            }));
            CityPointsView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
             {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
             };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_HISTORIAL_PUNTOS) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                PerformSegue("MISALDO_SEGUE", this);
            }));
            CanjearPuntosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
             {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
             };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_RECOMPENSAS) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                PerformSegue("CANJEARPUNTOS_SEGUE", this);
            }));
            ComentariosView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
             {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
             };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_COMENTARIOS) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                PerformSegue("COMENTARIO_SEGUE", this);
            }));
            MembresiaView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
                 {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
                 };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_MEMBRESIA) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                PerformSegue("MEMBRESIA_SEGUE", this);
            }));
            //SOPORTE_SEGUE
            SoporteView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
               {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
               };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_AYUDA) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                PerformSegue("SOPORTE_SEGUE", this);
            }));
            #endregion

            QuieroDeComerView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var keys = new NSString[]
           {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
           };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_QDC) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);
                OpenQuieroDeComer("10");
            }));
        }
        #region OVERRIDES
        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            switch (segueIdentifier)
            {
                case "SumarPuntosSegue": return AuthViewModelV2.Instance.Usuario.RegistroCompleto;
                default: return true;
            }

        }
        #endregion

        private void CityPoints_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AgregarStatus")
            {
                if (AppDelegate.CityPoints.AgregarStatus)
                {
                    //if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    //{
                    //    var okAlertController = UIAlertController.Create("City Points", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                    //    var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                    //    {
                    //        AppDelegate.CityPoints.ObtenerEstadoCuenta();
                    //        //MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";
                    //    });
                    //    okAlertController.AddAction(okAction);
                    //    okAlertController.PreferredAction = okAction;
                    //    PresentViewController(okAlertController, true, null);
                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {
                        var okAlertController = UIAlertController.Create("Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                        {
                            AppDelegate.CityPoints.ObtenerEstadoCuenta();
                            //MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";
                        });
                        okAlertController.AddAction(okAction);
                        okAlertController.PreferredAction = okAction;
                        PresentViewController(okAlertController, true, null);
                    }
                }
            }
            if (e.PropertyName == "CanjearStatus")
            {
                if (AppDelegate.CityPoints.CanjearStatus)
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {
                        var okAlertController = UIAlertController.Create("Puntos", AppDelegate.CityPoints.ErrorMessage, UIAlertControllerStyle.Alert);
                        var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                        {
                            AppDelegate.CityPoints.ObtenerEstadoCuenta();
                            //MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";
                        });
                        okAlertController.AddAction(okAction);
                        okAlertController.PreferredAction = okAction;
                        PresentViewController(okAlertController, true, null);
                    }

                    if (string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString()))
                    {
                        MiSaldo.Text = "Mi Saldo: ";
                    }
                    else
                    {
                        MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";

                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppDelegate.CityPoints.ErrorMessage))
                    {

                    }
                }
            }
            if (e.PropertyName == "ErrorStatus")
            {

                //if (!AppDelegate.CityPoints.ErrorStatus)
                //{
                //    if (string.IsNullOrEmpty(AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString()))
                //    {
                //        MiSaldo.Text = "Mi Saldo: ";
                //    }
                //    else
                //    {
                //        MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";

                //    }
                //}
            }

        }

        private NotificacionesViewModel ViewModel;
        private void Notificaciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AppDelegate.ObtenerNotificaciones.NotificacionesNuevas > 0)
            {
                label.Hidden = false;
                label.Text = AppDelegate.ObtenerNotificaciones.NotificacionesNuevas.ToString();

            }
            else
            {
                label.Hidden = true;
            }
        }

        public void ScanAsync()
        {
            if (AppDelegate.Auth.Usuario.RegistroCompleto)
            {
                var keys = new NSString[]
             {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
             };

                var objects = new NSObject[]
                {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.Flujos.FLUJO_BOTON_SUMAR_PUNTOS) as NSObject
                };

                var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                Analytics.LogEvent(EventNamesConstants.ViewItem, dictionary);

                PerformSegue("SumarPuntosSegue", this);

                //window = UIApplication.SharedApplication.KeyWindow;
                //var vc = window.RootViewController;
                //while (vc.PresentedViewController != null)
                //{
                //    vc = vc.PresentedViewController;
                //}
                //var scanner = new ZXing.Mobile.MobileBarcodeScanner(vc);
                //var result = await scanner.Scan();

                //if (result != null)
                //{
                //    AppDelegate.CityPoints.AgregarPuntos(result.Text);
                //}
            }
            else
            {
                var okAlertController = UIAlertController.Create("Puntos", "Complete su registro para poder continuar", UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var PerfilAction = UIAlertAction.Create("Completar Registro", UIAlertActionStyle.Default, (OK) =>
                {
                    PerformSegue("COMPLETAR_REGISTRO_SEGUE", this);
                });
                okAlertController.AddAction(okAction);
                okAlertController.AddAction(PerfilAction);
                okAlertController.PreferredAction = PerfilAction;
                PresentViewController(okAlertController, true, null);
            }

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            AppDelegate.CityPoints.ObtenerEstadoCuenta();
            AppDelegate.CityPoints.OnAgregarPuntosFinished += CityPoints_OnAgregarPuntosFinished;
            AppDelegate.CityPoints.OnEstadoCuentaFinished += CityPoints_OnEstadoCuentaFinished;
            AppDelegate.CityPoints.PropertyChanged += CityPoints_PropertyChanged;
            AppDelegate.ObtenerNotificaciones.ObtenerNotificaciones();

            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(6, 181, 158);

            if (!Helpers.ValidarVersiones.ValidarAppVersion())
            {
                //if (MystiqueApp.VersionIOS != Appversion.ToString())
                //{
                var alert = UIAlertController.Create("Actualización Disponible", "Para seguir disfrutando de los beneficios de Fresco es necesario instalar la nueva versión",
                    UIAlertControllerStyle.Alert);

                var WalletAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                {
                    PerformSegue("MenuSegue", this);

                });
                var OkAction = UIAlertAction.Create("Ir a App Store", UIAlertActionStyle.Default, (Link) =>
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/mx/app/apple-store/id1460400309"));
                });
                alert.AddAction(OkAction);
                alert.AddAction(WalletAction);
                alert.PreferredAction = WalletAction;

                PresentViewController(alert, true, null);
                //}
            }
        }

        private void CityPoints_OnEstadoCuentaFinished(object sender, EstadoCuentaArgs e)
        {
            if (e.EstadoCuenta.Success)
            {
                MiSaldo.Text = "Mi Saldo: " + e.EstadoCuenta.PuntosAsInt.ToString() + "pts";
            }
            else
            {
                AppDelegate.CityPoints.ObtenerEstadoCuenta();
            }
        }

        private void CityPoints_OnAgregarPuntosFinished(object sender, AgregarPuntosArgs e)
        {
            if (e.Success)
            {
                var okAlertController = UIAlertController.Create("Canjear Puntos", e.Message, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
                MiSaldo.Text = "Mi Saldo: " + AppDelegate.CityPoints.EstadoCuenta.PuntosAsInt.ToString() + "pts";
            }
            else
            {
                var okAlertController = UIAlertController.Create("Canjear Puntos", e.Message, UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.CityPoints.OnAgregarPuntosFinished -= CityPoints_OnAgregarPuntosFinished;
            AppDelegate.CityPoints.OnEstadoCuentaFinished -= CityPoints_OnEstadoCuentaFinished;
            AppDelegate.CityPoints.PropertyChanged -= CityPoints_PropertyChanged;
            AppDelegate.ObtenerNotificaciones.Notificaciones.CollectionChanged -= Notificaciones_CollectionChanged;

        }
        public PlaydoohViewController(IntPtr handle) : base(handle)
        {

        }

        public void OpenQuieroDeComer(string p)
        {
            //TODO UPDATE QUIERODECOMER IOS VERSION 
            NSUrl request = new NSUrl("quierodecomer://" + p);

            try
            {
                if (!UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("qdc://com.QuieroDeComer")))
                {
                    if (UIApplication.SharedApplication.CanOpenUrl(request))

                        UIApplication.SharedApplication.OpenUrl(request);
                    else
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/quierodecomer/id1216419620?mt=8"));
                    }
                }
            }
            catch (Exception ex)
            {
                var okAlertController = UIAlertController.Create("Quiero de Comer", "No se pudo abrir el enlace/app", UIAlertControllerStyle.Alert);

                //Add Action
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                // Present Alert
                PresentViewController(okAlertController, true, null);
            }
        }

        public bool ValidarVersiones()
        {
#if DEBUG
            return true;
            //PRODUCCION:
#else
            if ((MystiqueApp.VersionIOSPruebas != Appversion.ToString() && MystiqueApp.VersionIOS != Appversion.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }

#endif

        }

        //partial void NotificacionesBtn_TouchUpInside(UIButton sender)
        //{

        //}
        private void SetNavigationBarLogo()
        {
            //var NavLogo = new UIImageView(UIImage.FromBundle("Images/Logoblanco.png"));
            //NavLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
            //NavLogo.ClipsToBounds = true;
            //NavigationItem.TitleView = NavLogo;
            
        }
    }
}