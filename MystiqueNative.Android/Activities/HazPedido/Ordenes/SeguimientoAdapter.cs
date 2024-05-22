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
    public class SeguimientoAdapter : BaseRecyclerViewAdapter
    {
        private readonly IList<SeguimientoPedido> _viewModel;
        private readonly Activity _context;

        public SeguimientoAdapter(Activity context, IList<SeguimientoPedido> viewModel)
        {
            this._viewModel = viewModel;
            if (_viewModel == null) _viewModel = new List<SeguimientoPedido>();
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_seguimiento;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new SeguimientoViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is SeguimientoViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Comentario}";
            myHolder.Hora.Text = $"{item.Fecha:hh:mm tt}";
        }

        public override int ItemCount => _viewModel.Count();
    }

    public class SeguimientoViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Hora { get; }
        public SeguimientoViewHolder(View itemView) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Hora = itemView.FindViewById<TextView>(Resource.Id.item_hora);
        }
    }
}