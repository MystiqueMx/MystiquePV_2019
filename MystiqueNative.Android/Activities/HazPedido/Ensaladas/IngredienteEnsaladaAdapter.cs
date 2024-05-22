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

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    public class IngredienteEnsaladaAdapter : BaseRecyclerViewAdapter
    {
        private readonly List<Models.Ensaladas.IngredienteEnsalada> _viewModel;
        private readonly Activity _context;

        public IngredienteEnsaladaAdapter(Activity context, List<Models.Ensaladas.IngredienteEnsalada> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_ensalada_ingrediente;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new IngredienteEnsaladaViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is IngredienteEnsaladaViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Descripcion}";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class IngredienteEnsaladaViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }

        public IngredienteEnsaladaViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);

            itemView.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

        }
    }
}