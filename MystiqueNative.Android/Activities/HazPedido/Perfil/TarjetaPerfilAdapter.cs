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
using Humanizer;
using MystiqueNative.Models.OpenPay;

namespace MystiqueNative.Droid.HazPedido.Perfil
{    public class TarjetaPerfilAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Card> _viewModel;

        public TarjetaPerfilAdapter(Activity context, ObservableCollection<Card> viewModel)
        {
            this._viewModel = viewModel;
            _viewModel.CollectionChanged += delegate { context.RunOnUiThread(NotifyDataSetChanged); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_perfil_tarjeta;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new TarjetaPerfilViewHolder(itemView, OnClick, OnClick2);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is TarjetaPerfilViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.MaskedCardNumber}";
            myHolder.Line1.Text = $"{item.HolderName}";
            //TODO REMOVE CONEKTA
            //NO BANK NAME
            myHolder.Line2.Text = $"{item.Brand.Humanize(LetterCasing.Title)}";
            //myHolder.Line2.Text = $"{item.Brand.Humanize(LetterCasing.Title)}, {item.BankName}";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class TarjetaPerfilViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Line1 { get; }
        public TextView Line2 { get; }
        public TarjetaPerfilViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Line2 = itemView.FindViewById<TextView>(Resource.Id.item_line2);

            itemView.FindViewById<ImageView>(Resource.Id.item_edit).Visibility = ViewStates.Gone;
            //itemView.FindViewById<ImageView>(Resource.Id.item_edit).Click += delegate
            //{
            //    click1(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            //};

            itemView.FindViewById<ImageView>(Resource.Id.item_delete).Click += delegate
            {
                click2(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}