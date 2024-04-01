using Facebook.LoginKit;
using FFImageLoading;
using FFImageLoading.Transformations;
using Firebase.Analytics;
using Foundation;
using Google.SignIn;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class MembresiaViewController : UIViewController
    {
        LoadingOverlay loadPop;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Cerrando Sesión...");


    

            //if (AppDelegate.Auth.Usuario != null)
            //{
            //    if (AppDelegate.Auth.LoggedStatus)
            //    {
            //        Folio.Text = "FOLIO: " + AppDelegate.Auth.Usuario.FolioMembresia;
            //        Nombre.Text = AppDelegate.Auth.Usuario.NombreCompleto;
            //        VigenciaDato.Text = AppDelegate.Auth.Usuario.ExpiracionMembresiaConFormatoEspanyol;
            //        Sexo.Text = AppDelegate.Auth.Usuario.SexoAsString;
            //        FechaNacimiento.Text = AppDelegate.Auth.Usuario.FechaNacimientoConFormatoEspanyol;
            //        Telefono.Text = AppDelegate.Auth.Usuario.Telefono;

            //    }
            //}

            //Folio.AdjustsFontSizeToFitWidth = true;
            //Nombre.AdjustsFontSizeToFitWidth = true;
            //Vigencia.AdjustsFontSizeToFitWidth = true;
            //VigenciaDato.AdjustsFontSizeToFitWidth = true;

            if (UIScreen.MainScreen.Bounds.Size.Height >= 736)
            {
                // Folio.Font.WithSize(16);
                // Nombre.Font.WithSize(16);
                //// Vigencia.Font.WithSize(16);
                //// VigenciaDato.Font.WithSize(16);
                // FNacimiento.Font.WithSize(16);
                // FechaNacimiento.Font.WithSize(16);
                // SexoLabel.Font.WithSize(16);
                // Sexo.Font.WithSize(16);
                // Telefono.Font.WithSize(16);
                // TelefonoLabel.Font.WithSize(16);
                // BotonesBottom.Constant = 33;
            }
            //if (string.IsNullOrEmpty(VigenciaDato.Text))
            //{
            //  //  Vigencia.Text = "";
            //}

        }

        private void LoadUserData()
        {
            if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
            {
                ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 100).Into(ProfilePic);
            }
            //Folio.Text = "FOLIO: " + AppDelegate.Auth.Usuario.FolioMembresia;
            Nombre.Text = AppDelegate.Auth.Usuario.NombreCompleto;
            //VigenciaDato.Text = AppDelegate.Auth.Usuario.ExpiracionMembresiaConFormatoEspanyol;
            Sexo.Text = AppDelegate.Auth.Usuario.SexoAsString;
            FechaNacimiento.Text = AppDelegate.Auth.Usuario.FechaNacimientoConFormatoEspanyol;
            Telefono.Text = AppDelegate.Auth.Usuario.Telefono;

            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Colonia))
            {
                ColoniaLabel.Text = AuthViewModelV2.Instance.Usuario.Colonia;
            }
            if (ViewModels.AuthViewModelV2.Instance.Usuario.CiudadAsInt == 1)
            {
                CiudadLabel.Text = "Mexicali";
            }
            else if (ViewModels.AuthViewModelV2.Instance.Usuario.CiudadAsInt == 1)
            {
                CiudadLabel.Text = "Tijuana";
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            LoadUserData();
            if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
            {
                FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 100).Into(ProfilePic);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);



        }
        public MembresiaViewController(IntPtr handle) : base(handle)
        {
        }


        partial void BARCODE_TouchUpInside(UIButton sender)
        {
            ModalMembresiaBAR modal = Storyboard.InstantiateViewController("MembresiaBARID") as ModalMembresiaBAR;
            modal.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            modal.QRorBAR = "QR";
            NavigationController.PresentViewController(modal, true, null);
        }

        partial void QR_TouchUpInside(UIButton sender)
        {
            ModalMembresiaBAR modal = Storyboard.InstantiateViewController("MembresiaBARID") as ModalMembresiaBAR;
            modal.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            modal.QRorBAR = null;
            NavigationController.PresentViewController(modal, true, null);
        }

        partial void CerrarSesionButton_TouchUpInside(UIButton sender)
        {
            var alert = UIAlertController.Create("Cerrar Sesión", "Usted esta a punto de cerrar su sesión", UIAlertControllerStyle.Alert);
            var WalletAction = UIAlertAction.Create("Cerrar", UIAlertActionStyle.Destructive, (OK) => { CerrarSesion(); });
            var OkAction = UIAlertAction.Create("Cancelar", UIAlertActionStyle.Cancel, null);

            alert.AddAction(OkAction);
            alert.AddAction(WalletAction);
            alert.PreferredAction = WalletAction;
            PresentViewController(alert, true, null);
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
            NavigationController.SetNavigationBarHidden(true, true);
            PerformSegue("LOGOUT_SEGUE", this);
            return;
        }
    }
}