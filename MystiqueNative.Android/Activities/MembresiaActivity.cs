
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Content.Res;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using Firebase.Analytics;
using MystiqueNative.Droid.Animations;
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingHasbroActivity")]
    public class MembresiaActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_membresia;
        private TextView _labelFolio;
        private TextView _labelNombre;
        private TextView _labelVigencia;
        private TextView _labelVigencia2;
        private TextView _labelCiudad;
        private TextView _labelColonia;
        private FirebaseAnalytics _firebaseAnalytics;
        private LinearLayout _edit;
        private Button _btnQr;
        private Button _btnBarcode;
        private Button _cerrarSesion;
        private ImageButton _btnEdit;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            // SetupMenu();
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
            _btnQr.Click += BtnQR_Click;
            _btnBarcode.Click += BtnBarcode_Click;

        }
        protected override void OnResume()
        {
            base.OnResume();
            if (SupportActionBar != null)
            {
                SupportActionBar.Title = GetString(Resource.String.membresia_title);
            }

            if (ViewModels.AuthViewModelV2.Instance.Usuario != null)
            {
                _labelFolio.Text = "NOMBRE";
                _labelNombre.Text = ViewModels.AuthViewModelV2.Instance.Usuario.NombreCompleto;
                _labelVigencia2.Text = ViewModels.AuthViewModelV2.Instance.Usuario.ExpiracionMembresiaConFormatoEspanyol;
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        private void BtnBarcode_Click(object sender, System.EventArgs e)
        {
            new BarcodeDialogFragment(ViewModels.AuthViewModelV2.Instance.Usuario.GuidMembresia, ZXing.BarcodeFormat.CODE_128).Show(SupportFragmentManager, "barcode_128");
        }

        private void BtnQR_Click(object sender, System.EventArgs e)
        {
            new BarcodeDialogFragment(ViewModels.AuthViewModelV2.Instance.Usuario.GuidMembresia, ZXing.BarcodeFormat.QR_CODE).Show(SupportFragmentManager, "barcode_128");
        }

        private void GrabViews()
        {
            _edit = FindViewById<LinearLayout>(Resource.Id.membresia_btn_edit);
            _cerrarSesion = FindViewById<Button>(Resource.Id.cerrar_sesion_membresia);
            _btnEdit = FindViewById<ImageButton>(Resource.Id.go_to_edit);
            _labelFolio = FindViewById<TextView>(Resource.Id.label_folio);
            _labelNombre = FindViewById<TextView>(Resource.Id.label_nombre);
            _labelVigencia = FindViewById<TextView>(Resource.Id.label_vigencia);
            _labelVigencia2 = FindViewById<TextView>(Resource.Id.label_vigencia2);
            _btnBarcode = FindViewById<Button>(Resource.Id.ic_barcode);
            _btnQr = FindViewById<Button>(Resource.Id.ic_qrcode);
            _labelCiudad = FindViewById<TextView>(Resource.Id.ciudad_texto);
            _labelColonia = FindViewById<TextView>(Resource.Id.colonia_texto);

            if (!string.IsNullOrEmpty(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl))
                ImageService.Instance.LoadUrl(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl)
                        .Retry(retryCount: 5, millisecondDelay: 500)
                        .DownSampleInDip(height: 100)
                        .Transform(new CircleTransformation())
                        .Into(FindViewById<ImageViewAsync>(Resource.Id.img_perfil));
            else
            {
                FindViewById<ImageViewAsync>(Resource.Id.img_perfil).SetImageDrawable(AppCompatResources.GetDrawable(this, Resource.Drawable.account_circle));
            }
            _edit.Click += Edit_Click;
            if (ViewModels.AuthViewModelV2.Instance.Usuario == null) return;

            if (!string.IsNullOrEmpty(ViewModels.AuthViewModelV2.Instance.Usuario.Colonia))
            {
                _labelCiudad.Text = ViewModels.AuthViewModelV2.Instance.Usuario.Colonia;
            }
            if (ViewModels.AuthViewModelV2.Instance.Usuario.CiudadAsInt == 1)
            {
                _labelColonia.Text = "Mexicali";
            }
            else if (ViewModels.AuthViewModelV2.Instance.Usuario.CiudadAsInt == 1)
            {
                _labelColonia.Text = "Tijuana";
            }

            FindViewById<TextView>(Resource.Id.membresia_fnacimiento).Text = ViewModels.AuthViewModelV2.Instance.Usuario.FechaNacimientoConFormatoEspanyol;
            FindViewById<TextView>(Resource.Id.membresia_telefono).Text = ViewModels.AuthViewModelV2.Instance.Usuario.TelefonoConFormato;

            if (ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsInt != 0)
            {
                FindViewById<TextView>(Resource.Id.membresia_sexo).Text = ViewModels.AuthViewModelV2.Instance.Usuario.SexoAsString;
            }
            _btnEdit.Click += _btnEdit_Click;
            _cerrarSesion.Click += (s, e) =>
            {
                SendConfirmation("Usted esta a punto de cerrar su sesión", "Confirmar", "Cerrar Sesión", "Cancelar", ok =>
                {
                    if (ok)
                    {
                        Logout();
                    }
                });
            };

        }

        private void _btnEdit_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(PerfilActivity));
        }

        private void StartRevealActivity(View v)
        {
            int revealX = (int)(v.GetX() + v.Width / 2);
            int revealY = (int)(v.GetY() + v.Height / 2);

            Intent intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.PutExtra(RevealAnimation.ExtraCircularRevealX, revealX);
            intent.PutExtra(RevealAnimation.ExtraCircularRevealY, revealY);
            StartActivity(intent, null);

            OverridePendingTransition(0, 0);
        }
        private void Edit_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(PerfilActivity));
        }
        #region DRAWER

        private Android.Support.V4.Widget.DrawerLayout _drawerLayout;
        private Android.Support.Design.Widget.NavigationView _navigationView;
        private const int NavigationItemId = Resource.Id.nav_item_membership;

        private void SetupMenu()
        {
            _drawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.navigation_view);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //NavigationView.Menu.GetItem(ACTIVITY_INDEX_ON_MENU).SetChecked(true);
            _navigationView.Menu.FindItem(NavigationItemId).SetChecked(true);
            SetupDrawer(_navigationView);
        }
        private async void Logout()
        {
            var bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.Character, ViewModels.AuthViewModelV2.Instance.Usuario.Id);
            bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.MetodosLogin.LOGOUT);
            _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);

            //Com.Crashlytics.Android.Crashlytics.SetUserIdentifier("-1");
            Xamarin.Facebook.Login.LoginManager.Instance.LogOut();
            ViewModels.AuthViewModelV2.Instance.CerrarSesion();
            PreferencesHelper.SetSavedLoginMethod(AuthMethods.NotLogged);
            await Utils.SecureStorageHelper.SetCredentialsAsync("", "");
            ViewModels.AuthViewModelV2.Destroy();
            ViewModels.CitypointsViewModel.Destroy();
            ViewModels.HistorialViewModel.Destroy();
            ViewModels.NotificacionesViewModel.Destroy();
            ViewModels.TwitterLoginViewModel.Destroy();
            RestartMystique();
            //StartActivity(typeof(LoginActivity));
        }
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Android.Resource.Id.Home:
        //            _drawerLayout.OpenDrawer(_navigationView, true);
        //            return true;
        //        default:
        //            return base.OnOptionsItemSelected(item);
        //    }
        //}
        private void SetupDrawer(Android.Support.Design.Widget.NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_item_home:
                        if (NavigationItemId != Resource.Id.nav_item_home)
                            StartActivity(typeof(LandingHasbroActivity));
                        break;
                    case Resource.Id.nav_item_membership:
                        if (NavigationItemId != Resource.Id.nav_item_membership)
                            StartActivity(typeof(MembresiaActivity));
                        break;
                    case Resource.Id.nav_item_notificaciones:
                        if (NavigationItemId != Resource.Id.nav_item_notificaciones)
                            StartActivity(typeof(NotificacionesActivity));
                        break;
                    case Resource.Id.nav_item_historial:
                        if (NavigationItemId != Resource.Id.nav_item_historial)
                            StartActivity(typeof(EstadoCuentaActivity));
                        break;
                    case Resource.Id.nav_item_soporte:
                        if (NavigationItemId != Resource.Id.nav_item_soporte)
                            StartActivity(typeof(SoporteActivity));
                        break;
                    case Resource.Id.nav_item_mi_pedido:
                        if (NavigationItemId != Resource.Id.nav_item_mi_pedido)
                            StartActivity(typeof(LandingHasbroActivity));
                        break;
                    case Resource.Id.nav_item_comentarios:
                        if (NavigationItemId != Resource.Id.nav_item_comentarios)
                            StartActivity(typeof(ComentariosActivity));
                        break;
                }
                _drawerLayout.CloseDrawers();
            };

        }
        #endregion

    }
}