using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;
using Android.Support.Design.Widget;
using MystiqueNative.Helpers;
using Android.Support.V7.Widget;
using BarronWellnessMovil.Droid.Helpers;
using System.Collections.ObjectModel;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V4.App;
using MystiqueNative.Models;
using Android.Support.V4.Util;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Mis Facturas", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FacturacionActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_facturacion;

        #region VIEWS

        private FrameLayout _progressBarHolder;
        private TextView _labelVacio;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var adapter = new FacturasAdapter(this, FacturacionViewModel.Instance.Facturas);

            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(adapter);

            adapter.ItemClick += Adapter_ItemClick;
            adapter.NotifyDataSetChanged();
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(FacturaDetalleActivity));
            intent.PutExtra(FacturaDetalleActivity.ExtraFacturaPosition, e.Position);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {

                //var options = ActivityOptionsCompat.MakeScaleUpAnimation(e.View, 0, 0, e.View.Width, e.View.Height);
                var p1 = Pair.Create(e.View.FindViewById(Resource.Id.label_title), "title");
                var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, p1);
                StartActivity(intent, options.ToBundle());
            }
            else
            {
                StartActivity(intent);
            }
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += SumarPuntosActivity_Click;
            _labelVacio = FindViewById<TextView>(Resource.Id.label_vacio);
        }

        protected override void OnResume()
        {
            base.OnResume();
            FacturacionViewModel.Instance.OnObtenerFacturasFinished += Instance_OnObtenerFacturasFinished;
            FacturacionViewModel.Instance.ObtenerFacturas();
        }

        protected override void OnPause()
        {
            base.OnPause();
            FacturacionViewModel.Instance.OnObtenerFacturasFinished -= Instance_OnObtenerFacturasFinished;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return true;
            }
        }

        private void Instance_OnObtenerFacturasFinished(object sender, BaseEventArgs e)
        {
            _labelVacio.Visibility =
                FacturacionViewModel.Instance.Facturas.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
        }

        private void SumarPuntosActivity_Click(object sender, EventArgs e)
        {
            if (IsInternetAvailable)
            {
                if (PermissionsHelper.ValidatePermissionsForCamera())
                {
                    StartActivity(typeof(TicketFacturaActivity));
                }
                else
                {
                    RequestPermissions(PermissionsHelper.PermissionsToRequest.ToArray(),
                        PermissionsHelper.RequestCameraId);
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PermissionsHelper.RequestCameraId:
                    var gotRequestedPermission = true;
                    foreach (var p in grantResults)
                        if (p != Permission.Granted)
                        {
                            gotRequestedPermission = false;
                        }

                    if (gotRequestedPermission)
                    {
                        StartActivity(typeof(TicketFacturaActivity));
                    }

                    break;
                default:
                    break;
            }
        }

        /*private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }

        private void StopAnimatingLogin()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }*/
    }

    public class FacturasAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Factura> _viewModel;
        private readonly BaseActivity _context;

        public FacturasAdapter(BaseActivity context, ObservableCollection<Factura> viewModel)
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
            const int id = Resource.Layout.viewholder_factura;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new FacturaViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is FacturaViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Sucursal} - {item.Estatus}";
            if (string.IsNullOrEmpty(item.Folio))
            {
                myHolder.Id.Text = $"Ticket: {item.Folio}";
                myHolder.Id.Visibility = ViewStates.Visible;
            }
            else
            {
                myHolder.Id.Visibility = ViewStates.Gone;
            }
            
            myHolder.Fecha.Text = $"Fecha: {item.FechaCompraConFormatoEspanyol}";
            myHolder.Total.Text = $"Total: {item.MontoCompraConFormatoMoneda}";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class FacturaViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Id { get; }
        public TextView Fecha { get; }
        public TextView Total { get; }

        public FacturaViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Id = itemView.FindViewById<TextView>(Resource.Id.label_item);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.label_fecha);
            Total = itemView.FindViewById<TextView>(Resource.Id.label_total);
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