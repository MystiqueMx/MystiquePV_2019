
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using MystiqueNative.Droid.Helpers;
using Android.Content;
using Android.Graphics.Drawables;
using MystiqueNative.Droid.Views;
using Android.Support.V7.Content.Res;
using Android.Support.V7.Preferences;
using Android.Content.PM;
using Android.Runtime;
using System;
using ZXing.Mobile;
using Android.Widget;
using Android.Views;
using Firebase.Analytics;
using MystiqueNative.Helpers.Analytics;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class LandingHasbroActivity : BaseActivity
    {
        #region Views
        AppCompatImageButton ic_notificaciones;
        //TextView hasbroCitypoints;
        FrameLayout progressBarHolder;
        #endregion
        #region Fields
        FirebaseAnalytics firebaseAnalytics;
        #endregion
        protected override int LayoutResource => Resource.Layout.activity_landing_hasbro;

        public bool IsReturning { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Com.Crashlytics.Android.Crashlytics.SetUserIdentifier(ViewModels.AuthViewModelV2.Instance.Usuario.Id);
            Android.Support.V7.App.AppCompatDelegate.CompatVectorFromResourcesEnabled = true;
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            FindViews();

            MainApplication.ViewModelNotificaciones = new ViewModels.NotificacionesViewModel();
            SupportActionBar.SetDisplayShowHomeEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            //SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.home);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (PreferenceManager.GetDefaultSharedPreferences(this).GetBoolean("pref_zcs", false))
                TryInitZonaCitySalads();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (SupportActionBar != null)
                SupportActionBar.Title = GetString(Resource.String.title_hasbro);

            ic_notificaciones.SetImageDrawable(SetBadgeCount(Resource.Drawable.bellWithBadge, MainApplication.ViewModelNotificaciones.NotificacionesNuevas));

            MainApplication.ViewModelNotificaciones.OnObtenerNotificacionesFinished += ViewModelNotificaciones_OnObtenerNotificacionesFinished;
            MainApplication.ViewModelCitypoints.OnAgregarPuntosFinished += ViewModelCitypoints_OnAgregarPuntosFinished;
            MainApplication.ViewModelCitypoints.OnEstadoCuentaFinished += ViewModelCitypoints_OnEstadoCuentaFinished;
            if (!MainApplication.ViewModelCitypoints.IsBusy)
            {
                MainApplication.ViewModelCitypoints.ObtenerEstadoCuenta();
            }

            if (!MainApplication.ViewModelNotificaciones.IsBusy)
                MainApplication.ViewModelNotificaciones.ObtenerNotificaciones();

            Title = GetString(Resource.String.title_hasbro);
            ScanCodeAfterReturn();
        }

        private void ScanCodeAfterReturn()
        {
            if (IsReturning)
            {
                if (PermissionsHelper.ValidatePermissionsForCamera())
                {
                    ReadQRCode();
                }
                else
                {
                    RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestCameraId);
                }
                IsReturning = false;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            MainApplication.ViewModelNotificaciones.OnObtenerNotificacionesFinished -= ViewModelNotificaciones_OnObtenerNotificacionesFinished;
            MainApplication.ViewModelCitypoints.OnAgregarPuntosFinished -= ViewModelCitypoints_OnAgregarPuntosFinished;
            MainApplication.ViewModelCitypoints.OnEstadoCuentaFinished -= ViewModelCitypoints_OnEstadoCuentaFinished;
        }
        private void ViewModelCitypoints_OnEstadoCuentaFinished(object sender, ViewModels.EstadoCuentaArgs e)
        {
            if (e.EstadoCuenta != null && e.EstadoCuenta.Success)
            {
                //TODO descomentar si en vista se descomento este elemento
                //hasbroCitypoints.Text = string.Format("Mi saldo : {0} pts", e.EstadoCuenta.PuntosAsInt);
            }
        }

        private void ViewModelCitypoints_OnAgregarPuntosFinished(object sender, ViewModels.AgregarPuntosArgs e)
        {
            StopLoadingAnimation();
            if (e.Success)
            {
                SendMessage("Los puntos se agregaron exitosamente");
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    SendMessage(e.Message);
                    MainApplication.ViewModelCitypoints.ErrorMessage = string.Empty;
                }
            }
        }

        private void TryInitZonaCitySalads()
        {
            bool IsLocationEnabled = PermissionsHelper.ValidatePermissionsForLocation();
            if (IsLocationEnabled)
            {
                //Intent i = new Intent(this, typeof(ZonaCitySaladsService));
                //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                //    StartForegroundService(i);
                //else
                //    StartService(i);
            }
            else
            {
                RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestLocationId);
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            switch (requestCode)
            {
                case PermissionsHelper.RequestCameraId:
                    bool GotRequestedPermission = true;
                    foreach (Permission p in grantResults)
                        if (p != Permission.Granted)
                            GotRequestedPermission = false;
                    if (GotRequestedPermission)
                    {
                        ReadQRCode();
                    }
                    break;
                case PermissionsHelper.RequestLocationId:
                    bool GotRequestedPermission2 = true;
                    foreach (Permission p in grantResults)
                        if (p != Permission.Granted)
                            GotRequestedPermission2 = false;

                    bool IsLocationEnabled = GotRequestedPermission2;
                    if (IsLocationEnabled)
                    {
                        //Intent i = new Intent(this, typeof(ZonaCitySaladsService));
                        //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        //    StartForegroundService(i);
                        //else
                        //    StartService(i);
                    }
                    else
                    {
                        var prefedit = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
                        prefedit.PutBoolean("pref_zcs", false);
                        prefedit.Commit();
                    }
                    break;
                default:
                    break;
            }
        }

        private void ViewModelNotificaciones_OnObtenerNotificacionesFinished(object sender, ViewModels.ObtenerNotificacionesArgs e)
        {
            if (e.Success)
            {
                ic_notificaciones.SetImageDrawable(SetBadgeCount(Resource.Drawable.bellWithBadge, e.NotificacionesNuevas));
            }
        }

        #region Find Views
        private void FindViews()
        {
            progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            ic_notificaciones = FindViewById<AppCompatImageButton>(Resource.Id.ic_notificaciones);
            ic_notificaciones.SetImageDrawable(SetBadgeCount(Resource.Drawable.bellWithBadge, 0));

            //TODO descomentar si en vista se descomento este elemento
            //hasbroCitypoints = FindViewById<TextView>(Resource.Id.hasbro_citypoints);

            ic_notificaciones.Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_NOTIFICACIONES);

                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);

                StartActivity(typeof(NotificacionesActivity));
            };

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_beneficios).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_PROMOCIONES);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);

                StartActivity(typeof(BeneficiosActivity));
            };
            */

            FindViewById<CardView>(Resource.Id.btn_clientes).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_CLIENTES);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);

                StartActivity(typeof(Activities.ClienteRegistroActivity));
            };

            FindViewById<CardView>(Resource.Id.btn_qdc).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_QDC);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);

                //StartActivity(typeof(HazPedido.HomeHazPedidoActivity));
                StartActivity(typeof(Activities.ClientesActivity));

                //Intent intent = Application.Context.PackageManager.GetLaunchIntentForPackage(GetString(Resource.String.qdc_package_uri));
                //if (intent != null)
                //{
                //    intent.PutExtra("p", MystiqueApp.IdQDC);
                //    intent.AddFlags(ActivityFlags.NewTask);
                //    StartActivity(intent);
                //}
                //else
                //{
                //    intent = new Intent(Intent.ActionView);
                //    intent.AddFlags(ActivityFlags.NewTask);
                //    intent.SetData(Android.Net.Uri.Parse(GetString(Resource.String.qdc_market_link)));
                //    StartActivity(intent);
                //}
            };

            //TODO descomentar si en vista se descomento este elemento
            //FindViewById<CardView>(Resource.Id.btn_wallet).Click += CapturaPuntos_ClickAsync;

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_citypoints).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_HISTORIAL_PUNTOS);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);

                StartActivity(typeof(EstadoCuentaActivity));
            };
            */

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_mi_perfil).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_RECOMPENSAS);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);
                StartActivity(typeof(RecompensasActivity));
            };
            */

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_membresia).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_MEMBRESIA);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);
                StartActivity(typeof(MembresiaActivity));
            };
            */

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_acerca_de).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_AYUDA);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);
                StartActivity(typeof(SoporteActivity));
            };
            */

            //TODO descomentar si en vista se descomento este elemento
            /*
            FindViewById<CardView>(Resource.Id.btn_comentarios).Click += (s, e) =>
            {
                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
                bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Flujos.FLUJO_BOTON_COMENTARIOS);
                firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.ViewItem, bundle);
                StartActivity(typeof(ComentariosActivity));
            };
            */

        }
        private Drawable SetBadgeCount(int res, int badgeCount)
        {
            LayerDrawable icon = (LayerDrawable)AppCompatResources.GetDrawable(this, Resource.Drawable.bellWithBadge);
            Drawable mainIcon = AppCompatResources.GetDrawable(this, res);
            BadgeDrawable badge = new BadgeDrawable(this);
            badge.SetCount(badgeCount);
            icon.Mutate();
            icon.SetDrawableByLayerId(Resource.Id.ic_badge, badge);
            icon.SetDrawableByLayerId(Resource.Id.ic_main_icon, mainIcon);
            return icon;
        }
        #endregion

        #region CAPTURA PUNTOS
        private void CapturaPuntos_ClickAsync(object sender, EventArgs e)
        {
            if (IsInternetAvailable)
            {
                if (ViewModels.AuthViewModelV2.Instance.Usuario.RegistroCompleto)
                {
                    if (PermissionsHelper.ValidatePermissionsForCamera())
                    {
                        ReadQRCode();
                    }
                    else
                    {
                        RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestCameraId);
                    }
                }
                else
                {
                    SendConfirmation(GetString(Resource.String.error_registro_incompleto), "", "Completar ahora", "Ahora no", accept =>
                    {
                        if (accept)
                        {
                            Intent intent = new Intent(this, typeof(Activities.UpdateProfileActivity));
                            StartActivityForResult(intent, Activities.UpdateProfileActivity.UpdateRequestCode);
                        }
                    });
                }

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
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            IsReturning = requestCode == Activities.UpdateProfileActivity.UpdateRequestCode
                && resultCode == Result.Ok;
            base.OnActivityResult(requestCode, resultCode, data);
        }
        private void ReadQRCode()
        {
            StartActivity(typeof(SumarPuntosActivity));
        }
        #endregion
        #region LOADING
        public void StartLoadingAnimation()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = inAnimation;
            progressBarHolder.Visibility = ViewStates.Visible;
        }
        public void StopLoadingAnimation()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = outAnimation;
            progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion
    }
}