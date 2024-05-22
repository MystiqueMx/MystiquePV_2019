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
using FFImageLoading;
using FFImageLoading.Views;

namespace MystiqueNative.Droid.HazPedido
{
    public class MenuRestauranteHazPedidoAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Models.Menu.MenuRestaurante> _viewModel;
        private readonly Activity _context;

        public MenuRestauranteHazPedidoAdapter(Activity context, ObservableCollection<Models.Menu.MenuRestaurante> viewModel)
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
            const int id = Resource.Layout.item_restaurante_menu;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new MenuRestauranteHazPedidoViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is MenuRestauranteHazPedidoViewHolder myHolder)) return;

            myHolder.Image.SetImageBitmap(null);
            myHolder.Image.SetImageResource(Resource.Drawable.Cargandox3);
            myHolder.Title.Text = $"{item.Nombre}";
            ImageService.Instance.LoadUrl(item.ImagenUrl)
                .DownSampleInDip(height: 100)
                .Into(myHolder.Image);
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class MenuRestauranteHazPedidoViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public ImageViewAsync Image { get; }

        public MenuRestauranteHazPedidoViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.item_image);

            itemView.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}