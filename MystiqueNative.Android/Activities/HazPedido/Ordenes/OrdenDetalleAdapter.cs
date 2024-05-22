using System;
using System.Collections.Generic;
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
    public class OrdenDetalleAdapter : BaseRecyclerViewAdapter
    {
        private readonly IList<DetallePedido> _viewModel;
        private readonly Activity _context;

        public OrdenDetalleAdapter(Activity context, IList<DetallePedido> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_carrito;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new DetallePedidoViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is DetallePedidoViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Nombre}";
            myHolder.Precio.Text = $"{item.Precio:C}";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class DetallePedidoViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Precio { get; }
        public DetallePedidoViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Precio = itemView.FindViewById<TextView>(Resource.Id.item_precio);

            itemView.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}