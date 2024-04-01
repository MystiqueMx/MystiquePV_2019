using System;

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
using System.Collections.ObjectModel;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HistorialCanjeadosActivity : BaseActivity
    {
        TextView _labelTotal;
        protected override int LayoutResource => Resource.Layout.activity_historial;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            SetupRecyclerView();
            _labelTotal.Text = string.Format("Total    {0}", HistorialViewModel.Instance.Canjeados);
        }

        private void GrabViews()
        {
            _labelTotal = FindViewById<TextView>(Resource.Id.historial_total);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (SupportActionBar != null)
            {
                SupportActionBar.Title = "Puntos Canjeados";
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
            }
            return true;
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        private void SetupRecyclerView()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            var adapter = new Historial2Adapter(this, HistorialViewModel.Instance.MovimientosCanjeados);
            recyclerView.SetAdapter(adapter);
            adapter.NotifyDataSetChanged();
        }
    }
    public class Historial2Adapter : BaseRecyclerViewAdapter
    {
        ObservableCollection<MovimientoCitypoints> _viewModel;
        Activity _context;

        public Historial2Adapter(Activity context, ObservableCollection<MovimientoCitypoints> list)
        {
            this._viewModel = list;
            this._context = context;

            this._viewModel.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.viewholder_historial;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new Historial2ViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel[position];
            var myHolder = holder as Historial2ViewHolder;

            myHolder.Title.Text = string.Format("{0}", beneficio.PuntosAsInt);
            myHolder.Producto.Text = string.Format("{0}", beneficio.Producto);
            myHolder.Fecha.Text = string.Format("{0}",beneficio.FechaRegistroConFormatoEspanyol);
        }
        public override int ItemCount => _viewModel.Count;
    }
    public class Historial2ViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Fecha { get; set; }
        public TextView Producto { get; set; }
        public Historial2ViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> deleteListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.label_fecha);
            Producto = itemView.FindViewById<TextView>(Resource.Id.label_producto);
        }

    }

}