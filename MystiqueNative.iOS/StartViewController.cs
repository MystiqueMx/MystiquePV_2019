using Foundation;
using MystiqueNative.Helpers;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.ViewModels;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class StartViewController : UIViewController
    {
        public StartViewController(IntPtr handle) : base(handle)
        {
        }
        string Username = PreferencesHelper.GetUser();
        string Password = PreferencesHelper.GetPassword();

        string Usernamefb = PreferencesHelper.GetUserFB();
        string Passwordfb = PreferencesHelper.GetPasswordFB();

        string UsernameGoogle = PreferencesHelper.GetUserGoogle();
        string PasswordGoogle = PreferencesHelper.GetPasswordGoogle();

        string UsernameTwitter = PreferencesHelper.GetUserTwitter();
        string PasswordTwitter = PreferencesHelper.GetPasswordTwitter();

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
            TryLogin();
        }

        private void TryLogin()
        {
            try
            {
                if (PreferencesHelper.IsUserSaveFB())
                {
                    Login(AuthMethods.Facebook, Usernamefb, Passwordfb);
                }
                else if (PreferencesHelper.IsUserSaveGoogle())
                {
                    Login(AuthMethods.Google, UsernameGoogle, PasswordGoogle);
                }
                else if (PreferencesHelper.IsUserSave())
                {
                    Login(AuthMethods.Email, Username, Password);
                }
                else if (PreferencesHelper.IsUserSaveTwitter())
                {
                    Login(AuthMethods.Twitter, UsernameTwitter, PasswordTwitter);
                }
                else if (!PreferencesHelper.IsUserSave() && !PreferencesHelper.IsUserSaveFB())
                {
                    BeginInvokeOnMainThread(SinCredenciales);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                BeginInvokeOnMainThread(SinCredenciales);
            }

        }

        private async void Login(AuthMethods method, string Username, string Password) =>
    ContinueWithScenes(await AuthViewModelV2.Instance.IniciarSesion(method, Username, Password, null, null));
        private void ContinueWithScenes(bool success)
        {

            if (!ValidarVersiones.ValidarAppVersion())

            {
                AuthViewModelV2.Instance.CerrarSesion();
                var alert = UIAlertController.Create("Actualización Disponible", "Para seguir disfrutando de los beneficios de Fresco es necesario instalar la nueva versión",
                                   UIAlertControllerStyle.Alert);
                var WalletAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var OkAction = UIAlertAction.Create("Ir a App Store", UIAlertActionStyle.Default, (Link) =>
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/mx/app/apple-store/id1460400309"));
                });
                alert.AddAction(OkAction);
                alert.AddAction(WalletAction);
                alert.PreferredAction = WalletAction;
                PresentViewController(alert, true, null);
            }
        }
        private void SinConexion()
        {
            var alert = UIAlertController.Create("Sin conexión", "Intenta conectarte a internet para entrar a la aplicación",
                                   UIAlertControllerStyle.Alert);
            var WalletAction = UIAlertAction.Create("Salir", UIAlertActionStyle.Default, null);
            var OkAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null);
            alert.AddAction(OkAction);
            alert.AddAction(WalletAction);
            alert.PreferredAction = WalletAction;
            PresentViewController(alert, true, null);
        }
        private void SinCredenciales()
        {
            var _storyboard = UIStoryboard.FromName("Main", null);
            var controller = _storyboard.InstantiateViewController("LoginID") as LoginViewController;
            ShowViewController(controller, this);
        }
        private void Instance_OnIniciarSesionFinished(object sender, LoginEventArgs e)
        {
            if (e.Success)
            {
                var _storyboard = UIStoryboard.FromName("Menu", null);
                var controller = _storyboard.InstantiateViewController("MenuPrincipalID") as NavigationController;
                ShowViewController(controller, this);
            }
            else
            {
                var Alert = UIAlertController.Create("Inicio de sesión", e.Message, UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (ok) =>
                {
                    var _storyboard = UIStoryboard.FromName("Main", null);
                    var controller = _storyboard.InstantiateViewController("LoginID") as LoginViewController;
                    ShowViewController(controller, this);
                }));
                Alert.AddAction(UIAlertAction.Create("Reintentar", UIAlertActionStyle.Default, (retry) =>
                {
                    TryLogin();
                }));
                PresentViewController(Alert, true, null);
            }
        }
    }
}