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
using MystiqueNative.Models;

namespace MystiqueNative.Droid.HazPedido.Soporte
{
    public class NotificacionAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Notificacion_HP> _viewModel;
        private readonly Activity _context;

        public NotificacionAdapter(Activity context, ObservableCollection<Notificacion_HP> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { NotifyDataSetChanged(); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_haz_pedido_notificacion;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new NotificacionViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is NotificacionViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Titulo}";
            myHolder.Fecha.Text = $"Fecha: {item.FechaConFormatoEspanyol}";

            if (!item.IdPedido.HasValue)
            {
                myHolder.Folio.Visibility = ViewStates.Gone;
            }
            else
            {
                myHolder.Folio.Visibility = ViewStates.Visible;
                myHolder.Folio.Text = $"Pedido #{item.FolioPedido}";
            }

        }

        public override int ItemCount => _viewModel.Count;
    }

    public class NotificacionViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Folio { get; }
        public TextView Fecha { get; }
        public Button ButtonVer { get; }
        public View LayoutPedido { get; set; }
        public NotificacionViewHolder(View itemView, Action<RecyclerClickEventArgs> click1) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Folio = itemView.FindViewById<TextView>(Resource.Id.item_id);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            LayoutPedido = itemView.FindViewById(Resource.Id.layout_notificacion_pedido);

            ButtonVer = itemView.FindViewById<Button>(Resource.Id.button_calificar);

            itemView.FindViewById(Resource.Id.button_ver).Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}