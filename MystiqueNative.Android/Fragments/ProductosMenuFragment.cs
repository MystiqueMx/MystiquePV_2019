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
    public class ProductosMenuFragment : BaseFragment
    {
        
        protected override int LayoutResource => Resource.Layout.fragment_categorias_menu;

        readonly ObservableCollection<Categoria> _demo = new ObservableCollection<Categoria>
        {
            new Categoria { Id = 1, Titulo = "Baguette Alaska", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 2, Titulo = "Baguette Cancun", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 3, Titulo = "Baguette Italia", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 4, Titulo = "Baguette Italia 2", ImagenUrl = "https://picsum.photos/300/200/?random" },
            new Categoria { Id = 5, Titulo = "Baguette Italia 3", ImagenUrl = "https://picsum.photos/300/200/?random" },
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

            adapter.NotifyDataSetChanged();
        }

        public void BecameVisible()
        {
        }
    }
}