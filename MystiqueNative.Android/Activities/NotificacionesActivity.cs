using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Fragments;
using Android.Support.V4.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.ViewModels;
using FFImageLoading;
using FFImageLoading.Views;
using Android.Support.Design.Widget;
using MystiqueNative.Models;
using MystiqueNative.Droid.Animations;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingHasbroActivity")]
    public class NotificacionesActivity : BaseActivity
    {
        public static string IdSucursal;
        public static string Sucursal;

        private SwipeRefreshLayout _refreshLayout;
        private NotificacionesAdapter _adapter;
        private ProgressBar _progress;
        private TextView _labelVacio;
        protected override int LayoutResource => Resource.Layout.activity_notificaciones;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _labelVacio = FindViewById<TextView>(Resource.Id.label_vacio);
            SetupRecyclerView();

            // SetupMenu();
            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
            _refreshLayout.Enabled = false;
            
        }

        protected override void OnResume()
        {
            base.OnResume();

            _adapter.ItemClick += Adapter_ItemClick;
            if (IsInternetAvailable)
            {
                if (MainApplication.ViewModelNotificaciones.Notificaciones.Count == 0)
                    MainApplication.ViewModelNotificaciones.ObtenerNotificaciones();
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


            if (SupportActionBar != null)
            {
                SupportActionBar.Title = GetString(Resource.String.notificaciones_title);
            }

            MainApplication.ViewModelNotificaciones.LimpiarNotificaciones();
            MainApplication.ViewModelNotificaciones.PropertyChanged += ViewModelNotificaciones_PropertyChanged;
            MainApplication.ViewModelNotificaciones.Notificaciones.CollectionChanged += Notificaciones_CollectionChanged;
        }

        private void SetupRecyclerView()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(_adapter = new NotificacionesAdapter(this, MainApplication.ViewModelNotificaciones));

            _refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refresh_layout);
            _refreshLayout.SetColorSchemeColors(Resource.Color.accent);

            _progress = FindViewById<ProgressBar>(Resource.Id.progressbar);

        }

        private void ViewModelNotificaciones_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBusy" when MainApplication.ViewModelNotificaciones.IsBusy:
                    _progress.Visibility = ViewStates.Visible;
                    break;
                case "IsBusy":
                    _progress.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void Notificaciones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _labelVacio.Visibility = (sender as ObservableCollection<Notificacion>).Count == 0
                ? ViewStates.Visible
                : ViewStates.Gone;
        }

        protected override void OnPause()
        {
            base.OnPause();
            _adapter.ItemClick -= Adapter_ItemClick;
            MainApplication.ViewModelNotificaciones.PropertyChanged-= ViewModelNotificaciones_PropertyChanged;
            //btnHomeAboutUs.Click -= BtnHomeAboutUs_Click;
            //ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = MainApplication.ViewModelNotificaciones.Notificaciones[e.Position];
            SendMessage(item.Contenido, item.Titulo);
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
        #region DRAWER

        private Android.Support.V4.Widget.DrawerLayout _drawerLayout;
        private Android.Support.Design.Widget.NavigationView _navigationView;
        private const int NavigationItemId = Resource.Id.nav_item_notificaciones;

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

    internal class NotificacionesAdapter : BaseRecyclerViewAdapter
    {
        private readonly NotificacionesViewModel _viewModel;
        private Activity _context;

        public NotificacionesAdapter(Activity context, NotificacionesViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._context = context;

            this._viewModel.Notificaciones.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            const int id = Resource.Layout.viewholder_notificaciones;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new NotificacionesViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel.Notificaciones[position];

            if (holder is NotificacionesViewHolder myHolder)
            {
                myHolder.Title.Text = beneficio.Titulo;
            }

            //if (string.IsNullOrEmpty(beneficio.ImgBeneficio))
            //{
            //    ImageService.Instance
            //        .LoadEmbeddedResource("@drawable/imgMenuCPCanjear")
            //        .DownSampleInDip(width: 400)
            //        .IntoAsync(myHolder.Image);
            //}
            //else
            //{
            //    ImageService.Instance
            //        .LoadUrl(beneficio.ImgBeneficio)
            //        .DownSampleInDip(width: 400)
            //        .IntoAsync(myHolder.Image);
            //}
        }
        public override int ItemCount => _viewModel.Notificaciones.Count;
    }

    public class NotificacionesViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        //public ImageViewAsync Image { get; set; }
        public NotificacionesViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            //Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.image_icon);
            itemView.Click += delegate
            {
                clickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
            itemView.LongClick += delegate
            {
                longClickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
        }

    }
}