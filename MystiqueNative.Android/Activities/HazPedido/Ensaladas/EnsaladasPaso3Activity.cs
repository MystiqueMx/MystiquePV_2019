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
using Firebase.Analytics;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils.Flexbox;
using MystiqueNative.Droid.Utils.Views;
using MystiqueNative.Models.Carrito;
using ViewModel = MystiqueNative.ViewModels.EnsaladaPasoTresViewModel;

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/ensaladas_armar_title")]
    public class EnsaladasPaso3Activity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_ensaladas_paso3;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #region FIELDS

        private IngredienteEnsaladaAdapter _adapterCortesias;

        private EnsaladaCarrito _pedido;

        private bool _advertenciaExtra;
        #endregion

        #region VIEWS

        private FlexboxLayout _flexbox;
        private TextView _labelCortesias;
        private Button _buttonNext;
        private FirebaseAnalytics _firebaseAnalytics;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            ViewModel.Instance.ReiniciarPaso();


            GrabViews();
            _advertenciaExtra = true;
        }

        private void GrabViews()
        {
            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            var recyclerViewCortesias = FindViewById<RecyclerView>(Resource.Id.recycler_view2);

            _flexbox = FindViewById<FlexboxLayout>(Resource.Id.flexbox);
            _labelCortesias = FindViewById<TextView>(Resource.Id.ensaladas_categoria5_counter);
            _buttonNext = FindViewById<Button>(Resource.Id.button_next);


            _adapterCortesias = new IngredienteEnsaladaAdapter(this,
                ViewModel.Instance.ListaCortesias);

            recyclerViewCortesias.HasFixedSize = true;
            recyclerViewCortesias.SetAdapter(_adapterCortesias);
            recyclerViewCortesias.AddItemDecoration(itemDecor);
            _adapterCortesias.NotifyDataSetChanged();

            _adapterCortesias.ItemClick += Cortesias_ButtonClick;


            _buttonNext.Click += ButtonTerminar_Click;

        }


        protected override void OnResume()
        {
            base.OnResume();

            _labelCortesias.Text = ViewModel.Instance.EtiquetaCortesias;
            ViewModel.Instance.OnExtraAdded += Instance_OnExtraAdded;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel.Instance.OnExtraAdded -= Instance_OnExtraAdded;
        }

        #endregion

        #region ACTIVITY OVERRIDES

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
            ViewModel.Instance.ReiniciarPaso();
            base.OnBackPressed();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode != 1 || resultCode != Result.Ok) return;

            SetResult(Result.Ok);
            Finish();
        }

        #endregion

        private void Instance_OnExtraAdded(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            //if (!_advertenciaExtra) return;
            //SendConfirmation(e.Message, "", "Ok", "Dejar de mostrar advertencia", save => _advertenciaExtra = false);

            SendToast(e.Message);
        }


        private void Cortesias_ButtonClick(object sender, RecyclerClickEventArgs e)
        {
            if (_pedido != null) return;

            if (ViewModel.Instance.CantidadIngredientesCortesias >= ViewModel.Instance.MaximoCantidadCortesias)
            {
                SendMessage($"Solo puedes agregar {ViewModel.Instance.MaximoCantidadCortesias} cortesías por ensalada");
                return;
            }

            var item = ViewModel.Instance.ListaCortesias[e.Position];
            if (ViewModel.Instance.Ensalada.CantidadIngredientesCortesias.ContainsKey(item.Id)) return;
            var chip = new Chip(this)
            {
                ChipText = item.Descripcion,
                Closable = true,
                Tag = item.Id,
                LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
            };

            if (!ViewModel.Instance.Ensalada.CantidadIngredientesCortesias.ContainsKey(item.Id))
            {
                _labelCortesias.Text = ViewModel.Instance.AgregarCortesias(item);
            }
            chip.Close += Chip_Cortesia_Close;
            _flexbox.AddView(chip);
        }

        private void Chip_Cortesia_Close(object sender, System.EventArgs e)
        {
            if (_pedido != null) return;
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Cortesia_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesCortesias.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaCortesias.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesCortesias[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverCortesias(item);
                }
            }

            _labelCortesias.Text = ViewModel.Instance.EtiquetaCortesias;
            _flexbox.RemoveView(chip);
        }

        private void ButtonTerminar_Click(object sender, System.EventArgs e)
        {
            StartActivityForResult(typeof(EnsaladasPaso1Activity), 1);
        }
    }
}