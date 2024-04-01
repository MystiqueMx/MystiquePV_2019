using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Iniciar Sesión", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginMethodsActivity : BaseActivity, IFacebookCallback, GoogleApiClient.IOnConnectionFailedListener
    {
        protected override bool AllowNotLogged => true;
        #region INTENT PARAMS
        public string MsgErrorLogin;
        public string MsgErrorVersion;
        public string UsrnLogin;
        public string PassLogin;
        public string NombreLogin;
        public string PaternoLogin;

        public bool IsTwitterLogin => !string.IsNullOrEmpty(UsrnLogin);
        public bool StartupHadError => !string.IsNullOrEmpty(MsgErrorLogin);
        #endregion
        #region INTENT EXTRAS
        public const string ExtraErrorMessage = "MSG_ERROR_LOGIN";
        public const string ExtraUsernameTwitter = "USRN_LOGIN";
        public const string ExtraErrorVersion = "MSG_ERROR_VERSION";
        public const string ExtraPasswordTwitter = "PASS_LOGIN";
        public const string ExtraNombre = "NOMBRE_LOGIN";
        public const string ExtraPaterno = "PATERNO_LOGIN";
        #endregion
        #region VIEWS
        Button _btnEmail, _btnGoogle, _btnTwitter, _btnFacebook;
        FrameLayout _progressBarHolder;
        #endregion
        #region FACEBOOK
        bool _isFacebookLogin;
        bool _isEmailLogin;
        ICallbackManager _callbackManager;
        readonly List<string> _facebookPermissions = new List<string>() { "email", "public_profile" };
        #endregion
        #region GOOGLE
        GoogleApiClient _mGoogleApiClient;
        private string _mensajeError;
        private string _mensajeVersionError;
        private const int GoogleLoginRequest = 1010;
        #endregion
        #region FIELDS
        FirebaseAnalytics _firebaseAnalytics;
        #endregion

        protected override int LayoutResource => Resource.Layout.activity_login_methods;
        protected override bool AllowNoConfiguration => true;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            GrabViews();

            _callbackManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(_callbackManager, this);
        }

        private void ParseIntentParameters()
        {
            MsgErrorLogin = Intent.GetStringExtra(ExtraErrorVersion);
            MsgErrorVersion = Intent.GetStringExtra(ExtraErrorMessage);
            UsrnLogin = Intent.GetStringExtra(ExtraUsernameTwitter);
            PassLogin = Intent.GetStringExtra(ExtraPasswordTwitter);
            NombreLogin = Intent.GetStringExtra(ExtraNombre);
            PaternoLogin = Intent.GetStringExtra(ExtraPaterno);
        }

        protected override void OnResume()
        {
            base.OnResume();

            ParseIntentParameters();

            _mensajeError = Intent.GetStringExtra(ExtraErrorMessage);
            if (!string.IsNullOrEmpty(_mensajeError))
            {
                SendMessage(_mensajeError);
                _mensajeError = string.Empty;
            }

            _mensajeVersionError = Intent.GetStringExtra(ExtraErrorVersion);
            if (!string.IsNullOrEmpty(_mensajeVersionError))
            {
                OpenPlaystore();
                _mensajeVersionError = string.Empty;
            }

            if (!IsInternetAvailable)
            {
                SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                {
                    if (accept)
                    {
                        StartActivity(new Intent(Android.Provider.Settings.ActionWirelessSettings));
                    }
                });
            }


            //if (IsTwitterLogin) TwitterLogin();
            TwitterLoginViewModel.Instance.OnObtenerUrlOAuthFinished += Instance_OnObtenerUrlOAuthFinished;
            AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
            AuthViewModelV2.Instance.OnRegistrarFailed += Instance_OnRegistrarFailed;
            _btnFacebook.Click += BtnFacebook_Click;
            _btnGoogle.Click += BtnGoogle_Click;
            _btnTwitter.Click += BtnTwitter_Click;
            _btnEmail.Click += BtnEmail_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();

            TwitterLoginViewModel.Instance.OnObtenerUrlOAuthFinished -= Instance_OnObtenerUrlOAuthFinished;
            AuthViewModelV2.Instance.OnIniciarSesionFinished -= Instance_OnIniciarSesionFinished;
            AuthViewModelV2.Instance.OnRegistrarFailed -= Instance_OnRegistrarFailed;
            _btnFacebook.Click -= BtnFacebook_Click;
            _btnGoogle.Click -= BtnGoogle_Click;
            _btnTwitter.Click -= BtnTwitter_Click;
            _btnEmail.Click -= BtnEmail_Click;
        }
        private void Instance_OnObtenerUrlOAuthFinished(object sender, ViewModels.ObtenerUrlOAuthEventArgs e)
        {
            if (e.Success)
            {
                Intent webviewIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(e.Url));
                StartActivity(webviewIntent);
            }
            else
            {
                SendToast(GetString(Resource.String.methods_btn_twitter));
            }
            StopAnimatingLogin();
        }

        private async void BtnFacebook_Click(object sender, EventArgs e)
        {

            StartAnimatingLogin();
            InitFacebookLogin();
        }
        private void Instance_OnRegistrarFailed(object sender, RegistrarEventArgs e)
        {
            SendMessage(e.Message);
            StopAnimatingLogin();
        }
        private void BtnEmail_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        private void BtnTwitter_Click(object sender, EventArgs e)
        {
            StartAnimatingLogin();
            TwitterLoginViewModel.Instance.ObtenerUrlOAuth();
        }

        private void BtnGoogle_Click(object sender, EventArgs e)
        {
            StartAnimatingLogin();
            InitGoogleLogin();
        }

        private async void Instance_OnIniciarSesionFinished(object sender, LoginEventArgs e)
        {
            StopAnimatingLogin();
            if (e.Success)
            {
                StartEnterAppIntent();
                PreferencesHelper.SetSavedLoginMethod(e.Method);
                await SecureStorageHelper.SetCredentialsAsync(e.Username, e.Password);
            }
            else
            {
                SendMessage(e.Message);
            }
        }
        private void InitFacebookLogin()
        {
            StartAnimatingLogin();
            LoginManager.Instance.LogInWithReadPermissions(this, _facebookPermissions);
        }
        private void InitGoogleLogin()
        {
            if (_mGoogleApiClient == null)
            {
                GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail()
                .Build();
                _mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
            }
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(_mGoogleApiClient);
            StartActivityForResult(signInIntent, GoogleLoginRequest);
        }
        private void OpenPlaystore()
        {
            SendConfirmation(GetString(Resource.String.version_nueva_mensaje), "Nueva actualización disponible", "Ir a la tienda", "Cerrar", ok =>
            {
                if (ok)
                {
                    try
                    {
                        StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.GrupoRed.Fresco" + Application.Context.ApplicationContext.PackageName)));
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine(e.Message);
#endif
                        StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(@"https://play.google.com/store/apps/details?id=com.GrupoRed.Fresco" + Application.Context.ApplicationContext.PackageName)));
                    }
                }
            });
        }

        private void GrabViews()
        {
            _btnEmail = FindViewById<Button>(Resource.Id.methods_btn_email);
            _btnFacebook = FindViewById<Button>(Resource.Id.methods_btn_facebook);
            _btnGoogle = FindViewById<Button>(Resource.Id.methods_btn_google);
            _btnTwitter = FindViewById<Button>(Resource.Id.methods_btn_twitter);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
        }
        private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLogin()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        private void StartEnterAppIntent()
        {
            var intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            Finish();
        }
        private async void FacebookLogin()
        {
            try
            {
                FacebookUserRequest req = new FacebookUserRequest();
                req.MakeUserRequest();
                MystiqueApp.FacebookId = AccessToken.CurrentAccessToken.UserId;
                MystiqueApp.FbProfile = req.Profile;
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGIN_FACEBOOK);
                _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);
                AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Facebook, req.Profile.Email, MystiqueApp.FacebookId, Profile.CurrentProfile.FirstName, Profile.CurrentProfile.LastName);
            }
            catch (Exception e)
            {
                StopAnimatingLogin();
            }
            //SendMessage(string.Format("{0} {1} {2} {3} {4}",AuthMethods.Facebook, req.Profile.Email, MystiqueApp.FacebookId, Profile.CurrentProfile.FirstName, Profile.CurrentProfile.LastName));

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            _callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == GoogleLoginRequest)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                GoogleLogin(result);
            }

        }
        public void GoogleLogin(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                var acct = result.SignInAccount;
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGIN_GOOGLE);
                _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);
                AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Google, acct.Email, acct.Id, acct.GivenName, acct.FamilyName);
                //SendMessage(string.Format("{0} {1} {2} {3} {4}", AuthMethods.Google, acct.Email, acct.Id, acct.GivenName, acct.FamilyName));
            }
            else
            {
                SendToast(GetString(Resource.String.google_cancelled));
                StopAnimatingLogin();
            }
        }
        public void TwitterLogin()
        {
            StartAnimatingLogin();
            var bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGIN_TWITTER);
            _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);
            AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Twitter, UsrnLogin, PassLogin, NombreLogin, PaternoLogin);
            //SendMessage(string.Format("{0} {1} {2} {3} {4}", AuthMethods.Twitter, USRN_LOGIN, PASS_LOGIN, NOMBRE_LOGIN, PATERNO_LOGIN));
        }
        #region IFacebookCallback
        public void OnCancel()
        {
            StopAnimatingLogin();
            if (!IsFinishing)
                SendToast(GetString(Resource.String.fb_cancelled));
        }

        public void OnError(FacebookException error)
        {
            StopAnimatingLogin();
            if (!IsFinishing)
                SendToast(GetString(Resource.String.fb_cancelled));
#if DEBUG
            Console.WriteLine(error.Message);
#endif
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            FacebookLogin();
        }
        #endregion
        #region IOnConnectionFailedListener
        public void OnConnectionFailed(ConnectionResult result)
        {
            StopAnimatingLogin();
            SendToast(GetString(Resource.String.google_cancelled));
        }
        #endregion
    }
}