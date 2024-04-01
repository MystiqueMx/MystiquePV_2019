using Com.OneSignal;
using Facebook.CoreKit;
using Firebase.Core;
using Firebase.Crashlytics;
using Foundation;
using Google.SignIn;
using MystiqueNative.Helpers;
using MystiqueNative.iOS.Helpers;
//using Google.Maps;
using MystiqueNative.ViewModels;
using System;
using System.Linq;
using UIKit;

namespace MystiqueNative.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        string appId = "667292223740940";
        string appName = "Fresco App";
        public string[] paths;
        public string scheme, path, query, openUrlOptions;
        public NSUrl URL;


        // NSObject Appversion = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

        // VIEWMODELS
        public static AuthViewModelV2 Auth { get => AuthViewModelV2.Instance; set { } }
        public static AuthViewModelV2 AuthV2 { get; set; }
        public static BeneficiosViewModel Beneficios { get; set; }
        public static ComentariosViewModel Comment { get; set; }
        public static NotificacionesViewModel ObtenerNotificaciones { get; set; }
        public static WalletViewModel Wallet { get; set; }
        public static CitypointsViewModel CityPoints { get; set; }
        public static TwitterLoginViewModel LogInWithTwitter { get; set; }


        string Username = PreferencesHelper.GetUser();
        string Password = PreferencesHelper.GetPassword();

        string Usernamefb = PreferencesHelper.GetUserFB();
        string Passwordfb = PreferencesHelper.GetPasswordFB();

        string UsernameGoogle = PreferencesHelper.GetUserGoogle();
        string PasswordGoogle = PreferencesHelper.GetPasswordGoogle();

        string UsernameTwitter = PreferencesHelper.GetUserTwitter();
        string PasswordTwitter = PreferencesHelper.GetPasswordTwitter();

        const string MapsApiKey = "AIzaSyCEYWl8j5LMbfVSkQYgEfDCiXsit7fZBUA";

        public override UIWindow Window
        {
            get;
            set;
        }
        UIWindow window;
        public static UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);
        public static UIStoryboard StoryboardMenu = UIStoryboard.FromName("Menu", null);

        public static UIViewController initialViewController;
        public string oauth;
        public string oauth_verifier;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            //var storyboard = UIStoryboard(name: "Main", bundle: nil)
            //GOOGLE CONFIG SIGNIN:
            App.Configure();
            Crashlytics.Configure();

            var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
            SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();

            LogInWithTwitter = new TwitterLoginViewModel();
            Comment = new ComentariosViewModel();
            ObtenerNotificaciones = new NotificacionesViewModel();
            Beneficios = new BeneficiosViewModel();
            Wallet = new WalletViewModel();
            CityPoints = new CitypointsViewModel();

            Profile.EnableUpdatesOnAccessTokenChange(true);
            Settings.AppID = appId;
            Settings.DisplayName = appName;

            //   MapServices.ProvideAPIKey(MapsApiKey);
            if (ObjCRuntime.Runtime.Arch != ObjCRuntime.Arch.SIMULATOR)
            {
                OneSignal.Current.StartInit("6c0db721-4fdd-4fe2-961e-679bb6a67327")
                  .EndInit();
            }
            BeginInvokeOnMainThread(() =>
           {
               OneSignal.Current.IdsAvailable(RegistrarPlayerID);
           });


            MystiqueApp.DevicePlatform = "iOS";
            MystiqueApp.DeviceModel = UIDevice.CurrentDevice.Model;
            MystiqueApp.DeviceVersion = UIDevice.CurrentDevice.SystemVersion;
            MystiqueApp.DeviceId = UIDevice.CurrentDevice.Name;
            MystiqueApp.DeviceConnectionType = "Mobile";

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            return true;
        }

        private void ContinueWithScenes(bool success)
        {
            var ip = new System.Net.IPAddress(0);
            using (var defaultRouteReachability = new SystemConfiguration.NetworkReachability(ip))
            {
                if (!defaultRouteReachability.TryGetFlags(out var flags))
                {
                    SinConexion();
                    return;
                }

                var isReachable = (flags & SystemConfiguration.NetworkReachabilityFlags.Reachable) != 0;
                var noConnectionRequired = (flags & SystemConfiguration.NetworkReachabilityFlags.ConnectionRequired) == 0;
                if ((flags & SystemConfiguration.NetworkReachabilityFlags.IsWWAN) != 0)
                    noConnectionRequired = true;
                if (!isReachable || !noConnectionRequired)
                {
                    SinConexion();
                    return;
                }
            }
            if (ValidarVersiones.ValidarAppVersion())
            {
                if (success)
                {
                    initialViewController = StoryboardMenu.InstantiateInitialViewController() as UIViewController;
                    window.RootViewController = initialViewController;
                    window.MakeKeyAndVisible();
                }
                else
                {
                    initialViewController = Storyboard.InstantiateInitialViewController() as UIViewController;
                    window.RootViewController = initialViewController;
                    // @TODO
                    //var Alert = UIAlertController.Create("Inicio de sesión", e.Message, UIAlertControllerStyle.Alert);
                    //Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    //window.MakeKeyAndVisible();
                    //initialViewController.PresentViewController(Alert, true, null);
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
                initialViewController.PresentViewController(alert, true, null);
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
            initialViewController.PresentViewController(alert, true, null);
        }
        private void SinCredenciales()
        {
            initialViewController = Storyboard.InstantiateInitialViewController();
            window.RootViewController = initialViewController;
            window.MakeKeyAndVisible();
        }

        private void RegistrarPlayerID(string playerID, string pushToken)
        {
            MystiqueApp.PlayerId = playerID;

            //if (PreferencesHelper.IsUserSaveFB())
            //{
            //    Login(AuthMethods.Facebook, Usernamefb, Passwordfb);
            //}
            //else if (PreferencesHelper.IsUserSaveGoogle())
            //{
            //    Login(AuthMethods.Google, UsernameGoogle, PasswordGoogle);
            //}
            //else if (PreferencesHelper.IsUserSave())
            //{
            //    Login(AuthMethods.Email, Username, Password);
            //}
            //else if (PreferencesHelper.IsUserSaveTwitter())
            //{
            //    Login(AuthMethods.Twitter, UsernameTwitter, PasswordTwitter);
            //}
            //else if (!PreferencesHelper.IsUserSave() && !PreferencesHelper.IsUserSaveFB())
            //{
            //    BeginInvokeOnMainThread(SinCredenciales);
            //}

        }

        private async void Login(AuthMethods method, string Username, string Password) =>
            ContinueWithScenes(await AuthViewModelV2.Instance.IniciarSesion(method, Username, Password, null, null));

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.

            Console.WriteLine("App entering background state.");
            Console.WriteLine("Now receiving location updates in the background");

        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
            Console.WriteLine("App will enter foreground");
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            Console.WriteLine("App is becoming active");
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
        //public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        //{
        //    var openUrlOptions = new UIApplicationOpenUrlOptions(options);
        //    paths = url.AbsoluteUrl.PathComponents;
        //    scheme = url.Scheme;
        //    path = url.Path;
        //    query = url.Query;
        //    URL = url.AbsoluteUrl;
        //    var x = new NSUrlComponents(url, false);
        //    oauth = x.QueryItems.FirstOrDefault(c => c.Name == "oauth_token")?.Value;
        //    oauth_verifier = x.QueryItems.FirstOrDefault(c => c.Name == "oauth_verifier")?.Value;
        //    return SignIn.SharedInstance.HandleUrl(url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
        //}
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.


            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }


    }
}

