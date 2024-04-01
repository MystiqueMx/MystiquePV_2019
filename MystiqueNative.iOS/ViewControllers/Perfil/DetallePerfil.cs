using Foundation;
using System;
using UIKit;
using FFImageLoading;
using MystiqueNative.iOS.Helpers;
using FFImageLoading.Transformations;
using Facebook.LoginKit;
using MystiqueNative.iOS.View;
using Google.SignIn;

namespace MystiqueNative.iOS
{
    public partial class DetallePerfil : UITableViewController
    {
        LoadingOverlay loadPop;
        private NSTimer timer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Cerrando Sesión...");
          
          
            //if (AppDelegate.Auth.Usuario != null)
            //{
            //    if (AppDelegate.Auth.LoggedStatus)
            //    {
            //        nombreLabel.Text = AppDelegate.Auth.Usuario.Nombre + " " + AppDelegate.Auth.Usuario.Paterno + " " + AppDelegate.Auth.Usuario.Materno;
            //        nombreLabel.AdjustsFontSizeToFitWidth = true;
            //        CorreoField.Text = AppDelegate.Auth.Usuario.Email;

            //        if (string.IsNullOrEmpty(AppDelegate.Auth.Usuario.Telefono))
            //        {
            //            PhoneField.Text = string.Empty;
            //            PhoneField.Placeholder = "Teléfono sin registrar";
            //        }
            //        else
            //        {
            //            PhoneField.Text = AppDelegate.Auth.Usuario.Telefono;
            //        }

            //        if (string.IsNullOrEmpty(AppDelegate.Auth.Usuario.Colonia))
            //        {
            //            ColoniaField.Text = string.Empty;
            //            ColoniaField.Text = "Colonia sin registrar";
            //        }
            //        else
            //        {
            //            ColoniaField.Text = AppDelegate.Auth.Usuario.Colonia;
            //        }



            //        if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
            //        {
            //            FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
            //        }

            //    }
            //}

            //ProfilePictureDetalle.Image =
            //ToCircleTransformation.ToRounded(ProfilePictureDetalle.Image, 0f, 1f, 1f, 0d, null);

            //***CLOSE KEYBOARD ON TAP OUTSIDE *** //
            View.AddGestureRecognizer(new UITapGestureRecognizer(() => View.EndEditing(true)));

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (AppDelegate.Auth.Usuario != null)
            {
                //if (AppDelegate.Auth.LoggedStatus)
                //{
                //    nombreLabel.Text = AppDelegate.Auth.Usuario.Nombre +" " + AppDelegate.Auth.Usuario.Paterno + " "+ AppDelegate.Auth.Usuario.Materno;
                //    CorreoField.Text = AppDelegate.Auth.Usuario.Email;
                //    nombreLabel.AdjustsFontSizeToFitWidth = true;
                //    //PasswordField.Text = AppDelegate.Auth.Usuario.Nombre;
                //    PhoneField.Text = AppDelegate.Auth.Usuario.Telefono;
                //    if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
                //    {
                //        FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
                //    }

                //    if (string.IsNullOrEmpty(AppDelegate.Auth.Usuario.Colonia))
                //    {
                //        ColoniaField.Text = string.Empty;
                //        ColoniaField.Text = "Colonia sin registrar";
                //    }
                //    else
                //    {
                //        ColoniaField.Text = AppDelegate.Auth.Usuario.Colonia;
                //    }
                //}
            }

        }
        public DetallePerfil (IntPtr handle) : base (handle)
        {
        }

        partial void CerrarSesion_TouchUpInside(UIButton sender)
        {
            AppDelegate.Auth.CerrarSesion();
            new LoginManager().LogOut();
            SignIn.SharedInstance.SignOutUser();
            AppDelegate.Auth.ErrorMessage = string.Empty;

            PreferencesHelper.SetCredentials(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsFB(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsGoogle(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsTwitter(string.Empty, string.Empty);

            loadPop.Hide();
            PerformSegue("LOGOUT_SEGUE", this);
            return;
        }

      
    }
}