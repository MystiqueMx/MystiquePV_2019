using AudioToolbox;
using Foundation;
using MystiqueNative.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using System.ComponentModel;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class RecuperarViewController : UIViewController
    {
        LoadingOverlay loadPop;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddGestureRecognizer(new UITapGestureRecognizer(() => View.EndEditing(true)));
            
            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Recuperando contraseña...");

        }

        private void Instance_OnRecuperarContrasenaFinished(object sender, RecuperarPasswordEventArgs e)
        {
            loadPop.Hide();
            if (Helpers.ValidarVersiones.ValidarAppVersion())
            {
                if (e.Success)
                {
                    var Alert = UIAlertController.Create("Recuperar Contraseña","Su contraseña se ha enviado a su correo", UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(Alert, true, null);
                }
                else
                {
                    var Alert = UIAlertController.Create("Recuperar Contraseña", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(Alert, true, null);
                }
            }
            else
            {
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

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppDelegate.Auth.PropertyChanged += Auth_PropertyChanged;
            AuthViewModelV2.Instance.OnRecuperarContrasenaFinished += Instance_OnRecuperarContrasenaFinished;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AuthViewModelV2.Instance.OnRecuperarContrasenaFinished -= Instance_OnRecuperarContrasenaFinished;
        }

        private void Auth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "RecoverPasswordStatus")
            {
                //if (AppDelegate.Auth.RecoverPasswordStatus)
                //{
                //    BeginInvokeOnMainThread(() =>
                //    {
                //        var okAlertController = UIAlertController.Create("Recuperar Contraseña", "Su contraseña se ha enviado a su correo", UIAlertControllerStyle.Alert);
                //        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                //        {
                //            NavigationController.PopViewController(true);
                //        }));
                //        PresentViewController(okAlertController, true, null);
                //        NavigationController.PopViewController(true);
                //    });
                //}
                //else
                //{
                //    BeginInvokeOnMainThread(() =>
                //    {
                //        if (!string.IsNullOrEmpty(AppDelegate.Auth.ErrorMessage))
                //        {
                //            var okAlertController = UIAlertController.Create("Recuperar Contraseña", AppDelegate.Auth.ErrorMessage, UIAlertControllerStyle.Alert);
                //            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                //            {
                //                NavigationController.PopViewController(true);
                //            }));
                //            PresentViewController(okAlertController, true, null);

                //            SystemSound.Vibrate.PlaySystemSound();
                //            AppDelegate.Auth.ErrorMessage = string.Empty;

                //        }
                //    });

                //}

            }
            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.Auth.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        View.EndEditing(true);
                        View.Add(loadPop);

                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {

                        loadPop.Hide();


                    });
                }
            }

        }

        public RecuperarViewController(IntPtr handle) : base(handle)
        {
        }

        partial void RecuperarButton_TouchUpInside(UIButton sender)
        {
            if (string.IsNullOrEmpty(inputEmail.Text))
            {
                var okAlertController = UIAlertController.Create("Recuperar Contraseña", "El campo esta vacío", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                SystemSound.Vibrate.PlaySystemSound();
                PresentViewController(okAlertController, true, null);

            }
            else if (!ValidatorHelper.IsValidEmail(inputEmail.Text))
            {
                var okAlertController = UIAlertController.Create("Recuperar Contraseña", "Ingrese un correo válido", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                SystemSound.Vibrate.PlaySystemSound();
                PresentViewController(okAlertController, true, null);
            }
            else
            {
                AuthViewModelV2.Instance.RecuperarContrasena(inputEmail.Text);
            }


        }
    }
}