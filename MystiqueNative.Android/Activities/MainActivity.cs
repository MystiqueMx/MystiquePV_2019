using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Net;
using Android.OS;
using FFImageLoading;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;
using System;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Fresco", Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true, ClearTaskOnLaunch = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionView }, DataScheme = "mystique", Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault })]
    public class MainActivity : BaseActivity
    {
        protected override bool AllowNotLogged => true;
        protected override bool AllowNoConfiguration => true;
        #region Twitter DataScheme

        //private const string OauthTokenKey = "oauth_token";
        //private const string VerifierTokenKey = "oauth_verifier";
        //private string _token;
        //private string _verifier;
        //private bool _isTwitterCallback;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ImageService.Instance.Config.TransformPlaceholders = true;
            UpdateLastLocation();
        }
        protected override void OnResume()
        {
            base.OnResume();
            // ParseQueryParameters();
            var connectionManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            // Instance.OnSolicitarUsuarioTwitterFinished += Instance_OnSolicitarUsuarioTwitterFinished;
            var networkInfo = connectionManager.ActiveNetworkInfo;
            var isOnline = networkInfo != null && networkInfo.IsConnected;

            if (isOnline)
            {
                try
                {
                    var savedLogin = PreferencesHelper.GetSavedLoginMethod();
                    if (savedLogin == AuthMethods.NotLogged)
                    {
                        //StartActivity(typeof(LoginMethodsActivity));
                        ContinueToApp();
                    }
                    else
                    {

                        LoginWithSavedCredentials(savedLogin);
                    }
                }
                catch (Exception ex)
                {
                    PreferencesHelper.SetSavedLoginMethod(AuthMethods.NotLogged);
                    SecureStorageHelper.SetCredentialsAsync("", "");
                    SecureStorageHelper.RemoveAll();
                    RestartMystique();
                    Android.Util.Log.WriteLine(Android.Util.LogPriority.Debug, "FRESCO MainActivity.cs OnResume()", ex.ToString());
                    Console.WriteLine(ex);
                }

            }
            else
            {
                StartActivity(typeof(LoginMethodsActivity));
            }
        }

        private async void ContinueToApp()
        {
            await MystiqueApp.Obtenerconfiguracion();
            StartActivity(typeof(LoginMethodsActivity));
        }

        protected override void OnPause()
        {
            base.OnPause();
            // Instance.OnSolicitarUsuarioTwitterFinished -= Instance_OnSolicitarUsuarioTwitterFinished;
        }
        private void ParseQueryParameters()
        {
            if (Intent.ActionView == Intent.Action)
            {
                //var uri = Intent.Data;
                //_token = uri.GetQueryParameter(OauthTokenKey);
                //_verifier = uri.GetQueryParameter(VerifierTokenKey);
                //_isTwitterCallback = true;
            }
        }
        private async void LoginWithSavedCredentials(AuthMethods savedLoginMethod)
        {
            try
            {
                var user = await SecureStorageHelper.GetUsernameAsync();
                var password = await SecureStorageHelper.GetPasswordAsync();
#if DEBUG
                SendToast("Iniciando sesión como: USER=" + user + ", METHOD=" + savedLoginMethod);
#endif
                if (user != null && password != null)
                {
                    if (await AuthViewModelV2.Instance.IniciarSesion(savedLoginMethod, user, password))
                    {
                        if (ValidateVersion())
                        {
                            StartEnterAppIntent();
                        }
                        else
                        {
                            Intent intent = new Intent(this, typeof(LoginMethodsActivity));
                            intent.PutExtra(LoginMethodsActivity.ExtraErrorVersion, GetString(Resource.String.version_nueva_mensaje));
                            StartActivity(intent);
                        }
                    }
                    else
                    {
                        SecureStorageHelper.SetCredentialsAsync("", "");
                        Intent intent = new Intent(this, typeof(LoginMethodsActivity));
                        intent.PutExtra(LoginMethodsActivity.ExtraErrorMessage, "Error al intentar iniciar sesión en Fresco");
                        StartActivity(intent);
                    }
                }
                else
                {
                    Intent intent = new Intent(this, typeof(LoginMethodsActivity));
                    intent.PutExtra(LoginMethodsActivity.ExtraErrorMessage, "Error al intentar iniciar sesión en Fresco");
                    StartActivity(intent);
                }

            }
            catch (Exception ex)
            {
                //StartActivity(typeof(LoginMethodsActivity));
                PreferencesHelper.SetSavedLoginMethod(AuthMethods.NotLogged);
                await Utils.SecureStorageHelper.SetCredentialsAsync("", "");
                SecureStorageHelper.RemoveAll();
                RestartMystique();
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Debug, "FRESCO MainActivity.cs LoginWithSavedCredentials", ex.ToString());
                Console.WriteLine(ex);
            }
        }
        private void StartEnterAppIntent()
        {
            var intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        private async void UpdateLastLocation()
        {
            if (!PermissionsHelper.ValidatePermissionsForLocation()) return;
            var fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            var location = await fusedLocationProviderClient.GetLastLocationAsync();
            if (location == null) return;
            MystiqueApp.UltimaUbicacionConocida = new MystiqueNative.Models.Location.Point
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }

        //private void Instance_OnSolicitarUsuarioTwitterFinished(object sender, SolicitarUsuarioTwitterEventArgs e)
        //{
        //    var intent = new Intent(this, typeof(LoginMethodsActivity));
        //    if (e.Success)
        //    {
        //        if (string.IsNullOrEmpty(e.User.Email))
        //        {
        //            intent.PutExtra(LoginMethodsActivity.ExtraErrorMessage, "Para iniciar sesión es necesario que verifiques la configuración de tu correo electrónico en tu cuenta de Twitter");
        //        }
        //        else
        //        {
        //            intent.PutExtra(LoginMethodsActivity.ExtraNombre, e.User.Nombre);
        //            intent.PutExtra(LoginMethodsActivity.ExtraPaterno, ".");
        //            intent.PutExtra(LoginMethodsActivity.ExtraUsernameTwitter, e.User.Email);
        //            intent.PutExtra(LoginMethodsActivity.ExtraPasswordTwitter, e.User.Id);
        //        }
        //    }
        //    else
        //    {
        //        intent.PutExtra(LoginMethodsActivity.ExtraErrorMessage, GetString(Resource.String.twttr_failure));
        //    }
        //    StartActivity(intent);
        //}
    }
}