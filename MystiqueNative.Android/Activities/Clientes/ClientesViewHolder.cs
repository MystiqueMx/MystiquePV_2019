using System;
using System.Collections.Generic;
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

namespace MystiqueNative.Droid.Activities
{
    public class ClientesViewHolder : RecyclerView.ViewHolder
    {
        public TextView NombreCliente { get; }

        public TextView DireccionCliente { get; }

        public TextView TelefonoCliente { get; }

        public CardView CardViewCliente { get; }

        public ClientesViewHolder(View itemView, Action<RecyclerClickEventArgs> click1) : base(itemView)
        {
            NombreCliente = itemView.FindViewById<TextView>(Resource.Id.txv_cliente_nombre_completo);
            DireccionCliente = itemView.FindViewById<TextView>(Resource.Id.txv_cliente_direccion);
            TelefonoCliente = itemView.FindViewById<TextView>(Resource.Id.txv_cliente_telefono);
            CardViewCliente = itemView.FindViewById<CardView>(Resource.Id.card_view_clientes);

            CardViewCliente.Click += delegate
            {
                click1(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            };
        }
    }
}