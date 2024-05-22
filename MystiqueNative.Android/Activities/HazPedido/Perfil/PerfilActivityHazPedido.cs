using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location.Places.UI;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.HazPedido.Direccion;
using MystiqueNative.Droid.HazPedido.Historial;
using MystiqueNative.Droid.HazPedido.Ordenes;
using MystiqueNative.Droid.HazPedido.Tarjetas;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Perfil
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask, Label = "Mi perfil")]
    public class PerfilActivityHazPedido : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_perfil;
        protected override int BackButtonIcon => Resource.Drawable.ic_menu_white_24dp;
        //protected override bool DisallowNotLogged => true;
        #endregion

        #region EXPORTS

        public const int RequestDireccion = 6724;
        public const int RequestMaps = 1094;

        public const string ExtraDirection = "com.qdc.PerfilActivity.extradirection";
        #endregion

        #region VIEWS

        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private TextView _labelNombre;
        private TextView _labelEmail;
        private ImageButton _btnEditar;
        private LinearLayout _layoutEditarPassword;

        private ImageViewAsync _headerImage;
        private TextView _headerTitle;
        private View _dividerPassword;
        private ImageButton _buttonAgregarTarjeta;
        private ImageButton _buttonAgregarDireccion;
        private TarjetaPerfilAdapter _adapterTarjetas;
        private DireccionPerfilAdapter _adapterDirecciones;
        private FrameLayout _progressBarHolder;

        #endregion

        #region FIELDS

        private static GoogleApiClient _mGoogleApiClient;
        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();

            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido).SetChecked(true);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            var header = _navigationView.GetHeaderView(0);
            _headerImage = header.FindViewById<ImageViewAsync>(Resource.Id.header_profile_image);
            _headerTitle = header.FindViewById<TextView>(Resource.Id.header_profile_name);

           // var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
           //.RequestEmail()
           //.Build();

           // _mGoogleApiClient = new GoogleApiClient.Builder(this)
           //     .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
           //     .Build();
           // _mGoogleApiClient.Connect();
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _labelNombre = FindViewById<TextView>(Resource.Id.perfil_label_nombre);
            _labelEmail = FindViewById<TextView>(Resource.Id.perfil_label_email);
            _btnEditar = FindViewById<ImageButton>(Resource.Id.perfil_button_editar);
            _layoutEditarPassword = FindViewById<LinearLayout>(Resource.Id.perfil_layout_password);

            _btnEditar.Click += BtnEditar_Click;
            _layoutEditarPassword.Click += LayoutEditarPassword_Click;
            _dividerPassword = FindViewById(Resource.Id.divider);

            _buttonAgregarTarjeta = FindViewById<ImageButton>(Resource.Id.button_agregar_tarjeta);
            _buttonAgregarDireccion = FindViewById<ImageButton>(Resource.Id.button_agregar_direccion);

            var recyclerViewDireccion = FindViewById<RecyclerView>(Resource.Id.recycler_view1);
            var recyclerViewTarjetas = FindViewById<RecyclerView>(Resource.Id.recycler_view2);
            recyclerViewTarjetas.HasFixedSize = true;
            recyclerViewDireccion.HasFixedSize = true;

            _adapterTarjetas = new TarjetaPerfilAdapter(this, ViewModels.TarjetasViewModel.Instance.Tarjetas);
            _adapterDirecciones = new DireccionPerfilAdapter(this, ViewModels.DireccionesViewModel.Instance.Direcciones);

            recyclerViewTarjetas.SetAdapter(_adapterTarjetas);
            recyclerViewDireccion.SetAdapter(_adapterDirecciones);

            ViewModels.TarjetasViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            ViewModels.DireccionesViewModel.Instance.PropertyChanged += Instance_PropertyChanged;

            _buttonAgregarDireccion.Click += ButtonAgregarDireccion_Click;
            _buttonAgregarTarjeta.Click += ButtonAgregarTarjeta_Click;

            _adapterDirecciones.ItemClick += AdapterDirecciones_Button1Click;
            _adapterDirecciones.ItemClick2 += AdapterDirecciones_Button2Click;
            _adapterTarjetas.ItemClick += AdapterTarjetas_Button1Click;
            _adapterTarjetas.ItemClick2 += AdapterTarjetas_Button2Click;
        }


        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsBusy") return;

            if (ViewModels.TarjetasViewModel.Instance.IsBusy || ViewModels.DireccionesViewModel.Instance.IsBusy)
            {
                StartAnimatingLogin();
            }
            if (!ViewModels.TarjetasViewModel.Instance.IsBusy && !ViewModels.DireccionesViewModel.Instance.IsBusy)
            {
                StopAnimatingLogin();
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();
            SetupNavigationView();

            ShowFullMenu();
            //if (QdcApplication.Instance.IsLoggedIn)
            //{
            //    ShowFullMenu();
            //}
            //else
            //{
            //    ShowVisitorMenu();
            //}

            if (ViewModels.TarjetasViewModel.Instance.Tarjetas.Count == 0)
            {
                await ViewModels.TarjetasViewModel.Instance.ObtenerTarjetas();
            }

            if (ViewModels.DireccionesViewModel.Instance.Direcciones.Count == 0)
            {
                ViewModels.DireccionesViewModel.Instance.ObtenerDirecciones();
            }
            ViewModels.TarjetasViewModel.Instance.OnRemoverTarjetaFinished += Instance_OnRemoverTarjetaFinished;
            ViewModels.DireccionesViewModel.Instance.OnEditarDireccionFinished += Instance_OnEditarDireccionFinished;
        }


        protected override void OnPause()
        {
            base.OnPause();
        }

        #endregion

        #region NAVIGATION
        private void SetupNavigationView()
        {
            _navigationView.Menu.FindItem(Resource.Id.nav_item_profile_haz_pedido)
                .SetChecked(true);
            //if (MainApplication.Instance.IsLoggedIn)
            //{
                _headerTitle.Text = $"{ViewModels.AuthViewModelV2.Instance.Usuario.NombreCompleto}";
                _labelNombre.Text = $"{ViewModels.AuthViewModelV2.Instance.Usuario.NombreCompleto}";
                _labelEmail.Text = $"{ViewModels.AuthViewModelV2.Instance.Usuario.Email}";
                _layoutEditarPassword.Visibility = MystiqueApp.Usuario.AuthMethod == AuthMethods.Email ? ViewStates.Visible : ViewStates.Gone;
                _dividerPassword.Visibility = MystiqueApp.Usuario.AuthMethod == AuthMethods.Email ? ViewStates.Visible : ViewStates.Gone;
            //}

            ImageService.Instance
                .LoadUrl(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl)
                .DownSampleInDip(100)
                .Transform(new FFImageLoading.Transformations.CircleTransformation())
                .Into(_headerImage);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer((int)GravityFlags.Start);
                    return true;
                default:
                    return true;
            }
        }
        private void ShowVisitorMenu()
        {
            _navigationView.Menu.FindItem(Resource.Id.nav_item_profile_haz_pedido).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(false);

            //_navigationView.Menu.FindItem(Resource.Id.nav_item_login).SetVisible(true);
        }

        private void ShowFullMenu()
        {
            _navigationView.Menu.FindItem(Resource.Id.nav_item_profile_haz_pedido).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(true);

            //_navigationView.Menu.FindItem(Resource.Id.nav_item_login).SetVisible(false);
        }


        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var tag = string.Empty;
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_item_go_home_haz_pedido:
                    Intent intent = new Intent(this, typeof(LandingHasbroActivity));
                    intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);
                    break;
                //case Resource.Id.nav_item_login:
                //    StartActivity(typeof(LoginSocialActivity));
                //    OverridePendingTransition(0, 0);
                //    break;
                case Resource.Id.nav_item_cart_haz_pedido:
                    StartActivity(typeof(Carrito.CarritoActivity));
                    OverridePendingTransition(0, 0);
                    break;
                //case Resource.Id.nav_item_logout:
                //    SendConfirmation("Está seguro que desea salir de la aplicación", "Cerrar Sesión", "Confirmar", "Salir",
                //        ok =>
                //        {
                //            if (!ok) return;
                //            PreferencesHelper.SetSavedLoginMethod(AuthMethods.NotLogged);
                //            ViewModels.AuthViewModel.Instance.CerrarSesion();
                //            _drawerLayout.CloseDrawer((int)GravityFlags.Start, true);
                //            Auth.GoogleSignInApi.SignOut(_mGoogleApiClient);
                //            FinishAffinity();
                //        });
                //    break;
                case Resource.Id.nav_item_help_haz_pedido:
                    StartActivity(typeof(SoporteActivity));
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.nav_item_profile_haz_pedido:
                    StartActivity(typeof(PerfilActivityHazPedido));
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.nav_item_room_haz_pedido:

                    StartActivity(MystiqueApp.PedidosActivos == 1
                        ? typeof(OrdenActivaActivity)
                        : typeof(OrdenesActivasActivity));
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.nav_item_history_haz_pedido:
                    StartActivity(typeof(HistorialActivity));
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.nav_item_notifications_haz_pedido:
                    StartActivity(typeof(Soporte.NotificacionesHazPedidoActivity));
                    OverridePendingTransition(0, 0);
                    break;
                //case Resource.Id.nav_item_directory: 
                //    StartActivity(typeof(DirectorioActivity));
                //    OverridePendingTransition(0,0);
                //    break;
                default:
                    StartActivity(typeof(HomeHazPedidoActivity));
                    OverridePendingTransition(0, 0);
                    break;
            }
            _drawerLayout.CloseDrawer((int)GravityFlags.Start, true);
        }

        #endregion

        private void LayoutEditarPassword_Click(object sender, System.EventArgs e)
        {
            //StartActivityForResult(typeof(PerfilPasswordActivity), PerfilPasswordActivity.UpdatePasswordRequest);
        }

        private void BtnEditar_Click(object sender, System.EventArgs e)
        {
            //StartActivityForResult(typeof(PerfilEditarActivity), PerfilEditarActivity.UpdateProfileRequest);
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case RequestMaps:
                    HandleMapsResult(resultCode, data);
                    break;
                case EdicionDireccionActivity.RequestNuevaDireccion:
                    if (resultCode == Result.Ok) SendToast("La dirección fué agregada a tus destinos de reparto");
                    break;
                case EdicionTarjetaActivity.RequestAgregarTarjeta:
                    if (resultCode == Result.Ok) SendToast("La tarjeta fué agregada a tus métodos de pago");
                    await TarjetasViewModel.Instance.ObtenerTarjetas();
                    break;
                default: break;
            }
        }

        private void HandleMapsResult(Result resultCode, Intent data)
        {
            switch (resultCode)
            {
                case Result.Canceled:
                    break;
                case Result.FirstUser:
                    break;
                case Result.Ok:
                    var place = PlacePicker.GetPlace(this, data);
                    //var toastMsg = $"NameFormatted: {place.NameFormatted}, AddressFormatted: {place.AddressFormatted}, LatLng:({place.LatLng.Latitude}, {place.LatLng.Longitude}), PhoneNumberFormatted: {place.PhoneNumberFormatted}, AttributionsFormatted:{place.AttributionsFormatted}";
                    //SendToast(toastMsg);
                    var intent = new Intent(this, typeof(EdicionDireccionActivity));
                    intent.PutExtra(EdicionDireccionActivity.ExtraLatitud, place.LatLng.Latitude);
                    intent.PutExtra(EdicionDireccionActivity.ExtraLongitud, place.LatLng.Longitude);
                    StartActivityForResult(intent, EdicionDireccionActivity.RequestNuevaDireccion);
                    OverridePendingTransition(0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultCode), resultCode, null);
            }
        }

        #region LOADING
        private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 100
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }

        private void StopAnimatingLogin()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 100
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion

        #region RECYCLERVIEWS


        private void AdapterTarjetas_Button2Click(object sender, RecyclerClickEventArgs e)
        {
            var item = TarjetasViewModel.Instance.Tarjetas[e.Position];
            SendConfirmation("¿Estás seguro que deseas eliminar la tarjeta?", "", "Eliminar", "Cancelar", async ok =>
            {
                if (!ok) return;
                await TarjetasViewModel.Instance.RemoverTarjeta(item.Id);
            });
        }

        private void AdapterTarjetas_Button1Click(object sender, RecyclerClickEventArgs e)
        {
        }

        private void ButtonAgregarTarjeta_Click(object sender, System.EventArgs e)
        {
            StartActivityForResult(typeof(EdicionTarjetaActivity), EdicionTarjetaActivity.RequestAgregarTarjeta);
            OverridePendingTransition(0, 0);
        }

        private void AdapterDirecciones_Button2Click(object sender, RecyclerClickEventArgs e)
        {
            var item = DireccionesViewModel.Instance.Direcciones[e.Position];
            SendConfirmation("¿Estás seguro que deseas eliminar la dirección?", "", "Eliminar", "Cancelar", async ok =>
            {
                if (!ok) return;
                await DireccionesViewModel.Instance.EliminarDireccion(item);
            });

        }

        private void AdapterDirecciones_Button1Click(object sender, RecyclerClickEventArgs e)
        {
            var item = DireccionesViewModel.Instance.Direcciones[e.Position];
            var intent = new Intent(this, typeof(EdicionDireccionActivity));
            intent.PutExtra(EdicionDireccionActivity.ExtraIdEdicion, item.Id);
            StartActivity(intent);
            OverridePendingTransition(0, 0);
        }

        private void ButtonAgregarDireccion_Click(object sender, System.EventArgs e)
        {
            //TODO
            //SendConfirmation("¿Desea agregar su ubicación actual en Mis Direcciones?", "Mi ubicación", "Si", "No", async ok =>
            //{
            //    if (!ok) return;
            //    var intent = new Intent(this, typeof(EdicionDireccionActivity));
            //    intent.PutExtra(EdicionDireccionActivity.ExtraLatitud, MystiqueApp.UltimaUbicacionConocida.Latitude);
            //    intent.PutExtra(EdicionDireccionActivity.ExtraLongitud, MystiqueApp.UltimaUbicacionConocida.Longitude);
            //    StartActivityForResult(intent, EdicionDireccionActivity.RequestNuevaDireccion);
            //    OverridePendingTransition(0, 0);
            //});
            var builder = new PlacePicker.IntentBuilder();
            StartActivityForResult(builder.Build(this), RequestMaps);
            OverridePendingTransition(0, 0);
        }

        private void Instance_OnEditarDireccionFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (!e.Success && !string.IsNullOrEmpty(e.Message))
            {
                SendMessage(e.Message);
            }
        }

        private void Instance_OnRemoverTarjetaFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (!e.Success && !string.IsNullOrEmpty(e.Message))
            {
                SendMessage(e.Message);
            }
        }
        #endregion
    }
}