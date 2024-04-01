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

namespace MystiqueNative.Droid.Activities
{
    public class ClientesAdapter : BaseRecyclerViewAdapter
    {

        private readonly ObservableCollection<Models.Clientes.ClienteCallCenter> _viewModel;
        private readonly Activity _context;

        public ClientesAdapter(Activity context, ObservableCollection<Models.Clientes.ClienteCallCenter> viewModel)
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
            const int id = Resource.Layout.item_cliente;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new ClientesViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel[position];
            if (!(holder is ClientesViewHolder myHolder)) return;

            myHolder.NombreCliente.Text = item.nombreCompleto;
            myHolder.TelefonoCliente.Text = item.telefono;
        }

        public override int ItemCount => _viewModel.Count;
    }
}