using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Fragments
{
    public class ClienteBuscarDialogFragment : AppCompatDialogFragment
    {
        public event EventHandler<ClientSearchEventArgs> DialogClientSearchClosed;
        private TextInputEditText tietNombre;
        private TextInputEditText tietTelefono;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_fragment_buscar_cliente, container, false);

            tietNombre = view.FindViewById<TextInputEditText>(Resource.Id.tiet_buscar_cliente_nombre);
            tietTelefono = view.FindViewById<TextInputEditText>(Resource.Id.tiet_registro_cliente_busqueda_telefono);

            view.FindViewById<Button>(Resource.Id.buscar_cliente_btn_cancelar).Click += (s, ev) =>
            {
                DialogClientSearchClosed?.Invoke(this, null);
                Dismiss();
            };

            view.FindViewById<Button>(Resource.Id.buscar_cliente_btn_aceptar).Click += (s, ev) =>
            {
                DialogClientSearchClosed?.Invoke(this, new ClientSearchEventArgs { NombreCliente = tietNombre.Text, Telefono = tietTelefono.Text });
                Dismiss();
            };

            return view;
        }
    }

    public class ClientSearchEventArgs
    {
        public string NombreCliente { get; set; }
        public string Telefono { get; set; }
    }
}