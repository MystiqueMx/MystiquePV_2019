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

namespace MystiqueNative.Droid.HazPedido.Tarjetas
{
    public class TarjetaAdapter : BaseRecyclerViewAdapter
    {
        private readonly ObservableCollection<Card> _viewModel;
        private readonly Activity _context;

        public TarjetaAdapter(Activity context, ObservableCollection<Card> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _viewModel.CollectionChanged += delegate { this.NotifyDataSetChanged(); };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            const int id = Resource.Layout.item_carrito_tarjetas;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new TarjetaViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];

            if (!(holder is TarjetaViewHolder myHolder)) return;

            myHolder.Title.Text = $"{item.MaskedCardNumber}";
            myHolder.Line1.Text = $"{item.HolderName}";

            //TODO REMOVE CONEKTA
            //NO BANK NAME
            myHolder.Line2.Text = $"{item.Brand.Humanize(LetterCasing.Title)}";
            //myHolder.Line2.Text = $"{item.Brand.Humanize(LetterCasing.Title)}, {item.BankName}";
        }

        public override int ItemCount => _viewModel.Count;
    }

    public class TarjetaViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public TextView Line1 { get; }
        public TextView Line2 { get; }
        public TarjetaViewHolder(View itemView, Action<RecyclerClickEventArgs> click1,
            Action<RecyclerClickEventArgs> click2) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.item_title);
            Line1 = itemView.FindViewById<TextView>(Resource.Id.item_line1);
            Line2 = itemView.FindViewById<TextView>(Resource.Id.item_line2);

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