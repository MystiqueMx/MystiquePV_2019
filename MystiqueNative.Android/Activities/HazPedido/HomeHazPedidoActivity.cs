using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Firebase.Analytics;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Menu;
using MystiqueNative.Models.Location;
using FFImageLoading;
using MystiqueNative.Droid.Utils.Location;
using BarronWellnessMovil.Droid.Helpers;
using System.Collections.ObjectModel;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Point = MystiqueNative.Models.Location.Point;
using MystiqueNative.ViewModels;
using Newtonsoft.Json;
using Android.Gms.Location.Places.UI;
using MystiqueNative.Droid.HazPedido.Ordenes;
using MystiqueNative.Droid.HazPedido.Historial;

namespace MystiqueNative.Droid.HazPedido
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask)]
    public class HomeHazPedidoActivity : BaseActivity, DetachableResultReceiver.IReceiver
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_home;
        //protected override int BackButtonIcon => Resource.Drawable.ic_menu_white_24dp;
        #endregion

        #region EXPORT
        public const int PlacePickerRequest = 1;
        public const int LocationPermissionRequest = 13;
        public const int SolveGpsSettingRequest = 4478;
        public event EventHandler<BaseEventArgs> OnLocationUpdated;
        private static GoogleApiClient _mGoogleApiClient;
        #endregion

        #region FIELDS
        private DetachableResultReceiver _resultReceiver;
        private TextView _headerTitle;
        private ImageViewAsync _headerImage;
        private bool _manualLocationSelected;

        private bool _requestForLocationSetting;
        #endregion

        #region VIEWS
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;

        private View _layoutVacio;
        private View _layoutCerrado;
        private TextView _labelVacio;
        private TextView _labelTiempoApertura;

        private FirebaseAnalytics _firebaseAnalytics;
        private CancellationTokenSource _cancellationTokenSource;
        private RestauranteAdapter _adapter;
        private EditText _entryUbicacion;
        private RecyclerView _recyclerView;
        private AppCompatSpinner _spinnerFiltro;
        private int _filtroSelecionado;

        private FrameLayout progressBarHolder;
        #endregion

        #region LOCATION FIELDS

        private FusedLocationProviderClient _fusedLocationProviderClient;

        private bool _requestedPermissionsAlready;

        private bool _isTrackingLocation;
        private static LocationRequest DefaultLocationRequest
        {
            get
            {
                var req = new LocationRequest();
                req.SetFastestInterval(300_000L);
                req.SetInterval(400_000L);
                req.SetPriority(LocationRequest.PriorityHighAccuracy);
                return req;
            }
        }
        private LocationCallback DefaultLocationCallback
        {
            get
            {
                if (_defaultLocationCallback != null) return _defaultLocationCallback;

                _defaultLocationCallback = new LocationCallback();
                _defaultLocationCallback.LocationResult += Callback_OnLocationResult;
                return _defaultLocationCallback;
            }
        }

        private LocationCallback _defaultLocationCallback;

        #endregion

        #region LIFECYCLE
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _filtroSelecionado = (int)FiltroRestaurante.Todos;
            GrabViews();
            StartLoading();

            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            _resultReceiver = new DetachableResultReceiver(new Handler());
            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido).SetChecked(true);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            var header = _navigationView.GetHeaderView(0);
            _headerImage = header.FindViewById<ImageViewAsync>(Resource.Id.header_profile_image);
            _headerTitle = header.FindViewById<TextView>(Resource.Id.header_profile_name);

            _requestForLocationSetting = true;

            //var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            //.RequestEmail()
            //.Build();

            //_mGoogleApiClient = new GoogleApiClient.Builder(this)
            //    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
            //    .Build();
            //_mGoogleApiClient.Connect();
            //_firebaseAnalytics = FirebaseAnalytics.GetInstance(this);

            this.OnLocationUpdated += Activity_OnLocationUpdated;
        }

        protected override void OnPause()
        {
            #region OnPause
            base.OnPause();
            ViewModels.RestaurantesViewModel.Instance.OnObtenerRestaurantesFinished -= Instance_OnObtenerRestaurantesFinished;

            _resultReceiver.ClearReceiver();
            if (_isTrackingLocation)
            {
                StopUpdatingLocation();
            }

            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            #endregion
        }

        protected override void OnResume()
        {
            #region OnResume
            base.OnResume();
            ViewModels.RestaurantesViewModel.Instance.OnObtenerRestaurantesFinished += Instance_OnObtenerRestaurantesFinished;

            if (MystiqueApp.UltimaDireccionConocida == null)
            {
                if (MystiqueApp.UltimaUbicacionConocida != null)
                {
                    ParseDireccionFromUbicacion(MystiqueApp.UltimaUbicacionConocida);
                }
            }
            else
            {
                OnLocationUpdated?.Invoke(this, new BaseEventArgs { Success = true });
            }

            _resultReceiver.SetReceiver(this);
            SetupNavigationView();
            SetupLocationUpdates();
            _adapter.NotifyDataSetChanged();
            #endregion
        }
        #endregion

        #region OVERRIDES
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            #region OnOptionsItemSelected
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    //_drawerLayout.OpenDrawer((int)GravityFlags.Start);
                    OnBackPressed();
                    break;
                default:
                    break;
            }
            return true;
            #endregion
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            #region OnRequestPermissionsResult
            switch (requestCode)
            {
                case PermissionsHelper.RequestLocationId:
                    if (grantResults.Any(c => c == Permission.Denied))
                    {
                        OnLocationUpdated?.Invoke(this, new BaseEventArgs { Success = false });
                        SendToast("No se pudo obtener tu ubicación");
                        //SendSnackOrMessage(Resource.Id.coordinator_layout,
                        //    "Para buscar restaurantes en tu área, activa tu ubicación", "Reintentar",
                        //    () =>
                        //        RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(),
                        //            LocationPermissionRequest));
                    }
                    else
                    {
                        StartUpdatingLocation();
                    }
                    break;
                default:
                    break;
            }
            #endregion
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            #region OnReceiveResult
            if ((Result)resultCode == Result.Ok)
            {
                MystiqueApp.UltimaDireccionConocida = new Direction
                {
                    CountryCode = resultData.GetString(AddressResolverIntentService.ResultCountryCodeExtra),
                    CountryName = resultData.GetString(AddressResolverIntentService.ResultCountryNameExtra),
                    AdminArea = resultData.GetString(AddressResolverIntentService.ResultAdminAreaExtra),
                    SubAdminArea = resultData.GetString(AddressResolverIntentService.ResultSubAdminAreaExtra),
                    Locality = resultData.GetString(AddressResolverIntentService.ResultLocalityExtra),
                    SubLocality = resultData.GetString(AddressResolverIntentService.ResultSublocalityExtra),
                    Thoroughfare = resultData.GetString(AddressResolverIntentService.ResultThoroughfareExtra),
                    SubThoroughfare = resultData.GetString(AddressResolverIntentService.ResultSubThoroughfareExtra),
                    PostalCode = resultData.GetString(AddressResolverIntentService.ResultPostalCodeExtra),
                    OtherAddressLines = resultData.GetString(AddressResolverIntentService.ResultAddrExtra),
                    FeatureName = resultData.GetString(AddressResolverIntentService.ResultFeatureExtra)
                };
#if DEBUG
                Console.WriteLine($"||||||||||||||||||| OnReceiveResult Location Updated: {Newtonsoft.Json.JsonConvert.SerializeObject(MystiqueApp.UltimaDireccionConocida)}");
#endif
                OnLocationUpdated?.Invoke(this, new BaseEventArgs() { Success = true });
            }
            else
            {
                Console.WriteLine(resultData.GetString(AddressResolverIntentService.ResultErrorExtra));
                OnLocationUpdated?.Invoke(this, new BaseEventArgs() { Success = false });
            }
            #endregion
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            #region OnActivityResult
            if (requestCode == PlacePickerRequest)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        var place = PlacePicker.GetPlace(this, data);
                        //var toastMsg = $"NameFormatted: {place.NameFormatted}, AddressFormatted: {place.AddressFormatted}, LatLng:({place.LatLng.Latitude}, {place.LatLng.Longitude}), PhoneNumberFormatted: {place.PhoneNumberFormatted}, AttributionsFormatted:{place.AttributionsFormatted}";
                        //SendMessage(toastMsg);
                        MystiqueApp.UltimaUbicacionConocida = new Point
                        {
                            Longitude = place.LatLng.Longitude,
                            Latitude = place.LatLng.Latitude
                        };
                        MystiqueApp.UltimaDireccionConocida = null;
                        ParseDireccionFromUbicacion(MystiqueApp.UltimaUbicacionConocida);
                        _manualLocationSelected = true;
                        StopUpdatingLocation();
                        break;
                    case Result.Canceled:
                    case Result.FirstUser:
                    default:
                        base.OnActivityResult(requestCode, resultCode, data);
                        break;
                }
            }

            if (requestCode == SolveGpsSettingRequest)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        StartUpdatingLocation();
                        break;
                    case Result.Canceled:
                    case Result.FirstUser:
                    default:
                        base.OnActivityResult(requestCode, resultCode, data);
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region EVENTS
        private void Adapter_Button1Click(object sender, RecyclerClickEventArgs e)
        {
            #region Adapter_Button1Click
            var item = ViewModels.RestaurantesViewModel.Instance.Restaurantes[e.Position];
            RestaurantesViewModel.Instance.SeleccionarRestaurante(item);
            var intent = new Intent(this, typeof(MenuRestauranteHazPedidoActivity));
            intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraIdRestaurante, item.Id);
            intent.AddFlags(ActivityFlags.NewTask);

            //if (MainApplication.LogAnalytics)
            //{
            //    var bundle = new Bundle();
            //    bundle.PutString(FirebaseAnalytics.Param.ItemName, item.Nombre);
            //    _firebaseAnalytics.LogEvent(AnalyticsActions.Acciones.VerRestaurante, bundle);
            //}

            if (item.EstaAbierto && item.EstaOperando)
            {
                StartActivity(intent);
            }
            else
            {
                if (!item.EstaAbierto)
                {
                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el horario de atención es de {item.HoraApertura} a {item.HoraCierre}",
                        "Restaurante cerrado", "Continuar", "",
                        ok =>
                        {
                            intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                            StartActivity(intent);
                        });
                }
                else
                {
                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el restaurante no se encuentra disponible en este momento",
                        "Restaurante no disponible", "Continuar", "",
                        ok =>
                        {
                            intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                            StartActivity(intent);
                        });
                }
            }
            #endregion
        }

        private void EntryUbicacion_Click(object sender, System.EventArgs e)
        {
            #region EntryUbicacion_Click
            var builder = new PlacePicker.IntentBuilder();
            StartActivityForResult(builder.Build(this), HomeHazPedidoActivity.PlacePickerRequest);
            #endregion
        }

        private void _entryUbicacion_LongClick(object sender, View.LongClickEventArgs e)
        {
            #region _entryUbicacion_LongClick
            SendMessage(JsonConvert.SerializeObject(MystiqueApp.UltimaDireccionConocida), "Direccion");
            #endregion
        }

        private async void _spinnerFiltro_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            #region _spinnerFiltro_ItemSelected
            _filtroSelecionado = e.Position;
            await ViewModels.RestaurantesViewModel.Instance.ObtenerRestaurantes((FiltroRestaurante)_filtroSelecionado);
            #endregion
        }

        private async void Activity_OnLocationUpdated(object sender, BaseEventArgs e)
        {
            #region Activity_OnLocationUpdated
            if (!e.Success)
            {
                _labelVacio.Text = GetString(Resource.String.restaurantes_label_no_restaurantes);
                _layoutVacio.Visibility = ViewStates.Visible;
                _layoutCerrado.Visibility = ViewStates.Gone;
                _recyclerView.Visibility = ViewStates.Gone;
                _entryUbicacion.Text = $"Presiona aquí para seleccionar tu ubicación";
                return;
            }

            await ViewModels.RestaurantesViewModel.Instance.ObtenerRestaurantes((FiltroRestaurante)_filtroSelecionado);

            var direccion = new List<string>();
            var calle =
                $"{MystiqueApp.UltimaDireccionConocida.SubThoroughfare} {MystiqueApp.UltimaDireccionConocida.Thoroughfare}";
            if (!string.IsNullOrEmpty(calle.Trim()))
            {
                direccion.Add(calle);
            }
            if (!string.IsNullOrEmpty(MystiqueApp.UltimaDireccionConocida.SubLocality))
            {
                direccion.Add(MystiqueApp.UltimaDireccionConocida.SubLocality);
            }
            _entryUbicacion.Text =
                $"{string.Join(", ", direccion)}";
            _cancellationTokenSource = new CancellationTokenSource();
            #endregion
        }
        #endregion

        #region NAVIGATION
        private void ShowVisitorMenu()
        {
            #region ShowVisitorMenu
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_profile).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(false);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_login).SetVisible(true);
            #endregion
        }

        private void ShowFullMenu()
        {
            #region ShowFullMenu
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_profile).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(true);
            _navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_login).SetVisible(false);
            #endregion
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            #region NavigationView_NavigationItemSelected
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
            #endregion
        }
        #endregion

        #region METHODS
        private void GrabViews()
        {
            #region GrabViews
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido).SetChecked(true);

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);

            progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _entryUbicacion = FindViewById<EditText>(Resource.Id.restaurantes_entry_ubicacion);
            _layoutVacio = FindViewById(Resource.Id.ly_vacio);
            _layoutCerrado = FindViewById(Resource.Id.restaurantes_ly_cerrados);
            _labelVacio = FindViewById<TextView>(Resource.Id.label_vacio);
            _labelTiempoApertura = FindViewById<TextView>(Resource.Id.restaurantes_tiempo_apertura);
            _spinnerFiltro = FindViewById<AppCompatSpinner>(Resource.Id.restaurantes_spinner_filtro);
            _layoutVacio.Visibility = ViewStates.Gone;
            _layoutCerrado.Visibility = ViewStates.Gone;

            _adapter = new RestauranteAdapter(this, ViewModels.RestaurantesViewModel.Instance.Restaurantes);

            _recyclerView.HasFixedSize = true;
            _recyclerView.SetAdapter(_adapter);

            _adapter.ItemClick += Adapter_Button1Click;

#if DEBUG
            //NOTE por el momento EntryUbicacion_Click solo funcionara para dev
            _entryUbicacion.Click += EntryUbicacion_Click;
            _entryUbicacion.LongClick += _entryUbicacion_LongClick;
#endif
            SetupSpinnerFiltro();
            #endregion
        }

        private void SetupLocationUpdates()
        {
            #region SetupLocationUpdates
            if (!_isTrackingLocation && !_manualLocationSelected)
            {
                AskForPermissionsThenUpdateLocation();
            }

            //if (_manualLocationSelected && MainApplication.Instance.UltimaDireccionConocida == null)
            //{
            //    ParseDireccionFromUbicacion(MystiqueApp.UltimaUbicacionConocida);
            //}
            #endregion
        }

        private void SetupNavigationView()
        {
            #region SetupNavigationView
            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido)
                    .SetChecked(true);
            //_headerTitle.Text = $"{MystiqueApp.Usuario.Username}";
            _headerTitle.Text = $"{AuthViewModelV2.Instance.Usuario.NombreCompleto}";

            ImageService.Instance
                .LoadUrl(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl)
                .DownSampleInDip(100)
                .Transform(new FFImageLoading.Transformations.CircleTransformation())
                .Into(_headerImage);

            ShowFullMenu();
            //if (QdcApplication.Instance.IsLoggedIn)
            //{
            //    ShowFullMenu();
            //}
            //else
            //{
            //    ShowVisitorMenu();
            //}
            #endregion
        }

        private void AskForPermissionsThenUpdateLocation()
        {
            #region AskForPermissionsThenUpdateLocation
            if (PermissionsHelper.ValidatePermissionsForLocation())
            {
                StartUpdatingLocation();
            }
            else
            {
                if (_requestedPermissionsAlready) return;

                _requestedPermissionsAlready = true;
                RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(), PermissionsHelper.RequestLocationId);
            }
            #endregion
        }

        private async void StartUpdatingLocation()
        {
            #region StartUpdatingLocation
            var locationAvailability = await _fusedLocationProviderClient.GetLocationAvailabilityAsync();
            if (locationAvailability.IsLocationAvailable)
            {
                _isTrackingLocation = true;
                await _fusedLocationProviderClient.RequestLocationUpdatesAsync(DefaultLocationRequest, DefaultLocationCallback);
            }
            else
            {
                OnLocationUpdated?.Invoke(this, new BaseEventArgs { Success = false });

                if (_requestForLocationSetting)
                {
                    _requestForLocationSetting = false;
                    var builder = new LocationSettingsRequest.Builder()
                        .AddLocationRequest(DefaultLocationRequest);
                    var client = LocationServices.GetSettingsClient(this);
                    client.CheckLocationSettings(builder.Build())
                        .AddOnCompleteListener(new OnCompleteEventHandleListener(task =>
                        {
                            if (!task.IsSuccessful && task.Exception is ResolvableApiException resolvableApiException)
                            {
                                try
                                {
                                    resolvableApiException.StartResolutionForResult(this, SolveGpsSettingRequest);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }));
                }
                else
                {
                    SendToast("No se pudo obtener tu ubicación");
                }
            }
            #endregion
        }

        private void StopUpdatingLocation()
        {
            #region StopUpdatingLocation
            _fusedLocationProviderClient.RemoveLocationUpdates(DefaultLocationCallback);
            #endregion
        }

        private void Callback_OnLocationResult(object sender, LocationCallbackResultEventArgs e)
        {
            #region Callback_OnLocationResult
#if DEBUG
            Console.WriteLine($"||||||||||||||||||| Callback_OnLocationResult Location Updated: {e.Result.LastLocation.Latitude}, {e.Result.LastLocation.Longitude}");
#endif

            MystiqueApp.UltimaUbicacionConocida = new Point
            {
                Longitude = -115.449090655893 /*e.Result.LastLocation.Longitude*/,
                Latitude = 32.6260518104036  /*e.Result.LastLocation.Latitude*/
            };
            ParseDireccionFromUbicacion(MystiqueApp.UltimaUbicacionConocida);
            #endregion
        }

        private void ParseDireccionFromUbicacion(Point location)
        {
            #region ParseDireccionFromUbicacion
            var intent = new Intent(this, typeof(AddressResolverIntentService));
#if DEBUG
            Console.WriteLine($"||||||||||||||||||| ParseDireccionFromUbicacion Starting service intent");
#endif
            intent.PutExtra(AddressResolverIntentService.AddressReceiverExtra, _resultReceiver);
            intent.PutExtra(AddressResolverIntentService.LatitudeDataExtra, location.Latitude);
            intent.PutExtra(AddressResolverIntentService.LongitudeDataExtra, location.Longitude);
            StartService(intent);
            #endregion
        }

        private void SetupSpinnerFiltro()
        {
            #region SetupSpinnerFiltro
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, HelperFiltroRestaurante.FiltrosRestaurante);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _spinnerFiltro.Adapter = adapter;
            _spinnerFiltro.SetSelection((int)_filtroSelecionado);
            _spinnerFiltro.ItemSelected += _spinnerFiltro_ItemSelected;
            #endregion
        }
        #endregion

        #region CALLBACKS
        private void Instance_OnObtenerRestaurantesFinished(object sender, EventsArgs.ObtenerRestaurantesEventArgs e)
        {
            #region Instance_OnObtenerRestaurantesFinished
#if DEBUG
            Console.WriteLine($"||||||||||||||||||| Instance_OnObtenerRestaurantesFinished : {e.EstatusRestaurantes.ToString()}");
            //Crashlytics.Crashlytics.Instance.Crash();
#endif

            /* CODIGO PARA USO DE AGENTE FRESCO */
            StopLoading();
            if (e.Success && RestaurantesViewModel.Instance.Restaurantes.Count() > 0)
            {
                var item = ViewModels.RestaurantesViewModel.Instance.Restaurantes[0];
                RestaurantesViewModel.Instance.SeleccionarRestaurante(item);
                var intent = new Intent(this, typeof(MenuRestauranteHazPedidoActivity));
                intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraIdRestaurante, item.Id);
                intent.AddFlags(ActivityFlags.NewTask);

                if (item.EstaAbierto && item.EstaOperando)
                {
                    StartActivity(intent);
                }
                else
                {
                    if (!item.EstaAbierto)
                    {
                        SendConfirmation(
                            $"Puedes continuar explorando el menú, el horario de atención es de {item.HoraApertura} a {item.HoraCierre}",
                            "Restaurante cerrado", "Continuar", "",
                            ok =>
                            {
                                intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                                StartActivity(intent);
                            });
                    }
                    else
                    {
                        SendConfirmation(
                            $"Puedes continuar explorando el menú, el restaurante no se encuentra disponible en este momento",
                            "Restaurante no disponible", "Continuar", "",
                            ok =>
                            {
                                intent.PutExtra(MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                                StartActivity(intent);
                            });
                    }
                }
            } else
            {

            }
            /* FIN CODIGO PARA USO DE AGENTE FRESCO */






            /* COMENTADO POR USO DE AGENTE FRESCO */
            //if (e.Success && (e.Restaurantes?.Any() ?? false))
            //{
            //    _recyclerView.Visibility = ViewStates.Visible;
            //    _layoutVacio.Visibility = ViewStates.Gone;
            //    _layoutCerrado.Visibility = ViewStates.Gone;
            //}
            //else
            //{
            //    _recyclerView.Visibility = ViewStates.Gone;
            //    _labelVacio.Text = GetString(Resource.String.restaurantes_label_no_restaurantes);
            //    _layoutVacio.Visibility = ViewStates.Visible;
            //    _layoutCerrado.Visibility = ViewStates.Gone;
            //}
            /* FIN COMENTADO POR USO DE AGENTE FRESCO */



            //if (e.Success && QdcApplication.Instance.RestauranteId_DeepLink.HasValue)
            //{
            //    var restaurant = RestaurantesViewModel.Instance.Restaurantes.FirstOrDefault(c =>
            //        c.Id == QdcApplication.Instance.RestauranteId_DeepLink.Value);
            //    QdcApplication.Instance.RestauranteId_DeepLink = null;
            //    if (restaurant == null)
            //    {
            //        SendMessage("El restaurante no se encuentra disponible en tu área");
            //    }
            //    else
            //    {
            //        RestaurantesViewModel.Instance.SeleccionarRestaurante(restaurant);
            //        var intent = new Intent(this, typeof(MenuRestauranteActivity));
            //        intent.PutExtra(MenuRestauranteActivity.ExtraIdRestaurante, restaurant.Id);
            //        intent.AddFlags(ActivityFlags.NewTask);

            //        if (MainApplication.LogAnalytics)
            //        {
            //            var bundle = new Bundle();
            //            bundle.PutString(FirebaseAnalytics.Param.ItemName, restaurant.Nombre);
            //            _firebaseAnalytics.LogEvent(AnalyticsActions.Acciones.VerRestaurante, bundle);
            //        }

            //        if (restaurant.EstaAbierto && restaurant.EstaOperando)
            //        {

            //            StartActivity(intent);
            //        }
            //        else
            //        {
            //            if (!restaurant.EstaAbierto)
            //            {
            //                SendConfirmation(
            //                    $"Puedes continuar explorando el menú, el horario de atención es de {restaurant.HoraApertura} a {restaurant.HoraCierre}",
            //                    "Restaurante cerrado", "Continuar", "",
            //                    ok =>
            //                    {
            //                        intent.PutExtra(MenuRestauranteActivity.ExtraModoLectura, true);
            //                        StartActivity(intent);
            //                    });
            //            }
            //            else
            //            {
            //                SendConfirmation(
            //                    $"Puedes continuar explorando el menú, el restaurante no se encuentra disponible en este momento",
            //                    "Restaurante no disponible", "Continuar", "",
            //                    ok =>
            //                    {
            //                        intent.PutExtra(MenuRestauranteActivity.ExtraModoLectura, true);
            //                        StartActivity(intent);
            //                    });
            //            }

            //        }
            //    }
            //}
            #endregion
        }
        #endregion

        #region LOADING

        private void StartLoading()
        {
            #region StartAnimating
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = inAnimation;
            progressBarHolder.Visibility = ViewStates.Visible;
            #endregion
        }

        private void StopLoading()
        {
            #region StopAnimating
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = outAnimation;
            progressBarHolder.Visibility = ViewStates.Gone;
            #endregion
        }

        #endregion
    }


    public class RestauranteAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Models.Menu.Restaurante> _viewModel;
        private readonly Activity _context;

        public RestauranteAdapter(Activity context, ObservableCollection<Models.Menu.Restaurante> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));

            this._viewModel.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_restaurante;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new RestauranteViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is RestauranteViewHolder myHolder)) return;
            myHolder.Image.SetImageBitmap(null);
            myHolder.Image.SetImageResource(Resource.Drawable.Cargandox3);

            myHolder.Title.Text = $"{item.Nombre}";
            if (item.EstaAbierto && item.EstaOperando)
            {
                //myHolder.Line1.Text = $"Horario: {item.HoraApertura} - {item.HoraCierre}";
                //myHolder.Line1.SetTextColor(new Color(127,127,127));

                myHolder.Status.Text = $"ORDENAR";
                myHolder.Status.SetBackgroundColor(new Color(35, 200, 36));
            }
            else
            {
                if (!item.EstaAbierto)
                {
                    myHolder.Status.Text = $"CERRADO";
                    myHolder.Status.SetBackgroundColor(new Color(197, 68, 63));
                }
                else
                {
                    myHolder.Status.Text = $"NO DISPONIBLE";
                    myHolder.Status.SetBackgroundColor(new Color(197, 68, 63));
                }

            }

            myHolder.Line1.Text = $"Horario: {item.HoraApertura} - {item.HoraCierre}";
            var str = new SpannableStringBuilder("A domicilio: ");

            str.Append(item.TieneRepartoADomicilio ? $"DISPONIBLE" : $"NO DISPONIBLE",
                item.TieneRepartoADomicilio ? new ForegroundColorSpan(new Color(35, 200, 36)) : new ForegroundColorSpan(new Color(197, 68, 63)),
                SpanTypes.ExclusiveExclusive);
            myHolder.Line2.SetText(str, TextView.BufferType.Spannable);

            ImageService.Instance.LoadUrl(item.ImagenUrl)
                .DownSampleInDip(height: 100)
                .Into(myHolder.Image);

            myHolder.Direccion.Text = item.Direccion;
            string serviciosDisp = "Para Recoger";
            if (item.TieneRepartoADomicilio) { serviciosDisp += ", A Domicilio"; }
            if (item.TieneDriveThru) { serviciosDisp += ", Drive Thru"; }
            myHolder.servicios.Text = serviciosDisp;
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class RestauranteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Line1 { get; }
        public TextView Line2 { get; }
        public TextView Status { get; }
        public TextView Direccion { get; }
        public TextView servicios { get; }
        public Button ButtonMenu { get; }
        public CardView CardViewInicio { get; }
        public ImageViewAsync Image { get; }

        public RestauranteViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Line2 = itemView.FindViewById<TextView>(Resource.Id.item_line2);
            Status = itemView.FindViewById<TextView>(Resource.Id.item_status);
            Direccion = itemView.FindViewById<TextView>(Resource.Id.item_restaurante_txv_direccion);
            servicios = itemView.FindViewById<TextView>(Resource.Id.item_restaurante_txv_servicios);
            ButtonMenu = itemView.FindViewById<Button>(Resource.Id.item_button_1);
            CardViewInicio = itemView.FindViewById<CardView>(Resource.Id.card_view_inicio);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.item_image);

            CardViewInicio.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

            ButtonMenu.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

        }
    }
}