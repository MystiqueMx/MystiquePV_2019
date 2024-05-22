using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.HazPedido.Direccion
{
    public class DireccionesBottomSheet : BottomSheetDialogFragment
    {
        public static DireccionesBottomSheet Instance => new DireccionesBottomSheet();
        public event EventHandler<System.EventArgs> OnEditSelected;
        public event EventHandler<System.EventArgs> OnDeleteSelected;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(BottomSheetDialogFragment.StyleNormal, Resource.Style.CustomBottomSheetDialogTheme);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_haz_pedido_bottomsheet_direcciones, container, false);
            view.FindViewById(Resource.Id.button_editar).Click += delegate
            {
                OnEditSelected?.Invoke(this, System.EventArgs.Empty);
                Dismiss();
            };
            view.FindViewById(Resource.Id.button_eliminar).Click += delegate
            {
                OnDeleteSelected?.Invoke(this, System.EventArgs.Empty);
                Dismiss();
            };
            view.FindViewById(Resource.Id.button_cancel).Click += delegate
            {
                Dismiss();
            };
            return view;
        }
    }
}