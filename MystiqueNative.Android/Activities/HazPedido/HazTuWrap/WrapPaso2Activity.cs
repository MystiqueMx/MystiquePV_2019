using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using Com.Google.Android.Flexbox;
using MystiqueNative.Droid.HazPedido.Ensaladas;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils.Flexbox;
using MystiqueNative.Droid.Utils.Views;
using ViewModel = MystiqueNative.ViewModels.EnsaladaPasoDosViewModel;

namespace MystiqueNative.Droid.HazPedido.HazTuWrap
{
    //NOTE: espejo de los pasos de ensalada, se reciclan clases, modelos y palabras como ENSALADA
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/wrap_armar_title")]
    public class WrapPaso2Activity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_wrap_paso2;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #region FIELDS

        private IngredienteEnsaladaAdapter _adapterBarraFria;

        #endregion

        #region VIEWS

        private FlexboxLayout _flexbox;
        private TextView _labelBarraFria;
        private Button _buttonNext;
        private bool _advertenciaExtra;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            _advertenciaExtra = true;
            ViewModel.Instance.ReiniciarPaso();

        }

        private void GrabViews()
        {
            //var recyclerViewChips = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view1);
            _flexbox = FindViewById<FlexboxLayout>(Resource.Id.flexbox);
            _labelBarraFria = FindViewById<TextView>(Resource.Id.ensaladas_categoria3_counter);
            _buttonNext = FindViewById<Button>(Resource.Id.button_next);
            recyclerView.HasFixedSize = true;

            _adapterBarraFria = new IngredienteEnsaladaAdapter(this, ViewModel.Instance.ListaBarraFria);

            recyclerView.SetAdapter(_adapterBarraFria);

            _adapterBarraFria.NotifyDataSetChanged();
            _adapterBarraFria.ItemClick += BarraFria_ButtonClick;

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerView.AddItemDecoration(itemDecor);

            _buttonNext.Click += ButtonSiguiente_Click;

        }

        protected override void OnResume()
        {
            base.OnResume();
            _labelBarraFria.Text = ViewModel.Instance.EtiquetaBarraFria;

            if (ViewModel.Instance.Ensalada == null) Finish();
            ViewModel.Instance.OnExtraAdded += Instance_OnExtraAdded;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel.Instance.OnExtraAdded -= Instance_OnExtraAdded;
        }

        #endregion
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return true;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != 1 || resultCode != Result.Ok) return;

            SetResult(Result.Ok);
            Finish();
        }

        public override void OnBackPressed()
        {
            ViewModel.Instance.ReiniciarPaso();
            base.OnBackPressed();
        }

        private void Instance_OnExtraAdded(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            //if (!_advertenciaExtra) return;
            //SendConfirmation(e.Message, "", "Ok", "Dejar de mostrar advertencia", save => _advertenciaExtra = false);

            SendToast(e.Message);
        }


        private void BarraFria_ButtonClick(object sender, RecyclerClickEventArgs e)
        {
            if (ViewModel.Instance.CantidadIngredientesBarra >= ViewModel.Instance.MaximoBarraFria)
            {
                SendMessage($"Solo puedes agregar {ViewModel.Instance.MaximoBarraFria} ingredientes de barra fría por ensalada");
                return;
            }

            var item = ViewModel.Instance.ListaBarraFria[e.Position];
            if (ViewModel.Instance.Ensalada.CantidadIngredientesBarra.ContainsKey(item.Id))
            {
                _labelBarraFria.Text = ViewModel.Instance.AgregarBarraFria(item);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesBarra[item.Id];
                if (!(_flexbox.FindViewWithTag(item.Id) is Chip oldChip)) return;
                _flexbox.RemoveView(oldChip);
                var chip = new Chip(this)
                {
                    ChipText = cantidad > 1 ? $"{cantidad}x {item.Descripcion}" : $"{item.Descripcion}",
                    Closable = true,
                    Tag = item.Id,
                    LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
                };
                chip.Close += Chip_Barra_Close;
                _flexbox.AddView(chip);
            }
            else
            {
                _labelBarraFria.Text = ViewModel.Instance.AgregarBarraFria(item);
                var chip = new Chip(this)
                {
                    ChipText = item.Descripcion,
                    Closable = true,
                    Tag = item.Id,
                    LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
                };
                chip.Close += Chip_Barra_Close;
                _flexbox.AddView(chip);
            }
        }

        private void Chip_Barra_Close(object sender, System.EventArgs e)
        {
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Barra_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesBarra.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaBarraFria.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesBarra[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverBarraFria(item);
                }
            }

            _labelBarraFria.Text = ViewModel.Instance.EtiquetaBarraFria;
            _flexbox.RemoveView(chip);
        }


        private void ButtonSiguiente_Click(object sender, System.EventArgs e)
        {
            if (ViewModel.Instance.HaCompleadoSegundoPaso)
            {
                StartActivityForResult(typeof(WrapPaso4Activity), 1);
            }
            else
            {
                SendMessage("Aun puedes seleccionar ingredientes de barra fría");
            }

        }
    }
}