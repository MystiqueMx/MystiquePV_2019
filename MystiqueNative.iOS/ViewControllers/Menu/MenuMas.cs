using CoreGraphics;
using Foundation;
using System;
using UIKit;
using FFImageLoading.Transformations;
using FFImageLoading;
using System.ComponentModel;
using MystiqueNative.iOS.Helpers;
using AudioToolbox;
using MystiqueNative.iOS.View;
using System.Threading.Tasks;
using Facebook.LoginKit;
using Facebook.CoreKit;
using Facebook.ShareKit;

namespace MystiqueNative.iOS
{
    public partial class MenuMas : UITableViewController
    {
        LoadingOverlay loadPop;
        private NSTimer timer;
        string x = PreferencesHelper.GetUser();
        string y = PreferencesHelper.GetPassword();
        bool z =   PreferencesHelper.IsUserSave();
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var bounds = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds, "Cerrando Sesión...");

            //AppDelegate.Auth.PropertyChanged += Auth_PropertyChanged;

            UITapGestureRecognizer LogOut = new UITapGestureRecognizer(CerrarSesion);
            CellCerrarSesion.AddGestureRecognizer(LogOut);

            nombreMenu.Text = AppDelegate.Auth.Usuario.NombreCompleto;

            UITapGestureRecognizer tap = new UITapGestureRecognizer(QuierodeComer);
            StackQuierodeComer.AddGestureRecognizer(tap);
            CellQuieroDeComer.AddGestureRecognizer(tap);

            this.NavigationController.Title = "Menú";
        }

        private void CerrarSesion()
        {
            AppDelegate.Auth.CerrarSesion();

            new LoginManager().LogOut();
            AppDelegate.Auth.ErrorMessage = string.Empty;
            //Console.WriteLine(x +"\n"+ y+" " + z);
            PreferencesHelper.SetCredentials(string.Empty, string.Empty);
            PreferencesHelper.SetCredentialsFB(string.Empty, string.Empty);
            //x = PreferencesHelper.GetUser();
            //y = PreferencesHelper.GetPassword();
            //z = PreferencesHelper.IsUserSave();
            this.TabBarController.TabBar.Hidden = true;

            View.Add(loadPop);

            this.timer = NSTimer.CreateScheduledTimer(3, (_) =>
            {
                loadPop.Hide();
            });


            PerformSegue("LOGOUT_SEGUE", this);
            return;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
            {
                FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePic);
            }
            nombreMenu.Text = AppDelegate.Auth.Usuario.NombreCompleto;

        }


        public MenuMas(IntPtr handle) : base(handle)
        {
        }

        public void OpenQuieroDeComer(string p)
        {
            //TODO UPDATE QUIERODECOMER IOS VERSION 
            NSUrl request = new NSUrl("quierodecomer://" + p);

            try
            {
                if (UIApplication.SharedApplication.CanOpenUrl(request))

                    UIApplication.SharedApplication.OpenUrl(request);
                else
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/quierodecomer/id1216419620?mt=8"));
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

        public void QuierodeComer()
        {
            OpenQuieroDeComer("10");
        }
    }
}