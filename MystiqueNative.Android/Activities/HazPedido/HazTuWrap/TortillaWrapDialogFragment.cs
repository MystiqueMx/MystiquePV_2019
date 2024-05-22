using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MystiqueNative.Models.HazTuWrap;

namespace MystiqueNative.Droid.HazPedido.HazTuWrap
{
    public class TortillaWrapDialogFragment : AppCompatDialogFragment
    {
        public event EventHandler<TortillaDialogEventArgs> DialogClosed;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_haz_pedido_fragment_tortilla_wrap, container, false);

            view.FindViewById<LinearLayout>(Resource.Id.tortilla_opcion1).Click += (s, ev) =>
            {
                //Tortilla de Brocoli
                DialogClosed?.Invoke(this, new TortillaDialogEventArgs() { TipoTortilla = TortillaWrap.Brocoli });
                Dismiss();
            };

            view.FindViewById<LinearLayout>(Resource.Id.tortilla_opcion2).Click += (s, ev) =>
            {
                //Tortilla de Chipotle
                DialogClosed?.Invoke(this, new TortillaDialogEventArgs() { TipoTortilla = TortillaWrap.Chipotle });
                Dismiss();
            };

            view.FindViewById<LinearLayout>(Resource.Id.tortilla_opcion3).Click += (s, ev) =>
            {
                //Tortilla Normal
                DialogClosed?.Invoke(this, new TortillaDialogEventArgs() { TipoTortilla = TortillaWrap.Normal });
                Dismiss();
            };

            return view;
        }
    }

    public class TortillaDialogEventArgs
    {
        public TortillaWrap TipoTortilla { get; set; }
    }
}