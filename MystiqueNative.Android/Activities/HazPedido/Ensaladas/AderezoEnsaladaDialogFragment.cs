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
using MystiqueNative.Models.Ensaladas;

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    public class AderezoEnsaladaDialogFragment : AppCompatDialogFragment
    {
        public event EventHandler<AderezoDialogEventArgs> DialogClosed;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_haz_pedido_fragment_aderezo_ensalada, container, false);

            view.FindViewById<LinearLayout>(Resource.Id.ensaladas_aderezo_opcion1).Click += (s, ev) =>
            {
                //EnEnsalada
                DialogClosed?.Invoke(this, new AderezoDialogEventArgs() { TipoAderezo = AderezoEnsalada.EnEnsalada });
                Dismiss();
            };

            view.FindViewById<LinearLayout>(Resource.Id.ensaladas_aderezo_opcion2).Click += (s, ev) =>
            {
                //PorSeparado
                DialogClosed?.Invoke(this, new AderezoDialogEventArgs() { TipoAderezo = AderezoEnsalada.PorSeparado });
                Dismiss();
            };

            return view;
        }
    }
    public class AderezoDialogEventArgs
    {
        public AderezoEnsalada TipoAderezo { get; set; }
    }
}