using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using MystiqueNative.ViewModels;
using System;

namespace MystiqueNative.Droid.Helpers
{
    /// <summary>
    /// <para> Actividad base con Toolbar y mensajes de diálogos </para>
    /// </summary>
    public class BaseActivity : AppCompatActivity
    {
        private Android.Widget.TextView _labelTotalCarrito;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (!AllowNoConfiguration && MystiqueApp.Config == null)
                RestartMystique();

            if (LayoutResource == 0) return;

            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            _labelTotalCarrito = FindViewById<Android.Widget.TextView>(Resource.Id.carrito_label_total);

            if (Toolbar == null) return;

            SetSupportActionBar(Toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            if (BackButtonIcon != 0) Toolbar.SetNavigationIcon(BackButtonIcon);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!AllowNotLogged && ViewModels.AuthViewModelV2.Instance.Usuario == null)
            {
                RestartMystique();
            }
        }

        public void RestartMystique()
        {
            var i = PackageManager.GetLaunchIntentForPackage(PackageName);
            i.AddFlags(ActivityFlags.NewTask);
            i.AddFlags(ActivityFlags.ClearTask);
            StartActivity(i);
        }

        public Toolbar Toolbar
        {
            get;
            set;
        }

        protected virtual int LayoutResource
        {
            get;
        }

        protected virtual int BackButtonIcon
        {
            get;
        }

        protected int ActionBarIcon
        {
            set => Toolbar?.SetNavigationIcon(value);
        }

        protected virtual bool AllowNotLogged
        {
            get;
        }

        protected virtual bool AllowNoConfiguration
        {
            get;
        }

        protected void UpdateTotalCarrito()
        {
            if (_labelTotalCarrito == null) return;
            _labelTotalCarrito.Text = CarritoViewModel.Instance.SubTotalActual;
        }

        #region MessageDialogImpl
        public void SendMessage(string message, string title = null)
        {
            var activity = this;
            var builder = new AlertDialog.Builder(activity);
            builder
                .SetTitle(title ?? string.Empty)
                .SetMessage(message)
                .SetPositiveButton(Android.Resource.String.Ok, delegate
                {

                });

            activity.RunOnUiThread(() =>
            {
                if (activity.IsFinishing) return;
                var alert = builder.Create();
                alert.Show();
            });
        }


        public void SendToast(string message)
        {
            var activity = this;
            activity.RunOnUiThread(() =>
            {
                if (!activity.IsFinishing)
                {
                    Android.Widget.Toast.MakeText(activity, message, Android.Widget.ToastLength.Long).Show();
                }

            });
        }
        public void SendConfirmation(string message, string title, Action<bool> confirmationAction)
        {
            var activity = this;
            var builder = new AlertDialog.Builder(activity);
            builder
            .SetTitle(title ?? string.Empty)
            .SetMessage(message)
            .SetPositiveButton("Ok", delegate
            {
                confirmationAction(true);
            }).SetNegativeButton("Cancelar", delegate
            {
                confirmationAction(false);
            });

            activity.RunOnUiThread(() =>
            {
                if (!activity.IsFinishing)
                {
                    builder.Create().Show();
                }
            });
        }
        public void SendConfirmation(string message, string title, string okLabel, string cancelLabel, Action<bool> confirmationAction)
        {
            var activity = this;
            var builder = new Android.Support.V7.App.AlertDialog.Builder(activity);
            builder
            .SetTitle(title ?? string.Empty)
            .SetMessage(message)
            .SetPositiveButton(okLabel, delegate
            {
                confirmationAction(true);
            }).SetNegativeButton(cancelLabel, delegate
            {
                confirmationAction(false);
            });

            activity.RunOnUiThread(() =>
            {
                if (!activity.IsFinishing)
                {
                    builder.Create().Show();
                }

            });
        }
        public bool IsInternetAvailable
        {
            get
            {
                var connectionManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                using (var networkInfo = connectionManager.ActiveNetworkInfo)
                {
                    return networkInfo != null && networkInfo.IsConnected;
                }
            }
        }
        public bool ValidateVersion()
        {

            if (MystiqueApp.VersionAndroid == PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName 
                || MystiqueApp.VersionAndroidPruebas == PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}