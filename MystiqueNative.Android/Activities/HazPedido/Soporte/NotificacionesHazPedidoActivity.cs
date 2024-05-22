using System;
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
using BarronWellnessMovil.Droid.Helpers;
using FFImageLoading;
using FFImageLoading.Views;
using MystiqueNative.Droid.HazPedido.Historial;
using MystiqueNative.Droid.HazPedido.Ordenes;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Soporte
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask, Label = "Mis notificaciones")]
    public class NotificacionesHazPedidoActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_notificaciones;
        protected override int BackButtonIcon => Resource.Drawable.ic_menu_white_24dp;
        //protected override bool DisallowNotLogged => true;

        #region EXPORTS


        #endregion

        #region VIEWS

        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private ImageViewAsync _headerImage;
        private TextView _headerTitle;
        private RecyclerView _recyclerView;

        #endregion

        #region FIELDS
        private NotificacionAdapter _adapter;
        private View _viewNoContent;
        private FrameLayout _progressBarHolder;
        private static GoogleApiClient _mGoogleApiClient;
        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            _viewNoContent = FindViewById(Resource.Id.no_content_view);

            _navigationView.Menu.FindItem(Resource.Id.nav_item_home_haz_pedido).SetChecked(true);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            var header = _navigationView.GetHeaderView(0);
            _headerImage = header.FindViewById<ImageViewAsync>(Resource.Id.header_profile_image);
            _headerTitle = header.FindViewById<TextView>(Resource.Id.header_profile_name);
            SetupRecyclerView();

            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestEmail()
            .Build();

            _mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .Build();
            _mGoogleApiClient.Connect();
        }

        private void SetupRecyclerView()
        {
            _adapter = new NotificacionAdapter(this, ViewModels.NotificacionesViewModel.Instance.NotificacionesHazPedido);
            _adapter.ItemClick += Adapter_ItemClick;
            _recyclerView.HasFixedSize = true;
            _recyclerView.SetAdapter(_adapter);
            _adapter.NotifyDataSetChanged();
        }


        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
            var drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, _drawerLayout, Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_up_description);
            _drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
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
            ViewModels.NotificacionesViewModel.Instance.OnObtenerNotificacionesHPFinished += Instance_OnObtenerNotificacionesHPFinished;
            StartAnimatingLogin();
            await ViewModels.NotificacionesViewModel.Instance.ObtenerNotificacionesHP();
            await ViewModels.NotificacionesViewModel.Instance.MarcarNotificacionesHPComoLeidas();
        }

        private void Instance_OnObtenerNotificacionesHPFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            StopAnimatingLogin();
            _viewNoContent.Visibility = ViewModels.NotificacionesViewModel.Instance.NotificacionesHazPedido.Count > 0
                ? ViewStates.Gone
                : ViewStates.Visible;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.NotificacionesViewModel.Instance.OnObtenerNotificacionesHPFinished -= Instance_OnObtenerNotificacionesHPFinished;
            StopAnimatingLogin(false);
        }

        #endregion

        #region NAVIGATION
        private void SetupNavigationView()
        {
            _navigationView.Menu.FindItem(Resource.Id.nav_item_notifications_haz_pedido)
                .SetChecked(true);
            //if (MainApplication.Instance.IsLoggedIn)
            //{
            //    _headerTitle.Text = $"{MainApplication.Instance.Usuario.Nombre} {MainApplication.Instance.Usuario.Paterno}";
            //}
            _headerTitle.Text = $"{AuthViewModelV2.Instance.Usuario.NombreCompleto}";

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
                    StartActivity(typeof(NotificacionesHazPedidoActivity));
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

        private void StopAnimatingLogin(bool animate = true)
        {
            if (animate)
            {

                var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
                {
                    Duration = 100
                };
                _progressBarHolder.Animation = outAnimation;
            }
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion
        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = ViewModels.NotificacionesViewModel.Instance.NotificacionesHazPedido[e.Position];

            if (item.IdPedido.HasValue)
            {
                if (item.FechaPedido.GetValueOrDefault(DateTime.Now.Date) == DateTime.Now.Date)
                {
                    StartActivity(MystiqueApp.PedidosActivos == 1
                        ? typeof(OrdenActivaActivity)
                        : typeof(OrdenesActivasActivity));
                }
                else
                {
                    StartActivity(typeof(HistorialActivity));
                }

                OverridePendingTransition(0, 0);
            }
            else
            {
                SendMessage(item.Descripcion, item.Titulo);
            }

        }
    }
}