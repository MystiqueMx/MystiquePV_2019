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

namespace MystiqueNative.Droid.HazPedido.Historial
{
    public class HistorialAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<OrdenHistorial> _viewModel;
        private readonly Activity _context;

        public HistorialAdapter(Activity context, ObservableCollection<OrdenHistorial> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { NotifyDataSetChanged(); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_orden_historial;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new HistorialViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is HistorialViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Nombre}";
            myHolder.Folio.Text = $"Pedido #{item.Folio}";
            myHolder.Fecha.Text = $"Fecha: {item.FechaConFormatoEspanyol}";
            myHolder.Total.Text = $"Total: {item.Total:C} MXN";


            myHolder.ButtonCalificar.Visibility = item.Calificacion.HasValue ? ViewStates.Gone : ViewStates.Visible;
            myHolder.Imagen.Visibility = item.Calificacion.HasValue ? ViewStates.Visible : ViewStates.Gone;
            if (!item.Calificacion.HasValue) return;

            switch (item.Calificacion.Value)
            {
                case 5:
                    myHolder.Imagen.SetImageResource(Resource.Drawable.star_5);
                    break;
                case 4:
                    myHolder.Imagen.SetImageResource(Resource.Drawable.star_4);
                    break;
                case 3:
                    myHolder.Imagen.SetImageResource(Resource.Drawable.star_3);
                    break;
                case 2:
                    myHolder.Imagen.SetImageResource(Resource.Drawable.star_2);
                    break;
                default:
                    myHolder.Imagen.SetImageResource(Resource.Drawable.star_1);
                    break;
            }
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class HistorialViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Folio { get; }
        public TextView Fecha { get; }
        public TextView Total { get; }
        public ImageView Imagen { get; }
        public Button ButtonCalificar { get; }
        public HistorialViewHolder(View itemView, Action<RecyclerClickEventArgs> click1, Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Folio = itemView.FindViewById<TextView>(Resource.Id.item_folio);
            Fecha = itemView.FindViewById<TextView>(Resource.Id.item_fecha);
            Total = itemView.FindViewById<TextView>(Resource.Id.item_total);
            Imagen = itemView.FindViewById<ImageView>(Resource.Id.image_calificacion);

            ButtonCalificar = itemView.FindViewById<Button>(Resource.Id.button_calificar);
            itemView.FindViewById(Resource.Id.button_ver).Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
            ButtonCalificar.Click += delegate
            {
                click2(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}