using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Content.Res;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using MystiqueNative.Droid.Animations;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Iniciar Sesión", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class LoginActivity : BaseActivity
    {
        protected override bool AllowNotLogged => true;
        #region VIEWS

        private Button _btnInitLogin;
        private Button _btnRecoverPassword;
        private Button _btnRegistrarse;
        private Button _btnFacebook;
        public TextInputEditText EntryPassword { get; set; }
        private TextInputEditText _entryUsuario;
        private ProgressBar _progressBar;
        private FrameLayout _progressBarHolder;
        private RevealAnimation _mRevealAnimation;
        #endregion
        #region FACEBOOK

        private bool _isFacebookLogin;
        private bool _isEmailLogin;
        private ICallbackManager _callbackManager;
        private readonly List<string> _facebookPermissions = new List<string>() { "email", "public_profile" };
        #endregion
        #region GOOGLE

        private GoogleApiClient _mGoogleApiClient;
        private const int GoogleLoginRequest = 1010;
        #endregion
        #region FIELDS

        private FirebaseAnalytics _firebaseAnalytics;
        #endregion
        protected override int LayoutResource => Resource.Layout.activity_login;
        protected override bool AllowNoConfiguration => true;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            Android.Util.Log.Debug("LoginActivity.OnCreate", "Version Instalada " + PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionCode);
            GrabViews();

            _callbackManager = CallbackManagerFactory.Create();

            _mRevealAnimation = new RevealAnimation(FindViewById<ScrollView>(Resource.Id.scrollview1), Intent, this);
        }
        protected override void OnResume()
        {
            base.OnResume();

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

            _btnInitLogin.Click += BtnInitLogin_Click;
            _btnRecoverPassword.Click += BtnRecoverPassword_Click;
            _btnRegistrarse.Click += BtnRegistrarse_Click;
            // ViewModels.TwitterLoginViewModel.Instance.OnObtenerUrlOAuthFinished += Instance_OnObtenerUrlOAuthFinished;
            ViewModels.AuthViewModelV2.Instance.PropertyChanged += Auth_PropertyChanged;
            ViewModels.AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
        }

        private async void Instance_OnIniciarSesionFinished(object sender, ViewModels.LoginEventArgs e)
        {
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
        private void StartEnterAppIntent()
        {
            var intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }
        //private void Instance_OnObtenerUrlOAuthFinished(object sender, ViewModels.ObtenerUrlOAuthEventArgs e)
        //{
        //    if (e.Success)
        //    {
        //        Intent webviewIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(e.Url));
        //        //webviewIntent.PutExtra(WebViewActivity.PARAMETRO_URL, e.Url);
        //        StartActivity(webviewIntent);
        //    }
        //    else
        //    {
        //        SendToast("Ocurrió un error al contactar los servidores de Twitter ");
        //    }
        //}
        protected override void OnPause()
        {
            base.OnPause();
            _isEmailLogin = false;
            _isFacebookLogin = false;
            ViewModels.AuthViewModelV2.Instance.PropertyChanged -= Auth_PropertyChanged;
            _btnInitLogin.Click -= BtnInitLogin_Click;
            _btnRecoverPassword.Click -= BtnRecoverPassword_Click;
            _btnRegistrarse.Click -= BtnRegistrarse_Click;
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
            Drawable fbicon = AppCompatResources.GetDrawable(this, Resource.Drawable.facebook);

            _btnInitLogin = FindViewById<Button>(Resource.Id.btn_init_login);
            _btnRecoverPassword = FindViewById<Button>(Resource.Id.btn_restore_password);
            _btnRegistrarse = FindViewById<Button>(Resource.Id.btn_registrarse);
            _btnFacebook = FindViewById<Button>(Resource.Id.btn_facebook_connect);
            _btnFacebook.SetCompoundDrawablesWithIntrinsicBounds(fbicon, null, null, null);
            _entryUsuario = FindViewById<TextInputEditText>(Resource.Id.login_entry_email);
            EntryPassword = FindViewById<TextInputEditText>(Resource.Id.login_entry_password);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
        }
        private void BtnRegistrarse_Click(object sender, EventArgs e)
        {
            if (ValidateVersion())
            {
                StartActivity(typeof(RegisterActivity));
            }
            else
            {
                SendMessage(GetString(Resource.String.version_nueva_mensaje));
            }

        }
        private void BtnRecoverPassword_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RecoverPasswordActivity));
        }
        private void Auth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsBusy") return;

            if (ViewModels.AuthViewModelV2.Instance.IsBusy)
            {
                StartAnimatingLogin();
            }
            else
            {
                StopAnimatingLogin();
            }
        }
        private void BtnInitLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_entryUsuario.Text) || string.IsNullOrEmpty(EntryPassword.Text))
            {
                SendToast(GetString(Resource.String.error_ingresar_usuario_password));
                return;
            }
            else
            {
                if (ValidatorHelper.IsValidEmail(_entryUsuario.Text))
                {

                    if (IsInternetAvailable)
                    {
                        var bundle = new Bundle();
                        bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGIN_CORREO);
                        _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);

                        ViewModels.AuthViewModelV2.Instance.IniciarSesion(AuthMethods.Email, _entryUsuario.Text, EntryPassword.Text);
                    }
                    else
                    {
                        if (!IsFinishing)
                            SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                        {
                            if (accept)
                            {
                                StartActivity(new Intent(Android.Provider.Settings.ActionWirelessSettings));
                            }
                        });
                    }
                }
                else
                {
                    if (!IsFinishing)
                        SendToast(GetString(Resource.String.error_formato_usuario_incorrecto));
                    return;
                }
            }
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
    }
}