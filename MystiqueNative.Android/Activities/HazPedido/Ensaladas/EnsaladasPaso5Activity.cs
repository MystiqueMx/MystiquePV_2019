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
using Com.Google.Android.Flexbox;
using Firebase.Analytics;
using MystiqueNative.Droid.HazPedido.Carrito;
using MystiqueNative.Droid.Utils.Flexbox;
using MystiqueNative.Droid.Utils.Views;
using Humanizer;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.ViewModels;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.HazPedido.Ensaladas;
using BarronWellnessMovil.Droid.Helpers;
using ViewModel = MystiqueNative.ViewModels.EnsaladaPasoTresViewModel;
using ViewModel2 = MystiqueNative.ViewModels.EnsaladaPasoCuatroViewModel;

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/ensaladas_armar_title")]
    public class EnsaladasPaso5Activity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_ensaladas_paso5;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;

        #region FIELDS

        private IngredienteEnsaladaAdapter _adapterComplementos;

        private EnsaladaCarrito _pedido;

        private bool _advertenciaExtra;
        #endregion

        #region VIEWS

        private FlexboxLayout _flexbox;
        private TextView _labelComplementos;
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
            var recyclerViewComplementos = FindViewById<RecyclerView>(Resource.Id.recycler_view1);

            _flexbox = FindViewById<FlexboxLayout>(Resource.Id.flexbox);
            _labelComplementos = FindViewById<TextView>(Resource.Id.ensaladas_categoria4_counter);
            _buttonNext = FindViewById<Button>(Resource.Id.button_next);


            _adapterComplementos = new IngredienteEnsaladaAdapter(this,
                ViewModel.Instance.ListaComplementos);

            recyclerViewComplementos.HasFixedSize = true;
            recyclerViewComplementos.SetAdapter(_adapterComplementos);
            recyclerViewComplementos.AddItemDecoration(itemDecor);
            _adapterComplementos.NotifyDataSetChanged();

            _adapterComplementos.ItemClick += Complementos_ButtonClick;


            _buttonNext.Click += ButtonTerminar_Click;

        }


        protected override void OnResume()
        {
            base.OnResume();

            _labelComplementos.Text = ViewModel.Instance.EtiquetaComplementos;
            ViewModel.Instance.OnExtraAdded += Instance_OnExtraAdded;
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel.Instance.OnExtraAdded -= Instance_OnExtraAdded;
        }

        #endregion

        private void Instance_OnExtraAdded(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            //if (!_advertenciaExtra) return;
            //SendConfirmation(e.Message, "", "Ok", "Dejar de mostrar advertencia", save => _advertenciaExtra = false);

            SendToast(e.Message);
        }

        private void Complementos_ButtonClick(object sender, RecyclerClickEventArgs e)
        {
            if (_pedido != null) return;
            if (ViewModel.Instance.CantidadIngredientesComplementos >= ViewModel.Instance.MaximoComplementos)
            {
                SendMessage($"Solo puedes agregar {ViewModel.Instance.MaximoComplementos} complementos por ensalada");
                return;
            }
            var item = ViewModel.Instance.ListaComplementos[e.Position];
            var chip = new Chip(this)
            {
                ChipText = item.Descripcion,
                Closable = true,
                Tag = item.Id,
                LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
            };
            if (ViewModel.Instance.PrimerGrupoComplementos.Contains(item.Id) &&
                ViewModel.Instance.Ensalada.CantidadIngredientesComplementos.Keys.Any(c => ViewModel.Instance.PrimerGrupoComplementos.Contains(c)))
            {
                return;
            }
            if (ViewModel.Instance.SegundoGrupoComplementos.Contains(item.Id) &&
                     ViewModel.Instance.Ensalada.CantidadIngredientesComplementos.Keys.Any(c => ViewModel.Instance.SegundoGrupoComplementos.Contains(c)))
            {
                return;
            }

            if (ViewModel.Instance.Ensalada.CantidadIngredientesComplementos.ContainsKey(item.Id))
            {
                return;
                _labelComplementos.Text = ViewModel.Instance.AgregarComplementos(item);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesComplementos[item.Id];
                if (!(_flexbox.FindViewWithTag(item.Id) is Chip oldChip)) return;
                _flexbox.RemoveView(oldChip);
                if (cantidad > 1)
                {
                    chip.ChipText = $"{cantidad}x {item.Descripcion}";
                }
            }
            else
            {
                _labelComplementos.Text = ViewModel.Instance.AgregarComplementos(item);
            }
            chip.Close += Chip_Complementos_Close;
            _flexbox.AddView(chip);
        }

        private void Chip_Complementos_Close(object sender, System.EventArgs e)
        {
            if (_pedido != null) return;
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Complementos_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesComplementos.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaComplementos.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesComplementos[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverComplementos(item);
                }
            }

            _labelComplementos.Text = ViewModel.Instance.EtiquetaComplementos;
            _flexbox.RemoveView(chip);
        }

        private void ButtonTerminar_Click(object sender, System.EventArgs e)
        {
            var dialog = new ComplementoEnsaladaDialogFragment();
            dialog.DialogClosed += Complementos_DialogClosed1;
            dialog.Show(SupportFragmentManager, nameof(ComplementoEnsaladaDialogFragment));
        }

        private void Complementos_DialogClosed1(object sender, ComplementosDialogEventArgs e)
        {
            if (e == null || e.Seleccion == ComplementosEnsalada.NoDefinida) return;
            ViewModel.Instance.SeleccionarComplemento(e.Seleccion);

            TerminarEnsalada();
        }

        private void TerminarEnsalada()
        {
            var nombre = ViewModel.Instance.Ensalada.Presentacion.Humanize();
            if (ViewModel2.Instance.TerminarEnsalada())
            {
                if (MainApplication.LogAnalytics)
                {
                    var bundle = new Bundle();
                    bundle.PutString(FirebaseAnalytics.Param.ItemName, nombre);
                    bundle.PutString(FirebaseAnalytics.Param.ItemCategory, $"{CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre}");
                    _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.AddToCart, bundle);
                }
                SendConfirmation("Tu ensalada ha sido agregada al carrito con éxito", "", "Ir al carrito", "Seguir explorando", irAlCarrito =>
                {
                    if (irAlCarrito)
                    {
                        var i = new Intent(this, typeof(CarritoActivity));
                        StartActivity(i);
                        SetResult(Result.Ok);
                        Finish();
                    }
                    else
                    {
                        var i = new Intent(this, typeof(MenuRestauranteHazPedidoActivity));
                        i.PutExtra(MenuRestauranteHazPedidoActivity.ExtraReentry, true);
                        i.AddFlags(ActivityFlags.ClearTop);
                        StartActivity(i);
                        SetResult(Result.Ok);
                        Finish();
                    }
                });
            }
            else
            {
                SendMessage("Ocurrió un error al agregar la ensalada al carrito");
            }
        }
    }
}