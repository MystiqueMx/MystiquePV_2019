
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.HazPedido.Carrito;
using MystiqueNative.Droid.HazPedido.HazTuWrap;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Ensaladas
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, FinishOnTaskLaunch = true, Label = "@string/ensaladas_title")]
    public class EnsaladasActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_ensaladas;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;

        #region EXPORTS

        public const string ExtraEsDirectorio = "Qdc.EnsaladasActivity.ExtraEsDirectorio";
        public const string ExtraModoLectura = "Qdc.EnsaladasActivity.ExtraModoLectura";
        #endregion
        #region FIELDS

        private bool _isNewConfiguration;
        private bool _canContinue;

        #endregion

        #region VIEWS

        private FrameLayout _progressBarHolder;
        private TextView _labelPrecioPolloChica;
        private TextView _labelPrecioPolloMediana;
        private TextView _labelPrecioPolloGrande;
        private TextView _labelPrecioAtunChica;
        private TextView _labelPrecioAtunMediana;
        private TextView _labelPrecioAtunGrande;
        private TextView _labelPrecioCamaronChica;
        private TextView _labelPrecioCamaronMediana;
        private TextView _labelPrecioCamaronGrande;
        private TextView _labelPrecioWrapChico;
        private CardView _cardCamaronChica;
        private CardView _cardCamaronMediana;
        private CardView _cardCamaronGrande;
        private CardView _cardPolloMediana;
        private CardView _cardPolloChica;
        private CardView _cardPolloGrande;
        private CardView _cardAtunChica;
        private CardView _cardAtunMediana;
        private CardView _cardAtunGrande;
        private CardView _cardWrapChica;
        private FloatingActionButton _fab;
        private bool _isDirectorio;
        private bool _isReadOnly;
        private View _layoutTotal;
        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabIntentParameters();
            GrabViews();
            _cardAtunMediana.Visibility = ViewStates.Gone;
            _cardCamaronChica.Visibility = ViewStates.Gone;
            _cardCamaronMediana.Visibility = ViewStates.Gone;
            _cardCamaronGrande.Visibility = ViewStates.Gone;
            _cardPolloMediana.Visibility = ViewStates.Gone;

            StartAnimating();
            if (MystiqueNative.ViewModels.EnsaladasViewModel.Instance.IsLoaded)
            {
                SetPrices();
            }
            else
            {
                _isNewConfiguration = true;
            }
        }

        private void GrabIntentParameters()
        {
            _isDirectorio = Intent.GetBooleanExtra(ExtraEsDirectorio, false);
            _isReadOnly = Intent.GetBooleanExtra(ExtraModoLectura, false);
        }

        private void GrabViews()
        {
            _layoutTotal = FindViewById(Resource.Id.ensaladas_ly_total);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _labelPrecioPolloChica = FindViewById<TextView>(Resource.Id.ensaladas_precio_chica1);
            _labelPrecioPolloMediana = FindViewById<TextView>(Resource.Id.ensaladas_precio_mediana1);
            _labelPrecioPolloGrande = FindViewById<TextView>(Resource.Id.ensaladas_precio_grande1);

            _labelPrecioAtunChica = FindViewById<TextView>(Resource.Id.ensaladas_precio_chica2);
            _labelPrecioAtunMediana = FindViewById<TextView>(Resource.Id.ensaladas_precio_mediana2);
            _labelPrecioAtunGrande = FindViewById<TextView>(Resource.Id.ensaladas_precio_grande2);


            _labelPrecioCamaronChica = FindViewById<TextView>(Resource.Id.ensaladas_precio_chica3);
            _labelPrecioCamaronMediana = FindViewById<TextView>(Resource.Id.ensaladas_precio_mediana3);
            _labelPrecioCamaronGrande = FindViewById<TextView>(Resource.Id.ensaladas_precio_grande3);

            _labelPrecioWrapChico = FindViewById<TextView>(Resource.Id.wrap_precio_chica);


            _fab = FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.fab);

            _fab.Click += Fab_Click;

            #region CARDS CLICK

            _cardPolloChica = FindViewById<CardView>(Resource.Id.ensaladas_card_chica1);
            _cardPolloMediana = FindViewById<CardView>(Resource.Id.ensaladas_card_mediana1);
            _cardPolloGrande = FindViewById<CardView>(Resource.Id.ensaladas_card_grande1);

            _cardAtunChica = FindViewById<CardView>(Resource.Id.ensaladas_card_chica2);
            _cardAtunMediana = FindViewById<CardView>(Resource.Id.ensaladas_card_mediana2);
            _cardAtunGrande = FindViewById<CardView>(Resource.Id.ensaladas_card_grande2);

            _cardCamaronChica = FindViewById<CardView>(Resource.Id.ensaladas_card_chica3);
            _cardCamaronMediana = FindViewById<CardView>(Resource.Id.ensaladas_card_mediana3);
            _cardCamaronGrande = FindViewById<CardView>(Resource.Id.ensaladas_card_grande3);

            _cardWrapChica = FindViewById<CardView>(Resource.Id.wrap_card_chica);

            _cardAtunChica.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.MariscosChica);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardAtunMediana.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.MariscosMediana);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardAtunGrande.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.MariscosGrande);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };
            _cardCamaronChica.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.CamaronChica);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardCamaronMediana.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.CamaronMediana);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardCamaronGrande.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.CamaronGrande);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardPolloChica.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.PolloChica);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardPolloMediana.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.PolloMediana);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardPolloGrande.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.PolloGrande);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };

            _cardWrapChica.Click += delegate
            {
                if (_canContinue)
                {
                    SeleccionarTipo(PresentacionEnsalada.WrapChica);
                }
                else
                {
                    EnviarAlertaNoConfiguracion();
                }
            };
            #endregion
        }


        protected override async void OnResume()
        {
            base.OnResume();
            _layoutTotal.Visibility = _isDirectorio ? ViewStates.Gone : ViewStates.Visible;
            _fab.Visibility = CarritoViewModel.Instance.ExistenItemsEnCarrito && !_isDirectorio ? ViewStates.Visible : ViewStates.Gone;

            UpdateTotalCarrito();

            if (!_isNewConfiguration) return;
            MystiqueNative.ViewModels.EnsaladasViewModel.Instance.OnObtenerConfiguracionEnsaladasFinished += Instance_OnObtenerConfiguracionEnsaladasFinished;
            await MystiqueNative.ViewModels.EnsaladasViewModel.Instance.ObtenerConfiguracion();

        }

        protected override void OnPause()
        {
            base.OnPause();

            if (_isNewConfiguration)
            {
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.OnObtenerConfiguracionEnsaladasFinished -= Instance_OnObtenerConfiguracionEnsaladasFinished;
            }
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

        private void Instance_OnObtenerConfiguracionEnsaladasFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            if (e.Success)
            {
                SetPrices();
            }
            else
            {
                StopAnimating();
                SendConfirmation(e.Message, "", (ok) =>
                {
                    Finish();
                });
            }

        }

        private void SetPrices()
        {
            _labelPrecioAtunChica.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaMariscosChica;
            _labelPrecioAtunMediana.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaMariscosMediana;
            _labelPrecioAtunGrande.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaMariscosGrande;

            _labelPrecioCamaronChica.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaCamaronChica;
            _labelPrecioCamaronMediana.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaCamaronMediana;
            _labelPrecioCamaronGrande.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaCamaronGrande;

            _labelPrecioPolloChica.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaPolloChica;
            _labelPrecioPolloMediana.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaPolloMediana;
            _labelPrecioPolloGrande.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioEnsaladaPolloGrande;

            _labelPrecioWrapChico.Text =
                MystiqueNative.ViewModels.EnsaladasViewModel.Instance.Configuracion.Precios.PrecioWrapChica;

            _canContinue = true;
            StopAnimating();
        }

        private void SeleccionarTipo(PresentacionEnsalada presentacion)
        {

            if (_isReadOnly)
            {
                if (_isDirectorio) return;

                if (!RestaurantesViewModel.Instance.RestauranteActivo.EstaAbierto)
                {

                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el horario de atención es de {RestaurantesViewModel.Instance.RestauranteActivo.HoraApertura} a { RestaurantesViewModel.Instance.RestauranteActivo.HoraCierre}",
                        "Restaurante cerrado", "Continuar", "",
                        ok => { });
                }
                else
                {

                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el restaurante no se encuentra disponible en este momento",
                        "Restaurante no disponible", "Continuar", "",
                        ok =>
                        {
                        });
                }

            }
            else
            {
                if (CarritoViewModel.Instance.ExistenItemsEnCarrito &&
                    CarritoViewModel.Instance.PedidoActual.Restaurante.Id !=
                    RestaurantesViewModel.Instance.RestauranteActivo.Id)
                {
                    SendConfirmation($"Ya cuentas con un platillos seleccionados en {CarritoViewModel.Instance.PedidoActual.Restaurante.Descripcion}, ¿deseas desecharlo y ordenar en {RestaurantesViewModel.Instance.RestauranteActivo.Descripcion}?", "", "Continuar", "Salir",
                        continuar =>
                        {
                            if (continuar)
                            {
                                EnsaladasViewModel.Instance.SeleccionarPresentacion(presentacion);
                                if (presentacion == PresentacionEnsalada.WrapChica)
                                {
                                    StartActivity(typeof(WrapPaso3Activity));
                                }
                                else
                                {
                                    StartActivity(typeof(EnsaladasPaso3Activity));
                                }
                            }
                            else
                            {
                                Finish();
                            }
                        });
                }
                else
                {
                    EnsaladasViewModel.Instance.SeleccionarPresentacion(presentacion);
                    if (presentacion == PresentacionEnsalada.WrapChica)
                    {
                        StartActivity(typeof(WrapPaso3Activity));
                    }
                    else
                    {
                        StartActivity(typeof(EnsaladasPaso3Activity));
                    }
                }
            }
        }


        private void EnviarAlertaNoConfiguracion()
        {
            SendMessage(GetString(Resource.String.ensaladas_error_configuracion));
        }

        private void Fab_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CarritoActivity));
        }
        #region SPINNER


        private void StartAnimating()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimating()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }

        #endregion

    }
}