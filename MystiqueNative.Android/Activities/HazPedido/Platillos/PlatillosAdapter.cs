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

namespace MystiqueNative.Droid.HazPedido.Platillos
{
    public class PlatillosRestauranteAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Models.Platillos.Platillo> _viewModel;
        private readonly Activity _context;
        private readonly bool _modoLectura;
        public PlatillosRestauranteAdapter(Activity context, ObservableCollection<Models.Platillos.Platillo> viewModel, bool modoLectura)
        {
            this._viewModel = viewModel;
            _modoLectura = modoLectura;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._viewModel.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_restaurante_platillo;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new PlatilloRestauranteViewHolder(itemView, OnClick, OnClick2, OnLongClick, _modoLectura);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is PlatilloRestauranteViewHolder myHolder)) return;

            myHolder.Image.SetImageBitmap(null);
            myHolder.Image.SetImageResource(Resource.Drawable.Cargandox3);

            myHolder.Title.Text = $"{item.Nombre}";
            if (item.Precio > 0)
            {
                myHolder.Precio.Text = $"{item.Precio:C} MXN";
                myHolder.Precio.Visibility = ViewStates.Visible;
            }
            else
            {
                myHolder.Precio.Visibility = ViewStates.Gone;
            }

            myHolder.Line1.Text = $"{item.Descripcion}";
            myHolder.Count.Text = $"{ViewModels.CarritoViewModel.Instance.ConteoPlatillosPorId(item.Id)}";
            ImageService.Instance.LoadUrl(item.ImagenUrl)
                .DownSampleInDip(height: 100)
                .Into(myHolder.Image);
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class PlatilloRestauranteViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }

        public ImageButton More { get; set; }

        public ImageButton Less { get; set; }

        public TextView Count { get; set; }

        public TextView Line1 { get; set; }

        public TextView Precio { get; set; }

        public ImageViewAsync Image { get; set; }

        public Button Notas { get; set; }

        public PlatilloRestauranteViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2, Action<RecyclerClickEventArgs> click3, bool modoLectura) : base(itemView)
        {

            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Precio = itemView.FindViewById<TextView>(Resource.Id.item_precio);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Count = itemView.FindViewById<TextView>(Resource.Id.item_label_counter);
            Less = itemView.FindViewById<ImageButton>(Resource.Id.item_button_minus);
            More = itemView.FindViewById<ImageButton>(Resource.Id.item_button_plus);
            Notas = itemView.FindViewById<Button>(Resource.Id.item_button_notas);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.item_image);

            Title.Selected = true;

            Less.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

            More.Click += delegate
            {
                click2(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

            if (modoLectura)
            {
                itemView.FindViewById(Resource.Id.linearLayout1).Visibility = ViewStates.Gone;
            }
        }

    }
}