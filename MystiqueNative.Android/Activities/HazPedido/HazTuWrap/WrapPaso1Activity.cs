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
using ViewModel = MystiqueNative.ViewModels.EnsaladaPasoUnoViewModel;

namespace MystiqueNative.Droid.HazPedido.HazTuWrap
{
    //NOTE: espejo de los pasos de ensalada, se reciclan clases, modelos y palabras como ENSALADA
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/wrap_armar_title")]
    public class WrapPaso1Activity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_wrap_paso1;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;

        #region FIELDS

        private IngredienteEnsaladaAdapter _adapterProteina;

        private bool _advertenciaExtra;

        #region VIEWS

        private FlexboxLayout _flexbox;
        private TextView _labelProteina;
        private Button _buttonNext;
        #endregion

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            _advertenciaExtra = true;
            ViewModel.Instance.ReiniciarPaso();
        }

        private void GrabViews()
        {
            var recyclerViewProteina = FindViewById<RecyclerView>(Resource.Id.recycler_view1);

            _flexbox = FindViewById<FlexboxLayout>(Resource.Id.flexbox);
            _labelProteina = FindViewById<TextView>(Resource.Id.ensaladas_categoria1_counter);
            _buttonNext = FindViewById<Button>(Resource.Id.button_next);

            recyclerViewProteina.HasFixedSize = true;

            _adapterProteina = new IngredienteEnsaladaAdapter(this,
                ViewModel.Instance.ListaProteinas);

            recyclerViewProteina.SetAdapter(_adapterProteina);

            _adapterProteina.NotifyDataSetChanged();
            _adapterProteina.ItemClick += Proteinas_ButtonClick;

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerViewProteina.AddItemDecoration(itemDecor);

            _buttonNext.Click += _buttonSiguiente_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _labelProteina.Text = ViewModel.Instance.EtiquetaProteina;
            if (ViewModel.Instance.Ensalada == null) Finish();
            ViewModel.Instance.OnExtraAdded += Instance_OnExtraAdded;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel.Instance.OnExtraAdded -= Instance_OnExtraAdded;
        }

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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            ViewModel.Instance.ReiniciarPaso();
            base.OnBackPressed();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != 1 || resultCode != Result.Ok) return;

            SetResult(Result.Ok);
            Finish();
        }
        private void Proteinas_ButtonClick(object sender, RecyclerClickEventArgs e)
        {
            var item = ViewModel.Instance.ListaProteinas[e.Position];

            if (ViewModel.Instance.CantidadIngredientesProteina >= ViewModel.Instance.MaximoProteinas)
            {
                SendMessage($"Solo puedes agregar {ViewModel.Instance.MaximoProteinas} proteinas por ensalada");
                return;
            }

            if (ViewModel.Instance.Ensalada.CantidadIngredientesProteina.ContainsKey(item.Id))
            {
                _labelProteina.Text = ViewModel.Instance.AgregarProteina(item);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesProteina[item.Id];
                if (!(_flexbox.FindViewWithTag(item.Id) is Chip oldChip)) return;
                _flexbox.RemoveView(oldChip);
                var chip = new Chip(this)
                {
                    ChipText = cantidad > 1 ? $"{cantidad}x {item.Descripcion}" : $"{item.Descripcion}",
                    Closable = true,
                    Tag = item.Id,
                    LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
                };
                chip.Close += Chip_Proteina_Close;
                _flexbox.AddView(chip);
            }
            else
            {
                _labelProteina.Text = ViewModel.Instance.AgregarProteina(item);
                var chip = new Chip(this)
                {
                    ChipText = item.Descripcion,
                    Closable = true,
                    Tag = item.Id,
                    LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
                };
                chip.Close += Chip_Proteina_Close;
                _flexbox.AddView(chip);
            }

        }


        private void Chip_Proteina_Close(object sender, System.EventArgs e)
        {
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Extra_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesProteina.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaProteinas.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesProteina[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverProteina(item);
                }
            }
            _labelProteina.Text = ViewModel.Instance.EtiquetaProteina;
            _flexbox.RemoveView(chip);
        }

        private void Chip_Extra_Close(object sender, System.EventArgs e)
        {
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Extra_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesExtra.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaExtras.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesExtra[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverExtras(item);
                }
            }

            _flexbox.RemoveView(chip);
        }


        private void Instance_OnExtraAdded(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            //if (!_advertenciaExtra) return;
            //SendConfirmation(e.Message, "", "Ok", "Dejar de mostrar advertencia", save => _advertenciaExtra = false);

            SendToast(e.Message);
        }

        private void _buttonSiguiente_Click(object sender, System.EventArgs e)
        {
            if (ViewModel.Instance.HaCompleadoPrimerPaso)
            {
                StartActivityForResult(typeof(WrapPaso2Activity), 1);
            }
            else
            {
                SendMessage("Aun puedes seleccionar proteinas");
            }

        }
    }
}