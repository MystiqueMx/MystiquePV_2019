using AudioToolbox;
using Firebase.Analytics;
using Foundation;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Generic;
using UIKit;
using ViewShaker;
namespace MystiqueNative.iOS
{
    public partial class LoginEmailViewController : UIViewController
    {

        #region DECLARACIONES

        //public LoadingOverlay loadPop { get; set; }
        UIActivityIndicatorView LoadingIndicator;

        #endregion

        #region CONSTRUCTOR
        public LoginEmailViewController(IntPtr handle) : base(handle)
        {
        }
        [Action("UnwindToLoginEmailViewController:")]
        public void UnwindToLoginEmailViewController(UIStoryboardSegue segue)
        {

        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            EmptyLabel.Text = "";
            #region Loading Overlay

            //var bounds = UIScreen.MainScreen.Bounds;
            //loadPop = new LoadingOverlay(bounds, "Autentificando...");

            #endregion

            #region Gestures

            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            #endregion

            #region Inputs LogIn Email
            this.CorreoTextField.ShouldReturn += (textField) =>
            {
                PasswordTextField.BecomeFirstResponder();
                return true;
            };

            this.PasswordTextField.ShouldReturn += (textField) =>
            {

                View.EndEditing(true);
                return true;
            };
            #endregion


        }


        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
        }
        #endregion

        #region View Will Appear
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        #endregion

        #region View Did Disappear
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished -= Instance_OnIniciarSesionFinished;
        }


        #endregion

        #endregion

        #region METODOS INTERNOS

        private void Instance_OnIniciarSesionFinished(object sender, LoginEventArgs e)
        {
            LoadingIndicator.StopAnimating();
            Console.WriteLine(MystiqueApp.VersionIOS);
            if (Helpers.ValidarVersiones.ValidarAppVersion())
            {
                if (e.Success)
                {
                    //ABRIR PANTASHA PRINCIPAL
                    PreferencesHelper.SetCredentials(e.Username, e.Password);
                    BeginInvokeOnMainThread(() =>
                    {
                        //loadPop.Hide();
                        PerformSegue("PANTALLA_PRINCIPAL_SEGUE", this);
                    });
                }
                else
                {
                    //loadPop.Hide();
                    var Alert = UIAlertController.Create("Inicio de sesión", e.Message, UIAlertControllerStyle.Alert);
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

        #endregion

        #region EVENTOS

        partial void IniciarSesionButton_TouchUpInside(UIButton sender)
        {
            IList<UIView> Views = new List<UIView>();
            View.EndEditing(true);
            Views.Add(CorreoTextField);
            Views.Add(PasswordTextField);
            Views.Add(EmptyLabel);
            EmptyLabel.Text = "";
            if (string.IsNullOrEmpty(CorreoTextField.Text) || string.IsNullOrEmpty(PasswordTextField.Text))
            {
                EmptyLabel.Text = "Por favor, llene todos los campos";
                var viewShaker = new ViewShaker.ViewShaker(Views);
                viewShaker.Shake();
                SystemSound.Vibrate.PlaySystemSound();
                // var okAlertController = UIAlertController.Create("Iniciar Sesión", "Por favor, llene todos los campos", UIAlertControllerStyle.Alert);
                //  okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //   PresentViewController(okAlertController, true, null);

            }
            else
            {
                if (ValidatorHelper.IsValidEmail(CorreoTextField.Text))
                {
                    string user = CorreoTextField.Text;
                    string password = PasswordTextField.Text;
                    //View.Add(loadPop);
                    LoadingIndicator = iOSUtils.SetLoadingIndicatorLabel(View, UIColor.FromRGBA(255, 255, 255, 150));
                    BeginInvokeOnMainThread(async () =>
                    {

                        var keys = new NSString[]
                        {
                        ParameterNamesConstants.Source,
                        };

                        var objects = new NSObject[]
                        {
                        new NSString(AnalyticsActions.MetodosLogin.LOGIN_CORREO) as NSObject
                        };

                        var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                        Analytics.LogEvent(EventNamesConstants.Login, dictionary);

                        await AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Email, user, password, null, null);

                    });

                }
                else
                {
                    var viewShaker = new ViewShaker.ViewShaker(this.CorreoTextField);
                    viewShaker.Shake();

                    var controller = Helpers.GetTopViewController.GetVisibleViewController();
                    var okAlertController2 = UIAlertController.Create("Iniciar Sesión", "El correo o la contraseña son incorrectas", UIAlertControllerStyle.Alert);
                    okAlertController2.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    controller.PresentViewController(okAlertController2, true, null);
                }
            }


        }

        //partial void RegresarButton_TouchUpInside(UIButton sender)
        //{
        //    NavigationController?.PopViewController(true);
        //}


        #endregion

    }
}