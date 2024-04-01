using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using System;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class RecoverPasswordActivity : BaseActivity
    {
        protected override bool AllowNotLogged => true;
        private Button _btnInitLogin;
        private Button _btnBack;
        private TextInputEditText _entryUsuario;

        private ProgressBar _progressBar;
        //ImageView image;
        protected override int LayoutResource => Resource.Layout.activity_recover_password;
        protected override bool AllowNoConfiguration => true;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //SetContentView(LayoutResource);
            _btnInitLogin = FindViewById<Button>(Resource.Id.btn_recover);
            _btnBack = FindViewById<Button>(Resource.Id.btn_back);
            _entryUsuario = FindViewById<TextInputEditText>(Resource.Id.login_entry_email);
            //image = FindViewById<ImageView>(Resource.Id.login_logo);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            
        }
        protected override void OnResume()
        {
            base.OnResume();
            _btnInitLogin.Click += BtnInitLogin_Click;
            _btnInitLogin.LongClick += BtnInitLogin_LongClick;
            _btnBack.Click += BtnBack_Click;
            ViewModels.AuthViewModelV2.Instance.PropertyChanged += Auth_PropertyChanged;
            ViewModels.AuthViewModelV2.Instance.OnRecuperarContrasenaFinished += Instance_OnRecuperarContrasenaFinished;
        }

        private void BtnInitLogin_LongClick(object sender, View.LongClickEventArgs e)
        {
            string mensaje = string.Format("VC:{0} VN:{2} E:{1}", PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionCode, Configuration.MystiqueApiV2Config.MystiqueAppEmpresa, PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName);
            SendConfirmation(mensaje, "Información de Fresco App", "Crash", "Salir", okLabel =>
             {
                 if (okLabel)
                 {
                     Com.Crashlytics.Android.Crashlytics.Instance.Crash();
                 }
             });
        }

        protected override void OnPause()
        {
            base.OnPause();
            _btnInitLogin.Click -= BtnInitLogin_Click;
            _btnBack.Click -= BtnBack_Click;
            ViewModels.AuthViewModelV2.Instance.PropertyChanged -= Auth_PropertyChanged;
            ViewModels.AuthViewModelV2.Instance.OnRecuperarContrasenaFinished -= Instance_OnRecuperarContrasenaFinished;
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnInitLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_entryUsuario.Text))
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
                        ViewModels.AuthViewModelV2.Instance.RecuperarContrasena(_entryUsuario.Text);
                    }
                    else
                    {
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
                    SendToast(GetString(Resource.String.error_formato_usuario_incorrecto));
                    return;
                }
            }
        }



        private void Instance_OnRecuperarContrasenaFinished(object sender, ViewModels.RecuperarPasswordEventArgs e)
        {
            if (e.Success)
            {
                Finish();
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    SendMessage(e.Message);
                }
            }
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
        private void StartAnimatingLogin()
        {
            _entryUsuario.Enabled = false;

            _btnInitLogin.Enabled = false;
            _btnInitLogin.Alpha = 0.5F;
            _btnBack.Enabled = false;
            _btnBack.Alpha = 0.5F; ;

            _progressBar.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLogin()
        {
            _entryUsuario.Enabled = true;

            _btnInitLogin.Enabled = true;
            _btnInitLogin.Alpha = 1F;
            _btnBack.Enabled = true;
            _btnBack.Alpha = 1F;

            _progressBar.Visibility = ViewStates.Gone;
        }
    }
}