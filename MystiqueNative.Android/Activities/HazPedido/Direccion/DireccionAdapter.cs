using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.Models.Location;

namespace MystiqueNative.Droid.HazPedido.Direccion
{
    public class DireccionAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Direction> _viewModel;
        private readonly Activity _context;

        public DireccionAdapter(Activity context, ObservableCollection<Direction> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { this.NotifyDataSetChanged(); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_carrito_direccion;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new DireccionViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is DireccionViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Nombre}";
            myHolder.Line1.Text = $"{item.Thoroughfare} {item.SubThoroughfare}";
            myHolder.Line2.Text = $"{item.SubLocality} {item.Locality} {item.PostalCode}";
            //myHolder.Line3.Text = $" {MystiqueApp.Usuario.Telefono} ";
            myHolder.Line3.Text = $" {ViewModels.AuthViewModelV2.Instance.Usuario.Telefono} ";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class DireccionViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Line1 { get; }
        public TextView Line2 { get; }
        public TextView Line3 { get; }
        public DireccionViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Line2 = itemView.FindViewById<TextView>(Resource.Id.item_line2);
            Line3 = itemView.FindViewById<TextView>(Resource.Id.item_line3);

            itemView.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

            itemView.LongClick += delegate
            {
                click2(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}