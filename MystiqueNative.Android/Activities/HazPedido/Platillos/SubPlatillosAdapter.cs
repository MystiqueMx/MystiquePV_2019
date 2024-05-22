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

namespace MystiqueNative.Droid.HazPedido.Platillos
{
    class SubPlatillosAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Models.Platillos.BasePlatilloMultiNivel> _viewModel;
        private readonly Activity _context;

        public SubPlatillosAdapter(Activity context, ObservableCollection<Models.Platillos.BasePlatilloMultiNivel> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { NotifyDataSetChanged(); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_restaurante_subplatillo;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new SubPlatilloRestauranteViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is SubPlatilloRestauranteViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Descripcion}";
            if (item.Completado)
            {
                myHolder.Seleccionado.Visibility = ViewStates.Visible;
            }
            if (item.Precio > 0)
            {
                myHolder.Precio.Text = $"{item.Precio:C} MXN";
                myHolder.Precio.Visibility = ViewStates.Visible;
            }
            else
            {
                myHolder.Precio.Visibility = ViewStates.Gone;
            }

        }

        public override int ItemCount => _viewModel.Count;
    }

    public class SubPlatilloRestauranteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }

        public TextView Precio { get; set; }

        public ImageView Seleccionado { get; set; }

        public SubPlatilloRestauranteViewHolder(View itemView, Action<RecyclerClickEventArgs> click1) : base(itemView)
        {

            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Precio = itemView.FindViewById<TextView>(Resource.Id.item_precio);
            Seleccionado = itemView.FindViewById<ImageView>(Resource.Id.item_image);

            itemView.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

        }

    }
}