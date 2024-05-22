using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.HazPedido.Historial;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Models.Orden;
using System;
using System.Linq;

namespace MystiqueNative.Droid.HazPedido.Ordenes
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Mi orden activa")]
    public class OrdenActivaActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_orden_activa;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS

        public const string ExtraIdOrden = "QDC.Extraidordenactiva";
        #endregion

        #region VIEWS

        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private ImageViewAsync _headerImage;
        private TextView _headerTitle;
        private ImageView _imagenPago;

        private TextView _labelRestaurante;
        private TextView _labelFolio;
        private TextView _labelFecha;
        private TextView _labelTotal;
        private TextView _labelEstatus;
        private TextView _labelPago;

        private RecyclerView _recyclerView;
        private ImageView _imagenEstatus;

        private Button _buttonVerDetalle;
        private Button _buttonEnviarMensaje;

        private FrameLayout _progressBarHolder;
        #endregion

        #region FIELDS

        private bool _isRefreshing;
        private int _idPedidoActual;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private static GoogleApiClient _mGoogleApiClient;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            GrabIntentParameters();
            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido).SetChecked(true);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            var header = _navigationView.GetHeaderView(0);
            _headerImage = header.FindViewById<ImageViewAsync>(Resource.Id.header_profile_image);
            _headerTitle = header.FindViewById<TextView>(Resource.Id.header_profile_name);

            StartAnimating();
            SetSelectedItem();


            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestEmail()
            .Build();

            _mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .Build();
            _mGoogleApiClient.Connect();
        }

        private void GrabIntentParameters()
        {
            _idPedidoActual = Intent.GetIntExtra(ExtraIdOrden, -1);
        }

        private void GrabViews()
        {
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _labelEstatus = FindViewById<TextView>(Resource.Id.orden_activa_label_estatus);
            _labelFecha = FindViewById<TextView>(Resource.Id.orden_activa_label_fecha);
            _labelFolio = FindViewById<TextView>(Resource.Id.orden_activa_label_folio);
            _labelPago = FindViewById<TextView>(Resource.Id.orden_activa_label_pago);
            _labelRestaurante = FindViewById<TextView>(Resource.Id.orden_activa_label_restaurante);
            _labelTotal = FindViewById<TextView>(Resource.Id.orden_activa_label_total);

            _imagenPago = FindViewById<ImageView>(Resource.Id.orden_activa_imagen_pago);

            _imagenEstatus = FindViewById<ImageView>(Resource.Id.orden_activa_image_estatus);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _buttonVerDetalle = FindViewById<Button>(Resource.Id.button_ver);
            _buttonEnviarMensaje = FindViewById<Button>(Resource.Id.button_mensaje);

            _buttonVerDetalle.Click += ButtonVerDetalle_Click;
            _buttonEnviarMensaje.Click += ButtonEnviarMensaje_Click;
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
            await ViewModels.PedidosViewModel.Instance.ObtenerPedidosActivos();
            ViewModels.PedidosViewModel.Instance.OnObtenerDetallePedidoFinished += Instance_OnObtenerDetallePedidoFinished;
            _toolbar.SetNavigationIcon(MystiqueApp.PedidosActivos == 1
                ? Resource.Drawable.ic_menu_white_24dp
                : Resource.Drawable.ic_chevron_left_white_24dp);

            if (ViewModels.PedidosViewModel.Instance.OrdenSeleccionada == null || _isRefreshing) return;
            UpdateUi(ViewModels.PedidosViewModel.Instance.OrdenSeleccionada);
            StartUpdates();


        }

        protected override void OnPause()
        {
            base.OnPause();
            StopUpdates();
            ViewModels.PedidosViewModel.Instance.OnObtenerDetallePedidoFinished -= Instance_OnObtenerDetallePedidoFinished;
        }

        #endregion

        #region NAVIGATION
        private void SetupNavigationView()
        {
            _navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido)
                .SetChecked(true);
            //_headerTitle.Text = $"{MystiqueApp.Usuario.Username}";
            _headerTitle.Text = $"{ViewModels.AuthViewModelV2.Instance.Usuario.NombreCompleto}";
            //if (MainApplication.Instance.IsLoggedIn)
            //{
            //    _headerTitle.Text = $"{MystiqueApp.Usuario.Username}";
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
                    if (MystiqueApp.PedidosActivos == 1)
                    {
                        _drawerLayout.OpenDrawer((int)GravityFlags.Start);
                    }
                    else
                    {
                        OnBackPressed();
                    }
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
                    StartActivity(typeof(Perfil.PerfilActivityHazPedido));
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
                //    OverridePendingTransition(0, 0);
                //    break;
                default:
                    StartActivity(typeof(HomeHazPedidoActivity));
                    OverridePendingTransition(0, 0);
                    break;
            }
            _drawerLayout.CloseDrawer((int)GravityFlags.Start, true);
        }

        #endregion

        #region LOADING

        private void StartAnimating()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimating()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }

        #endregion

        private async void SetSelectedItem()
        {
            if (ViewModels.PedidosViewModel.Instance.OrdenesActivas.Count == 0)
            {
                await ViewModels.PedidosViewModel.Instance.ObtenerPedidosActivos();
            }
            if (ViewModels.PedidosViewModel.Instance.OrdenesActivas.Count == 1 || _idPedidoActual != -1)
            {
                var item = _idPedidoActual != -1
                    ? ViewModels.PedidosViewModel.Instance.OrdenesActivas.FirstOrDefault(c => c.Id == _idPedidoActual)
                    : ViewModels.PedidosViewModel.Instance.OrdenesActivas.FirstOrDefault();
                if (item != null)
                {
                    _idPedidoActual = item.Id;
                    await ViewModels.PedidosViewModel.Instance.SeleccionarOrden(item.Id);
                    UpdateUi(ViewModels.PedidosViewModel.Instance.OrdenSeleccionada);
                    StartUpdates();
                }
                else
                {
                    Finish();
                }
            }
            else
            {
                StartActivity(typeof(OrdenesActivasActivity));
                Finish();
            }
        }

        private void UpdateUi(Orden item)
        {
            switch (ViewModels.PedidosViewModel.Instance.MetodoPagoOrdenSeleccionada)
            {

                case MetodoPago.Tarjeta:
                    _imagenPago.SetImageResource(Resource.Drawable.ic_tarjeta);
                    _labelPago.Text = $"Pagado";
                    break;
                case MetodoPago.Efectivo:
                    _imagenPago.SetImageResource(Resource.Drawable.ic_efectivo);
                    _labelPago.Text = $"Pendiente de pago";
                    break;
                case MetodoPago.NoDefinido:
                default: break;
            }

            _labelRestaurante.Text = item.Restaurante;
            _labelFolio.Text = $"Pedido #{item.Folio}";
            _labelFecha.Text = $"Fecha: {item.FechaConFormatoEspanyol}";
            _labelTotal.Text = $"Total: {item.Total:C} MXN";
            _labelEstatus.Text = $"Estatus: {item.DescripcionEstatus}";
            ActualizarImagenEstatus(item.Estatus);
            _recyclerView.SetAdapter(new SeguimientoAdapter(this, item.Seguimientos));
            if (_progressBarHolder.Visibility == ViewStates.Visible)
            {
                StopAnimating();
            }
        }

        private void ActualizarImagenEstatus(EstatusOrden itemEstatus)
        {
            try
            {
                switch (itemEstatus)
                {

                    case EstatusOrden.EstatusUno:
                        _imagenEstatus.SetImageResource(Resource.Drawable.status1);
                        break;
                    case EstatusOrden.EstatusDos:
                        _imagenEstatus.SetImageResource(Resource.Drawable.status2);
                        break;
                    case EstatusOrden.EstatusTres:
                        _imagenEstatus.SetImageResource(Resource.Drawable.status3);
                        break;
                    case EstatusOrden.EstatusCuatro:
                        _imagenEstatus.SetImageResource(Resource.Drawable.status4);
                        break;
                    case EstatusOrden.Cancelada:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(itemEstatus), itemEstatus, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Instance_OnObtenerDetallePedidoFinished(object sender, EventsArgs.DetallePedidoEventArgs e)
        {
            if (e.Success)
            {
                UpdateUi(e.OrdenSeleccionada);
            }
        }

        private async void StartUpdates()
        {
            _isRefreshing = true;
            await ViewModels.PedidosViewModel.Instance.IniciarActualizacionPedido();
        }

        private void StopUpdates()
        {
            _isRefreshing = false;
            ViewModels.PedidosViewModel.Instance.DetenerActualizacionPedido();
        }

        private void ButtonEnviarMensaje_Click(object sender, System.EventArgs e)
        {
            var item = ViewModels.PedidosViewModel.Instance.OrdenSeleccionada;
            var intent = new Intent(this, typeof(OrdenSeguimientoActivity));
            intent.PutExtra(OrdenSeguimientoActivity.ExtraIdPedido, item.Id);
            StartActivityForResult(intent, OrdenSeguimientoActivity.RequestAgregarSeguimiento);
        }

        private void ButtonVerDetalle_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(OrdenDetalleActivity));
        }

    }
}