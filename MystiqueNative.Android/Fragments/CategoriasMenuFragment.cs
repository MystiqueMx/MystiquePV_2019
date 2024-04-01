using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using BarronWellnessMovil.Droid.Helpers;
using Android.Support.V7.Widget;
using FFImageLoading.Views;
using System.Collections.ObjectModel;
using FFImageLoading;

namespace MystiqueNative.Droid.Fragments
{
    public class CategoriasMenuFragment : BaseFragment
    {
        public event EventHandler<EventArgs> OnCategoriaSelected;
        protected override int LayoutResource => Resource.Layout.fragment_categorias_menu;

        readonly ObservableCollection<Categoria> _demo = new ObservableCollection<Categoria>
        {
            new Categoria { Id = 1, Titulo = "Ensaladas", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 2, Titulo = "Baguettes", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 3, Titulo = "Paninis", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 4, Titulo = "Hamburguesas", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 5, Titulo = "Sándwiches", ImagenUrl = "https://picsum.photos/300/200/?random" },
        };
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var layout = inflater.Inflate(LayoutResource, container, false);
            SetupRecyclerView(layout);
            return layout;
        }

        private void SetupRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var adapter = new CategoriasMenuAdapter(this, _demo);
            var dividerItemDecoration = new DividerItemDecoration(Context, (int)Orientation.Vertical);

            recyclerView.AddItemDecoration(dividerItemDecoration);
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(adapter);

            adapter.ItemClick += Adapter_ItemClick;
            adapter.NotifyDataSetChanged();
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            OnCategoriaSelected?.Invoke(this, new EventArgs());
        }

        public void BecameVisible()
        {
        }
    }
    public class CategoriasMenuAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Categoria> _viewModel;
        private readonly BaseFragment _context;

        public CategoriasMenuAdapter(BaseFragment context, ObservableCollection<Categoria> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context;

            this._viewModel.CollectionChanged += (sender, args) =>
            {
                this._context.Activity.RunOnUiThread(NotifyDataSetChanged);
            };
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.viewholder_categoria_menu;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new CategoriasMenuViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel[position];

            if (!(holder is CategoriasMenuViewHolder myHolder)) return;

            myHolder.Title.Text = beneficio.Titulo;
            if (string.IsNullOrEmpty(beneficio.ImagenUrl))
            {
                //ImageService.Instance
                //    .LoadEmbeddedResource("@drawable/imgMenuCPCanjear")
                //    .DownSampleInDip(width: 400)
                //    .IntoAsync(myHolder.Image);
            }
            else
            {
                ImageService.Instance
                    .LoadUrl(beneficio.ImagenUrl)
                    .DownSampleInDip(height: 80)
                    .IntoAsync(myHolder.Image);
            }
        }
        public override int ItemCount => _viewModel.Count;
    }
    public class CategoriasMenuViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public ImageViewAsync Image { get; set; }
        public CategoriasMenuViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.image_icon);
            itemView.Click += delegate
            {
                clickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
            itemView.LongClick += delegate
            {
                longClickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
        }
    }
    public class Categoria
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string ImagenUrl { get; set; }
    }
}