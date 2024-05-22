using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.Models.Orden;

namespace MystiqueNative.Droid.HazPedido.Ordenes
{
    public class OrdenActivaAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Orden> _viewModel;
        private readonly Activity _context;

        public OrdenActivaAdapter(Activity context, ObservableCollection<Orden> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { _context.RunOnUiThread(NotifyDataSetChanged); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_orden;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new OrdenActivaViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is OrdenActivaViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Restaurante}";
            myHolder.Folio.Text = $"Pedido #{item.Folio}";
            myHolder.Fecha.Text = $"Fecha: {item.FechaConFormatoEspanyol}";
            myHolder.Total.Text = $"Total: {item.Total:C} MXN";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class OrdenActivaViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Folio { get; }
        public TextView Fecha { get; }
        public TextView Total { get; }
        public OrdenActivaViewHolder(View itemView, Action<RecyclerClickEventArgs> click1) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Folio = itemView.FindViewById<TextView>(Resource.Id.item_folio);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.item_fecha);
            Total = itemView.FindViewById<TextView>(Resource.Id.item_total);
            itemView.FindViewById(Resource.Id.button_ver).Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}