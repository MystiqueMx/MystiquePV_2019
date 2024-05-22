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
using Humanizer;
using MystiqueNative.Droid.HazPedido.Carrito;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils.Flexbox;
using MystiqueNative.Droid.Utils.Views;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.ViewModels;
using ViewModel = MystiqueNative.ViewModels.EnsaladaPasoCuatroViewModel;

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/ensaladas_armar_title")]

    public class EnsaladasPaso4Activity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_ensaladas_paso4;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #region FIELDS

        private IngredienteEnsaladaAdapter _adapterAderezo;

        private EnsaladaCarrito _pedido;

        private bool _advertenciaExtra;
        #endregion

        #region VIEWS

        private FlexboxLayout _flexbox;
        private TextView _labelAderezo;
        private Button _buttonNext;
        private FirebaseAnalytics _firebaseAnalytics;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            GrabViews();
            ViewModel.Instance.ReiniciarPaso();


            _advertenciaExtra = true;
        }

        private void GrabViews()
        {
            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            var recyclerViewAderezo = FindViewById<RecyclerView>(Resource.Id.recycler_view1);

            _flexbox = FindViewById<FlexboxLayout>(Resource.Id.flexbox);
            _labelAderezo = FindViewById<TextView>(Resource.Id.ensaladas_categoria5_counter);
            _buttonNext = FindViewById<Button>(Resource.Id.button_next);


            _adapterAderezo = new IngredienteEnsaladaAdapter(this, ViewModel.Instance.ListaAderezos);
            recyclerViewAderezo.HasFixedSize = true;
            recyclerViewAderezo.SetAdapter(_adapterAderezo);
            recyclerViewAderezo.AddItemDecoration(itemDecor);
            _adapterAderezo.NotifyDataSetChanged();
            _adapterAderezo.ItemClick += Aderezo_ButtonClick;

            _buttonNext.Click += ButtonTerminar_Click;

        }


        protected override void OnResume()
        {
            base.OnResume();

            _labelAderezo.Text = ViewModel.Instance.EtiquetaAderezos;
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
            if (_pedido == null)
            {
                base.OnBackPressed();
            }
            else
            {
                SendConfirmation("Al salir perderas el progreso de tu ensalada, ¿estás seguro que deseas continuar?", "", "Continuar", "Cancelar",
                    (cancelarEnsalada) =>
                    {
                        if (!cancelarEnsalada) return;
                        var i = new Intent(this, typeof(MenuRestauranteHazPedidoActivity));
                        i.PutExtra(MenuRestauranteHazPedidoActivity.ExtraReentry, true);
                        i.AddFlags(ActivityFlags.ClearTop);
                        StartActivity(i);
                    });
            }
        }

        #endregion

        private void Instance_OnExtraAdded(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {

            //if (!_advertenciaExtra) return;
            //SendConfirmation(e.Message, "", "Ok", "Dejar de mostrar advertencia", save => _advertenciaExtra = false);

            SendToast(e.Message);
        }

        private void Aderezo_ButtonClick(object sender, RecyclerClickEventArgs e)
        {
            if (_pedido != null) return;

            if (ViewModel.Instance.CantidadIngredientesAderezos >= ViewModel.Instance.MaximoAderezos)
            {
                SendMessage($"Solo puedes agregar {ViewModel.Instance.MaximoAderezos} aderezos por ensalada");
                return;
            }

            var item = ViewModel.Instance.ListaAderezos[e.Position];
            var chip = new Chip(this)
            {
                ChipText = item.Descripcion,
                Closable = true,
                Tag = item.Id,
                LayoutParameters = Resources.GetLayoutParams(2, 2, 2, 2)
            };


            if (EnsaladasViewModel.Instance.Ensalada.CantidadIngredientesAderezos.ContainsKey(item.Id))
            {
                _labelAderezo.Text = ViewModel.Instance.AgregarAderezo(item);

                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesAderezos[item.Id];
                if (!(_flexbox.FindViewWithTag(item.Id) is Chip oldChip)) return;
                _flexbox.RemoveView(oldChip);
                if (cantidad > 1)
                {
                    chip.ChipText = $"{cantidad}x {item.Descripcion}";
                }
            }
            else
            {
                _labelAderezo.Text = ViewModel.Instance.AgregarAderezo(item);
            }

            chip.Close += Chip_Aderezo_Close;
            _flexbox.AddView(chip);
        }

        private void Chip_Aderezo_Close(object sender, System.EventArgs e)
        {
            if (_pedido != null) return;
            if (!((sender as ImageView)?.Parent is Chip chip)) return;
            var itemId = (int)chip.Tag;
            chip.Close -= Chip_Aderezo_Close;
            if (ViewModel.Instance.Ensalada.CantidadIngredientesAderezos.ContainsKey(itemId))
            {
                var item = ViewModel.Instance.ListaAderezos.First(c => c.Id == itemId);
                var cantidad = ViewModel.Instance.Ensalada.CantidadIngredientesAderezos[itemId];
                for (var i = cantidad - 1; i >= 0; i--)
                {
                    ViewModel.Instance.RemoverAderezo(item);
                }
            }

            _labelAderezo.Text = ViewModel.Instance.EtiquetaAderezos;
            _flexbox.RemoveView(chip);
        }

        private void ButtonTerminar_Click(object sender, System.EventArgs e)
        {
            if (ViewModel.Instance.HaCompletadoCuartoPaso)
            {
                if (_pedido == null)
                {
                    var dialog = new AderezoEnsaladaDialogFragment();
                    dialog.DialogClosed += Dialog_DialogClosed;
                    dialog.Show(SupportFragmentManager, nameof(AderezoEnsaladaDialogFragment));
                }
                else
                {
                    MystiqueNative.ViewModels.CarritoViewModel.Instance.AgregarEnsaladaACarrito(_pedido);
                }
            }
            else
            {
                SendMessage("Aun no has seleccionado aderezo");
            }

        }


        private void Dialog_DialogClosed(object sender, AderezoDialogEventArgs e)
        {
            if (e == null || e.TipoAderezo == AderezoEnsalada.NoDefinida) return;
            ViewModel.Instance.SeleccionarAderezo(e.TipoAderezo);
            StartActivityForResult(typeof(EnsaladasPaso5Activity), 1);
            //TerminarEnsalada();
        }

        private void TerminarEnsalada()
        {
            var nombre = ViewModel.Instance.Ensalada.Presentacion.Humanize();
            if (ViewModel.Instance.TerminarEnsalada())
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