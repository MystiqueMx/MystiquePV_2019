using Facebook.CoreKit;
using Facebook.LoginKit;
using Firebase.Analytics;
using Foundation;
using Google.SignIn;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Generic;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class LoginViewController : UIViewController, ISignInUIDelegate, ISignInDelegate
    {
        #region Declaraciones
        LoginManager LM = new LoginManager();
        UIActivityIndicatorView LoadingIndicator;
        bool isFBLogIn;
        public static UIStoryboard StoryboardMenu = UIStoryboard.FromName("Menu", null);
        public static UIViewController initialViewController;
        List<string> readPermissions = new List<string> { "public_profile", "email" };

        #endregion

        #region Constructor
        public LoginViewController(IntPtr handle) : base(handle)
        {
        }
        [Action("UnwindToLoginViewController:")]
        public void UnwindToLoginViewController(UIStoryboardSegue segue)
        {

        }
        #endregion

        #region LifeCycle

        #endregion
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // TwitterButton.Hidden = true;
            SignIn.SharedInstance.UIDelegate = this;
            SignIn.SharedInstance.Delegate = this;

            #region Constraints Screensizes

            if (UIScreen.MainScreen.Bounds.Size.Height > 568)
            {
                //LoginLogoTop.Constant = 70;
                // LoginInputTop.Constant = 40;
                // SocialLoginsBottom.Constant = 15;
            }
            if (UIScreen.MainScreen.Bounds.Size.Height >= 736)
            {
                //LoginLogoTop.Constant = 80;
                // LoginInputTop.Constant = 50;
                // SocialLoginsBottom.Constant = 25;
            }

            #endregion

            SetLoginWithFB();

            #region Gestures

            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            #endregion

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
        }

        #region INICIAR SESION
        private void Instance_OnIniciarSesionFinished(object sender, LoginEventArgs e)
        {
            Console.WriteLine("VERSION IOS EN BD: " + MystiqueApp.VersionIOS.ToUpper());
            LoadingIndicator.StopAnimating();
            if (ValidarVersiones.ValidarAppVersion())
            {
                if (e.Success)
                {
                    if (e.Method == AuthMethods.Google)
                    {
                        PreferencesHelper.SetCredentialsGoogle(e.Username, e.Password);
                    }
                    else
                    {
                        PreferencesHelper.SetCredentialsFB(e.Username, e.Password);
                    }

                    PerformSegue("PANTALLA_PRINCIPAL_SEGUE", this);
                }
                else
                {
                    var Alert = UIAlertController.Create("Inicio de sesión", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(Alert, true, null);
                    GoogleButton.UserInteractionEnabled = true;
                    LoginfbButton.UserInteractionEnabled = true;
                    if (SignIn.SharedInstance.CurrentUser != null)
                    {
                        SignIn.SharedInstance.SignOutUser();
                    }
                }
            }
            else
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
        #endregion


        #region VIEW DID APPEAR
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        #endregion


        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            AuthViewModelV2.Instance.OnIniciarSesionFinished -= Instance_OnIniciarSesionFinished;
        }

        private void SolicitarCorreoFB()
        {
            Profile.Notifications.ObserveDidChange((sender, e) =>
            {

                if (e.NewProfile == null)
                    return;

                var request = new GraphRequest("/" + e.NewProfile.UserID, new NSDictionary("fields", "first_name,last_name,email,gender,id"), AccessToken.CurrentAccessToken.TokenString, null, "GET");
                request.Start((connection, result, error) =>
                {
                    if (error != null)
                    {
                        Console.WriteLine("Falló la conexion con Facebook");
                    }

                    var userInfo = result as NSDictionary;

                    IniciarSesionFB(userInfo);
                });
            });

        }
        public async void IniciarSesionFB(NSDictionary userInfo)
        {
            try
            {
                isFBLogIn = true;
                string id = userInfo["id"].ToString();
                string name = userInfo["first_name"].ToString();
                string apellido = userInfo["last_name"].ToString();

                if (userInfo["email"] == null)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        iOSUtils.MostrarAlerta(this, "Error al conectarse a los servidores de facebook", "No se ha encontrado un correo electrónico asociado con la cuenta de Facebook proporcionada");
                        new LoginManager().LogOut();
                        View.UserInteractionEnabled = true;
                    });
                }
                else
                {
                    string email = userInfo["email"].ToString();

                    MystiqueApp.FacebookId = id;
                    MystiqueApp.FbProfile = new Models.Facebook.FacebookProfile()
                    {
                        Email = email,
                    };
                    var appurl = UIApplication.SharedApplication.Delegate as AppDelegate;
                    appurl.oauth = null;
                    appurl.oauth_verifier = null;

                    var keys = new NSString[]
                              {
                        ParameterNamesConstants.Source,
                              };

                    var objects = new NSObject[]
                    {
                        new NSString(AnalyticsActions.MetodosLogin.LOGIN_FACEBOOK) as NSObject
                    };

                    var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
                    Analytics.LogEvent(EventNamesConstants.Login, dictionary);

                    BeginInvokeOnMainThread(async () =>
                    {
                        LoadingIndicator = iOSUtils.SetLoadingIndicatorLabel(View, UIColor.FromRGBA(255, 255, 255, 150));
                        await AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Facebook, email, id, name, apellido);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("~ ERROR FACEBOOK" + ex);
                LoadingIndicator?.StopAnimating();
                iOSUtils.MostrarAlerta(this, "Error al conectarse a los servidores de facebook", "Error detectado");
            }
        }

        //partial void TwitterButton_TouchUpInside(UIButton sender)
        //{
        //    //AppDelegate.LogInWithTwitter.ObtenerUrlOAuth();
        //}

        partial void GoogleButton_TouchUpInside(UIButton sender)
        {
            #region Google LogIn

            if (SignIn.SharedInstance.CurrentUser != null)
            {
                BeginInvokeOnMainThread(async () =>
                {
                    LoadingIndicator = iOSUtils.SetLoadingIndicatorLabel(View, UIColor.FromRGBA(255, 255, 255, 150));
                    var user = SignIn.SharedInstance.CurrentUser;
                    await AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Google, user.Profile.Email, user.UserID, user.Profile.GivenName, user.Profile.FamilyName);
                });
            }
            else
            {
                SignIn.SharedInstance.SignInUser();
                GoogleButton.UserInteractionEnabled = false;
            }

            #endregion
        }

        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            #region Google Sign-In Delegate

            if (user != null && error == null)
            {
                LoadingIndicator = iOSUtils.SetLoadingIndicatorLabel(View, UIColor.FromRGBA(255, 255, 255, 150));
                AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Google, user.Profile.Email, user.UserID, user.Profile.GivenName, user.Profile.FamilyName);
            }
            else
            {
                GoogleButton.UserInteractionEnabled = true;
            }
            #endregion
        }
        private void SetLoginWithFB()
        {
            #region Setting fb login
            LoginfbButton.TouchUpInside += delegate
            {
                View.UserInteractionEnabled = false;
                LM.LoginBehavior = LoginBehavior.Native;
                LM.LogInWithReadPermissions(readPermissions.ToArray(), this, (result, e) =>
                {
                    if (e != null)
                    {
                        Console.WriteLine("Falló la conexion con Facebook");
                        View.UserInteractionEnabled = true;
                    }

                    if (result.IsCancelled)
                    {
                        Console.WriteLine("La conexion con Facebook fue cancelada");
                        View.UserInteractionEnabled = true;
                    }

                    Console.WriteLine("Has iniciado sesión con Facebook");
                    SolicitarCorreoFB();
                }); ;
            };
            #endregion
        }
    }
}