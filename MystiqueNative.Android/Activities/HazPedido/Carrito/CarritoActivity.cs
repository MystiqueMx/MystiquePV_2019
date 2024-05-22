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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using Firebase.Analytics;
using Humanizer;
using MystiqueNative.Droid.HazPedido.Historial;
using MystiqueNative.Droid.HazPedido.Ordenes;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Helpers;
using MystiqueNative.Helpers.Analytics;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Orden;
using MystiqueNative.ViewModels;
using Point = MystiqueNative.Models.Location.Point;

namespace MystiqueNative.Droid.HazPedido.Carrito
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask, Label = "Mi carrito")]
    public class CarritoActivity : BaseActivity, DetachableResultReceiver.IReceiver
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_carrito;
        //protected override int BackButtonIcon => Resource.Drawable.ic_menu_white_24dp;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #endregion

        #region EXPORTS
        public const int PlacePickerRequest = 1;
        #endregion

        #region VIEWS

        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private ImageViewAsync _headerImage;
        private TextView _headerTitle;
        private TextView _headerTitleCliente;
        private EditText _entryLugarEntrega;
        private EditText _entryLugarEntregaMapa;
        private EditText _entryMetodoPago;
        private LinearLayout _layoutNoContent;
        private LinearLayout _layoutContent;
        private Button _buttonTerminar;
        private TextView _labelTotal;
        private TextView _labelCargoServicio;
        //private Spinner _spinnerClientes;

        private FrameLayout _progressBarHolder;
        private Button _buttonComer;
        #endregion

        #region FIELDS
        private DetachableResultReceiver _resultReceiver;

        private ItemCarritoAdapter _itemCarritoAdapter;
        private ItemCarrito _ultimoItemSeleccionado;

        private TipoReparto _tipoReparto = TipoReparto.NoDefinido;
        private MetodoPago _metodoPago = MetodoPago.NoDefinido;

        private DireccionOrden _direccionOrdenEntrega;
        private FormaPago _informacionPago;
        private Models.Clientes.ClienteCallCenter _clienteRecibe;
        private FirebaseAnalytics _firebaseAnalytics;
        private TextView _labelRestaurante;
        private static GoogleApiClient _mGoogleApiClient;

        private double latMapSelected, lonMapSelected;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            #region OnCreate
            base.OnCreate(savedInstanceState);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            _resultReceiver = new DetachableResultReceiver(new Handler());
            GrabViews();
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_cart_haz_pedido).SetChecked(true);
            //_navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            //var header = _navigationView.GetHeaderView(0);
            //_headerImage = header.FindViewById<ImageViewAsync>(Resource.Id.header_profile_image);
            //_headerTitle = header.FindViewById<TextView>(Resource.Id.header_profile_name);
            _headerTitleCliente = FindViewById<TextView>(Resource.Id.carrito_label_cliente);

            _resultReceiver.SetReceiver(this);

            _metodoPago = MetodoPago.Efectivo;
            _informacionPago = new FormaPago()
            {
                Metodo = MetodoPago.Efectivo,
            };
            _clienteRecibe = new Models.Clientes.ClienteCallCenter();
            _entryMetodoPago.Text = $"Pago en efectivo";

            _headerTitleCliente.Text = "Pedido para: " + ClientesViewModel.Instance.ClienteSelected.nombreCompleto;

            //var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            //.RequestEmail()
            //.Build();

            //_mGoogleApiClient = new GoogleApiClient.Builder(this)
            //    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
            //    .Build();
            //_mGoogleApiClient.Connect(); 
            #endregion
        }
        private async void GrabViews()
        {
            #region GrabViews
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _labelTotal = FindViewById<TextView>(Resource.Id.carrito_label_total);
            _labelRestaurante = FindViewById<TextView>(Resource.Id.carrito_label_restaurante);
            _labelCargoServicio = FindViewById<TextView>(Resource.Id.carrito_label_cargo_servicio);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            //_navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
            _entryLugarEntrega = FindViewById<EditText>(Resource.Id.carrito_entry_lugar);
            _entryLugarEntregaMapa = FindViewById<EditText>(Resource.Id.carrito_entry_lugar_mapa);
            _entryMetodoPago = FindViewById<EditText>(Resource.Id.carrito_entry_metodo);
            _buttonTerminar = FindViewById<Button>(Resource.Id.button_terminar);
            _buttonComer = FindViewById<Button>(Resource.Id.button_comer);
            //_spinnerClientes = FindViewById<Spinner>(Resource.Id.spinner_clientes);

            //var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout,
            //    Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            //_drawerLayout.AddDrawerListener(drawerToggle);
            //drawerToggle.SyncState();

            //_navigationView.Menu.FindItem(Resource.Id.nav_item_help)
            //    .SetChecked(true);

            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _itemCarritoAdapter = new ItemCarritoAdapter(this, ViewModels.CarritoViewModel.Instance.Items);

            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(_itemCarritoAdapter);

            //_entryMetodoPago.Click += EntryMetodoPago_Click;
            _itemCarritoAdapter.ItemClick += ItemCarritoAdapter_Click;

            _layoutContent = FindViewById<LinearLayout>(Resource.Id.content_view);
            _layoutNoContent = FindViewById<LinearLayout>(Resource.Id.no_content_view);

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerview.AddItemDecoration(itemDecor);

            _buttonTerminar.Click += ButtonTerminar_Click;

            _buttonComer.Click += delegate
            {
                StartActivity(typeof(HomeHazPedidoActivity));
                OverridePendingTransition(0, 0);
            };

            _entryLugarEntregaMapa.Click += EntryLugarEntregaMapa_Click;
            _entryLugarEntregaMapa.SetBackgroundResource(Resource.Drawable.entry_white_background);

            //_spinnerClientes.ItemSelected += Spinner_ClienteSelected;
            //_spinnerClientes.SetBackgroundResource(Resource.Drawable.entry_white_background);

            await ClientesViewModel.Instance.ObtenerClientesCallCenter(); 
            #endregion
        }

        protected override void OnResume()
        {
            #region OnResume
            base.OnResume();
            SetupNavigationView();
            ShowContent();
            UpdateUi();
            _itemCarritoAdapter.NotifyDataSetChanged();
            PedidosViewModel.Instance.OnRegistrarOrdenFinished += Instance_OnRegistrarOrdenFinished;
            ClientesViewModel.Instance.OnFinishObtenerClientes += Instance_OnFinishObtenerclientes;
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

        protected override void OnPause()
        {
            #region OnPause
            base.OnPause();
            PedidosViewModel.Instance.OnRegistrarOrdenFinished -= Instance_OnRegistrarOrdenFinished;
            ClientesViewModel.Instance.OnFinishObtenerClientes -= Instance_OnFinishObtenerclientes;
            if (!CarritoViewModel.Instance.ExistenItemsEnCarrito) return;
            if (CarritoViewModel.Instance.PedidoActual.Restaurante.TieneRepartoADomicilio)
            {
                _entryLugarEntrega.Click -= EntryLugarEntrega_Click;
            } 
            #endregion
        }

        protected override void OnDestroy()
        {
            #region OnDestroy
            base.OnDestroy();
            _resultReceiver.ClearReceiver(); 
            #endregion
        }

        #endregion

        #region NAVIGATION
        private void SetupNavigationView()
        {
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_cart_haz_pedido)
            //    .SetChecked(true);
            ////_headerTitle.Text = $"{MystiqueApp.Usuario.Username}";
            //_headerTitle.Text = $"{AuthViewModelV2.Instance.Usuario.NombreCompleto}";

            //ImageService.Instance
            //    .LoadUrl(ViewModels.AuthViewModelV2.Instance.ProfilePictureUrl)
            //    .DownSampleInDip(100)
            //    .Transform(new CircleTransformation())
            //    .Into(_headerImage);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    //_drawerLayout.OpenDrawer((int)GravityFlags.Start);

                    OnBackPressed();
                    return true;
                default:
                    return true;
            }
        }
        private void ShowVisitorMenu()
        {
            ////_navigationView.Menu.FindItem(Resource.Id.nav_item_profile).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(false);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(false);
            ////_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(false);

            //_navigationView.Menu.FindItem(Resource.Id.nav_item_login).SetVisible(true);
        }

        private void ShowFullMenu()
        {
            ////_navigationView.Menu.FindItem(Resource.Id.nav_item_profile).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_history_haz_pedido).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_room_haz_pedido).SetVisible(true);
            //_navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido).SetVisible(true);
            ////_navigationView.Menu.FindItem(Resource.Id.nav_item_logout).SetVisible(true);


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

        #region OVERRIDES
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            #region OnActivityResult
            switch (resultCode)
            {

                case Result.FirstUser when CarritoDireccionActivity.RequestDireccion == requestCode:
                    _tipoReparto = TipoReparto.Sucursal;
                    _direccionOrdenEntrega = new DireccionOrden();
                    _entryLugarEntrega.Text = $"Recoger en sucursal: {CarritoViewModel.Instance.PedidoActual.Restaurante.Direccion}";
                    break;

                case Result.Ok when CarritoDireccionActivity.RequestDireccion == requestCode:
                    _tipoReparto = TipoReparto.Particular;
                    var idDireccion = data.GetIntExtra(CarritoDireccionActivity.ExtraDirection, -1);
                    if (idDireccion == -1) throw new ArgumentNullException(nameof(CarritoDireccionActivity.ExtraDirection));
                    var item = DireccionesViewModel.Instance.Direcciones.First(c => c.Id == idDireccion);
                    _direccionOrdenEntrega = new DireccionOrden()
                    {
                        Direccion = item,
                        Ubicacion = new Point
                        {
                            Latitude = item.Latitud,
                            Longitude = item.Longitud,
                        }
                    };
                    _entryLugarEntrega.Text = $"{item.Nombre}: {item.Thoroughfare} {item.SubThoroughfare}, {item.SubLocality}";
                    break;

                case Result.FirstUser when CarritoMetodoPagoActivity.RequestMetodoPago == requestCode:
                    _metodoPago = MetodoPago.Efectivo;
                    _informacionPago = new FormaPago()
                    {
                        Metodo = MetodoPago.Efectivo,
                    };
                    _entryMetodoPago.Text = $"Pago en efectivo";
                    break;

                case Result.Ok when CarritoMetodoPagoActivity.RequestMetodoPago == requestCode:
                    _metodoPago = MetodoPago.Tarjeta;
                    var idTarjeta = data.GetStringExtra(CarritoMetodoPagoActivity.ExtraTarjeta);
                    if (string.IsNullOrEmpty(idTarjeta)) throw new ArgumentNullException(nameof(CarritoMetodoPagoActivity.ExtraTarjeta));
                    var tarjeta = TarjetasViewModel.Instance.Tarjetas.First(c => c.Id == idTarjeta);
                    _informacionPago = new FormaPago()
                    {
                        Metodo = MetodoPago.Tarjeta,
                        IdTarjeta = idTarjeta,
                    };
                    _entryMetodoPago.Text = $"{tarjeta.Brand.Humanize(LetterCasing.Title)} {tarjeta.MaskedCardNumber}";
                    break;

                case Result.Canceled:

                default:
                    break;

            }

            if (requestCode == PlacePickerRequest)
            {
                //TODO
                switch (resultCode)
                {
                    case Result.Ok:
                        var place = PlacePicker.GetPlace(this, data);
                        _direccionOrdenEntrega = new DireccionOrden()
                        {
                            Ubicacion = new Point
                            {
                                Latitude = place.LatLng.Latitude,
                                Longitude = place.LatLng.Longitude,
                            }
                        };

                        var intent = new Intent(this, typeof(AddressResolverIntentService));
                        intent.PutExtra(AddressResolverIntentService.AddressReceiverExtra, _resultReceiver);
                        intent.PutExtra(AddressResolverIntentService.LatitudeDataExtra, place.LatLng.Latitude);
                        intent.PutExtra(AddressResolverIntentService.LongitudeDataExtra, place.LatLng.Longitude);
                        StartService(intent);
                        break;
                    case Result.Canceled:
                    case Result.FirstUser:
                    default:
                        base.OnActivityResult(requestCode, resultCode, data);
                        break;
                }
            }

            UpdateUi();
            #endregion
        }
        #endregion

        #region EVENTS
        private void EntryMetodoPago_Click(object sender, System.EventArgs e)
        {
            #region EntryMetodoPago_Click
            //if (string.IsNullOrEmpty(MystiqueApp.Usuario.Telefono))
            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Telefono))
            {
                SendConfirmation("Por favor complete la información de su perfil para continuar con su pedido", "",
                    "Completar", "Más tarde",
                    ok =>
                    {
                        if (!ok) return;
                        StartActivity(typeof(PerfilActivity));
                        //StartActivityForResult(typeof(Activities.UpdateProfileActivity),
                        //    Activities.UpdateProfileActivity.UpdateRequestCode);
                    });
            }
            else
            {
                StartActivityForResult(typeof(CarritoMetodoPagoActivity),
                    CarritoMetodoPagoActivity.RequestMetodoPago);
            } 
            #endregion
        }

        private void EntryLugarEntrega_Click(object sender, System.EventArgs e)
        {
            #region EntryLugarEntrega_Click
            //if (string.IsNullOrEmpty(MystiqueApp.Usuario.Telefono))
            if (string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Telefono))
            {
                SendConfirmation("Por favor actualiza tu número de teléfono para continuar con su pedido", "", "Completar", "Más tarde",
                    ok =>
                    {
                        if (!ok) return;

                        //StartActivityForResult(typeof(PerfilActivity), Activities.UpdateProfileActivity.UpdateRequestCode);
                        StartActivity(typeof(PerfilActivity));
                        OverridePendingTransition(0, 0);
                    });
            }
            else
            {
                var intent = new Intent(this, typeof(CarritoDireccionActivity));
                intent.AddFlags(ActivityFlags.ReorderToFront);

                StartActivityForResult(intent, CarritoDireccionActivity.RequestDireccion);
                OverridePendingTransition(0, 0);
            } 
            #endregion
        }        

        private void EntryLugarEntregaMapa_Click(object sender, EventArgs e)
        {
            #region EntryLugarEntregaMapa_Click
            var builder = new PlacePicker.IntentBuilder();
            StartActivityForResult(builder.Build(this), PlacePickerRequest);
            #endregion
        }

        private void ItemCarritoAdapter_Click(object sender, RecyclerClickEventArgs e)
        {
            _ultimoItemSeleccionado = CarritoViewModel.Instance.Items[e.Position];
            var bs = ItemCarritoBottomSheet.Instance;
            bs.OnDetailSelected += BottomSheet_OnDetailSelected;
            bs.OnDeleteSelected += BottomSheet_OnDeleteSelected;
            bs.OnNoteSelected += BottomSheet_OnNoteSelected;
            bs.Show(SupportFragmentManager, nameof(bs));
        }

        private void BottomSheet_OnNoteSelected(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(CarritoNotaActivity));
            intent.PutExtra(CarritoNotaActivity.ExtraHash, _ultimoItemSeleccionado.Hash.ToString());
            StartActivity(intent);
        }

        private void BottomSheet_OnDeleteSelected(object sender, System.EventArgs e)
        {
            SendConfirmation("¿Estás seguro que deseas eliminarlo de tu carrito?", "", "Eliminar", "Cancelar", ok =>
            {
                if (!ok) return;
                CarritoViewModel.Instance.RemoverItemCarrito(_ultimoItemSeleccionado.Hash);
                ShowContent();
                UpdateUi();
                //if (!MainApplication.LogAnalytics) return;

                var bundle = new Bundle();
                bundle.PutString(FirebaseAnalytics.Param.ItemName, _ultimoItemSeleccionado.Nombre);
                bundle.PutString(FirebaseAnalytics.Param.ItemCategory, $"{CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre}");
                _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.RemoveFromCart, bundle);
            });
        }

        private void BottomSheet_OnDetailSelected(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(CarritoDetalleActivity));
            intent.PutExtra(CarritoDetalleActivity.ExtraHash, _ultimoItemSeleccionado.Hash.ToString());
            intent.PutExtra(CarritoDetalleActivity.ExtraContent, _ultimoItemSeleccionado.Contenido);
            intent.PutExtra(CarritoDetalleActivity.ExtraImagen, _ultimoItemSeleccionado.Imagen);
            intent.PutExtra(CarritoDetalleActivity.ExtraPrecio, $"{_ultimoItemSeleccionado.Precio:C}");
            intent.PutExtra(CarritoDetalleActivity.ExtraTitle, _ultimoItemSeleccionado.Nombre);
            StartActivity(intent);
        }

        private async void ButtonTerminar_Click(object sender, System.EventArgs e)
        {
            StartAnimating();
            await PedidosViewModel.Instance.TerminarPedido(_tipoReparto, _informacionPago, _direccionOrdenEntrega);
        }        

        private void Spinner_ClienteSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            #region Spinner_ClienteSelected
            //Models.Clientes.ClienteCallCenter item = ClientesViewModel.Instance.ClienteCallCenter[e.Position];
            //_clienteRecibe.ID = item.ID;
            //_clienteRecibe.nombreCompleto = item.nombreCompleto;
            //_clienteRecibe.telefono = item.telefono;
            #endregion
        }
        #endregion

        #region CALLBACK
        private void Instance_OnRegistrarOrdenFinished(object sender, MystiqueNative.EventsArgs.RegistrarPedidoEventArgs e)
        {
            StopAnimating();
            if (e.Success)
            {
                if (MainApplication.LogAnalytics)
                {
                    var bundle = new Bundle();
                    //bundle.PutDouble(FirebaseAnalytics.Param.Price, (double)e.Total);
                    bundle.PutString(FirebaseAnalytics.Param.Destination, e.TipoDeReparto.Humanize());
                    bundle.PutString(FirebaseAnalytics.Param.CheckoutOption, e.MetodoDePago.Humanize());
                    _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.EcommercePurchase, bundle);
                }
                //SendConfirmation("Tu orden ha sido procesada con éxito", "", "Ver pedido", "Seguir explorando",
                //    ok =>
                //    {
                //        if (ok)
                //        {
                //            StartActivity(e.NumeroOrdenesActivas == 1
                //                ? typeof(OrdenActivaActivity)
                //                : typeof(OrdenesActivasActivity));
                //            OverridePendingTransition(0, 0);
                //        }
                //        else
                //        {
                //            StartActivity(typeof(HomeHazPedidoActivity));
                //            OverridePendingTransition(0, 0);
                //        }
                //    });

                var intent = new Intent(this, typeof(LandingHasbroActivity));
                intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                SendConfirmation("Tu orden ha sido procesada con éxito", "", "OK", "Cerrar",
                    ok =>
                    {
                        if (ok)
                        {
                            //StartActivity(e.NumeroOrdenesActivas == 1
                            //    ? typeof(OrdenActivaActivity)
                            //    : typeof(OrdenesActivasActivity));

                            StartActivity(intent);
                            OverridePendingTransition(0, 0);
                        }
                        else
                        {
                            //StartActivity(typeof(HomeHazPedidoActivity));

                            StartActivity(intent);
                            OverridePendingTransition(0, 0);
                        }
                    });
            }
            else
            {
                if (MainApplication.LogAnalytics)
                {
                    var bundle = new Bundle();
                    bundle.PutString(FirebaseAnalytics.Param.Source, AnalyticsActions.Errores.TerminarPedido);
                    bundle.PutString(FirebaseAnalytics.Param.Value, e.Message);
                    _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.CheckoutProgress, bundle);
                }
                SendMessage(e.Message, "Error al procesar el pedido");
            }
        }
               
        private void Instance_OnFinishObtenerclientes(object sender, BaseListEventArgs e)
        {
            #region Instance_OnFinishObtenerclientes
            //if (e.Success)
            //{
            //    List<string> _listClientes = new List<string>();
            //    _listClientes.AddRange(ClientesViewModel.Instance.ClienteCallCenter.Select(s => s.nombreCompleto).ToList());

            //    var _adapterIndPadre = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listClientes);
            //    _adapterIndPadre.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            //    _spinnerClientes.Adapter = _adapterIndPadre;
            //} else
            //{

            //}
            #endregion
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            //if (_direccionLoaded) return;
            //_direccionLoaded = true;
            if ((Result)resultCode == Result.Ok)
            {
                MystiqueNative.Models.Location.Direction _direction = new MystiqueNative.Models.Location.Direction
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

                _direccionOrdenEntrega.Direccion = _direction;
                _entryLugarEntregaMapa.Text = $"{_direction.Thoroughfare} {_direction.SubThoroughfare}, {_direction.SubLocality}";
                _tipoReparto = TipoReparto.Particular;
#if DEBUG
                Console.WriteLine($"||||||||||||||||||| OnReceiveResult Location Updated: {Newtonsoft.Json.JsonConvert.SerializeObject(_direction)}");
#endif
            }
            else
            {
                MystiqueNative.Models.Location.Direction _direction = new MystiqueNative.Models.Location.Direction();
                _direccionOrdenEntrega = new DireccionOrden();
                _entryLugarEntregaMapa.Text = "";
                _tipoReparto = TipoReparto.NoDefinido;
                Console.WriteLine(resultData.GetString(AddressResolverIntentService.ResultErrorExtra));
            }
            UpdateUi();
        }
        #endregion

        #region METHODS

        private void UpdateUi()
        {
            if (!CarritoViewModel.Instance.ExistenItemsEnCarrito) return;

            if (CarritoViewModel.Instance.PedidoActual.Restaurante.TieneRepartoADomicilio)
            {
                _entryLugarEntrega.Click += EntryLugarEntrega_Click;
                _entryLugarEntrega.SetBackgroundResource(Resource.Drawable.entry_white_background);
            }
            else
            {
                _tipoReparto = TipoReparto.Sucursal;
                _direccionOrdenEntrega = new DireccionOrden();
                _entryLugarEntrega.Text = $"Recoger en sucursal: {CarritoViewModel.Instance.PedidoActual.Restaurante.Direccion}";
                _entryLugarEntrega.SetBackgroundResource(Resource.Drawable.entry_white_disabled_background);
            }

            _labelRestaurante.Text = CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre;
            _labelTotal.Text = CarritoViewModel.Instance.TotalActual(_tipoReparto);
            if (_tipoReparto == TipoReparto.Particular)
            {
                _labelCargoServicio.Visibility = ViewStates.Visible;
                _labelCargoServicio.Text = string.Format(GetString(Resource.String.carrito_label_cargo), CarritoViewModel.Instance.PedidoActual.Restaurante.CostoEnvio);
            }
            else
            {
                _labelCargoServicio.Visibility = ViewStates.Gone;
            }
            _buttonTerminar.Visibility =
                _metodoPago != MetodoPago.NoDefinido
                && _tipoReparto != TipoReparto.NoDefinido
                    ? ViewStates.Visible
                    : ViewStates.Gone;
        }

        private void ShowContent()
        {
            if (CarritoViewModel.Instance.ExistenItemsEnCarrito)
            {
                _layoutContent.Visibility = ViewStates.Visible;
                _layoutNoContent.Visibility = ViewStates.Gone;
            }
            else
            {
                _layoutContent.Visibility = ViewStates.Gone;
                _layoutNoContent.Visibility = ViewStates.Visible;
            }
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
    }
}