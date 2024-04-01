using Facebook.LoginKit;
using FFImageLoading;
using FFImageLoading.Transformations;
using Firebase.Analytics;
using Foundation;
using Google.SignIn;
using MaterialControls;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MiPerfilViewController : UIViewController
    {
        LoadingOverlay loadPop;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Cerrando Sesión...");
            Nombre.Text = AuthViewModelV2.Instance.Usuario.NombreCompleto;
            Correo.Text = AuthViewModelV2.Instance.Usuario.Email;
            Telefono.Text = AuthViewModelV2.Instance.Usuario.TelefonoConFormato;
            Colonia.Text = AuthViewModelV2.Instance.Usuario.Colonia;
            Nombre.AdjustsFontSizeToFitWidth = true;

            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Telefono))
            {
                Telefono.Text = string.Empty;
                Telefono.Placeholder = "Teléfono sin registrar";
            }
            else
            {
                Telefono.Text = AuthViewModelV2.Instance.Usuario.Telefono;
            }

            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Colonia))
            {
                Colonia.Text = string.Empty;
                Colonia.Text = "Colonia sin registrar";
            }
            else
            {
                Colonia.Text = AuthViewModelV2.Instance.Usuario.Colonia;
            }

            #region CONSTRAINTS SCREENSIZES
           
            if (UIScreen.MainScreen.Bounds.Size.Height == 736)
            {
                //ImagenRecompensa.Frame = new CoreGraphics.CGRect(ImagenRecompensa.Frame.X, ImagenRecompensa.Frame.X,
                //                                                        ImagenRecompensa.Frame.Width,350);
                NSLayoutConstraint.Create(LogOut, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 120).Active = true;
                NSLayoutConstraint.Create(LogOut, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 120).Active = true;
            }
            #endregion

            LogOut.MdButtonType = MaterialControls.MDButtonType.FloatingAction;


            FFImageLoading.ImageService.Instance.LoadUrl(AuthViewModelV2.Instance.Usuario.Foto).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
        

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            Nombre.Text = AuthViewModelV2.Instance.Usuario.NombreCompleto;
            Correo.Text = AuthViewModelV2.Instance.Usuario.Email;
            Telefono.Text = AuthViewModelV2.Instance.Usuario.TelefonoConFormato;
           // Colonia.Text = AppDelegate.Auth.Usuario.Colonia;
            Nombre.AdjustsFontSizeToFitWidth = true;

            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Telefono))
            {
                Telefono.Text = string.Empty;
                Telefono.Text = "Teléfono sin registrar";
            }
            else
            {
                Telefono.Text = AuthViewModelV2.Instance.Usuario.Telefono;
            }

            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Colonia))
            {
                
                Colonia.Text = "Colonia sin registrar";
            }
            else
            {
                Colonia.Text = AuthViewModelV2.Instance.Usuario.Colonia;
            }

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
            {
                FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
            }
        }
        public void CerrarSesion()
        {
            var keys = new NSString[]
                {
                ParameterNamesConstants.Character,
                ParameterNamesConstants.Source,
                };

            var objects = new NSObject[]
                    {
                    new NSString(AuthViewModelV2.Instance.Usuario.Id)as NSObject,
                    new NSString(AnalyticsActions.MetodosLogin.LOGOUT) as NSObject
                    };

            var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
            Analytics.LogEvent(EventNamesConstants.Login, dictionary);

            AppDelegate.Auth.CerrarSesion();
            new LoginManager().LogOut();
            AppDelegate.Auth.ErrorMessage = string.Empty;
            SignIn.SharedInstance.SignOutUser();

            PreferencesHelper.SetCredentials(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsFB(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsGoogle(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsTwitter(string.Empty, string.Empty);

            View.Add(loadPop);
            loadPop.Hide();
            this.NavigationController.SetNavigationBarHidden(true, true);
            PerformSegue("LOGOUT_SEGUE", this);
            return;
        }
        public MiPerfilViewController (IntPtr handle) : base (handle)
        {
        }

        partial void LogOut_TouchUpInside(MDButton sender)
        {
            var alert = UIAlertController.Create("Cerrar Sesión", "Usted esta a punto de cerrar su sesión", UIAlertControllerStyle.Alert);
            var WalletAction = UIAlertAction.Create("Cerrar", UIAlertActionStyle.Destructive, (OK) => { CerrarSesion(); });
            var OkAction = UIAlertAction.Create("Cancelar", UIAlertActionStyle.Cancel, null);

            alert.AddAction(OkAction);
            alert.AddAction(WalletAction);
            alert.PreferredAction = WalletAction;
            PresentViewController(alert, true, null);
        }
    }
}