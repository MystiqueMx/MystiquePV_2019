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
using MystiqueNative.Models.Location;

namespace MystiqueNative.Droid.HazPedido.Perfil
{
    public class DireccionPerfilAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Direction> _viewModel;

        public DireccionPerfilAdapter(Activity context, ObservableCollection<Direction> viewModel)
        {
            this._viewModel = viewModel;
            _viewModel.CollectionChanged += delegate { context.RunOnUiThread(NotifyDataSetChanged); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_perfil_direccion;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new DireccionPerfilViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is DireccionPerfilViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.Nombre}";
            myHolder.Line1.Text = $"{item.Thoroughfare} {item.SubThoroughfare}";
            myHolder.Line2.Text = $"{item.SubLocality} {item.Locality} {item.PostalCode}";
            //myHolder.Line3.Text = $" {MystiqueApp.Usuario.Telefono} ";
            myHolder.Line3.Text = $" {ViewModels.AuthViewModelV2.Instance.Usuario.Telefono} ";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class DireccionPerfilViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Line1 { get; }
        public TextView Line2 { get; }
        public TextView Line3 { get; }
        public DireccionPerfilViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Line2 = itemView.FindViewById<TextView>(Resource.Id.item_line2);
            Line3 = itemView.FindViewById<TextView>(Resource.Id.item_line3);

            itemView.FindViewById<ImageView>(Resource.Id.item_edit).Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };

            itemView.FindViewById<ImageView>(Resource.Id.item_delete).Click += delegate
            {
                click2(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}