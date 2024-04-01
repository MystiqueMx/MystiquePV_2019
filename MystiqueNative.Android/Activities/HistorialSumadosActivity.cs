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
    public class HistorialSumadosActivity : BaseActivity
    {
        TextView _labelTotal;
        protected override int LayoutResource => Resource.Layout.activity_historial_sumados;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GrabViews();
            SetupRecyclerView();
            _labelTotal.Text = $"Total    {HistorialViewModel.Instance.Sumados}";
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
                SupportActionBar.Title = "Puntos Sumados";
            }
        }
        
        protected override void OnPause()
        {
            base.OnPause();
        }
        private void SetupRecyclerView()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            var adapter = new Historial3Adapter(this, HistorialViewModel.Instance.MovimientosSumados);
            recyclerView.SetAdapter(adapter);
            adapter.NotifyDataSetChanged();
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
    }
    public class Historial3Adapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<MovimientoCitypoints> _viewModel;
        Activity _context;

        public Historial3Adapter(Activity context, ObservableCollection<MovimientoCitypoints> list)
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
            const int id = Resource.Layout.viewholder_historial_sumados;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new Historial3ViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }
        
        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel[position];
            var myHolder = holder as Historial3ViewHolder;

            myHolder.Title.Text = $"{beneficio.PuntosAsInt}";
            myHolder.Ticket.Text = $"{beneficio.Folio}";
            myHolder.Monto.Text = $"{beneficio.MontoAsInt:C}";
            myHolder.Fecha.Text = $"{beneficio.FechaCompraConFormatoEspanyol}";
        }
        public override int ItemCount => _viewModel.Count;
    }
    public class Historial3ViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Fecha { get; set; }
        public TextView Ticket { get; set; }
        public TextView Monto { get; set; }
        public Historial3ViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener, Action<RecyclerClickEventArgs> deleteListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.label_fecha);
            Ticket = itemView.FindViewById<TextView>(Resource.Id.label_ticket);
            Monto = itemView.FindViewById<TextView>(Resource.Id.label_monto);
        }

    }

}