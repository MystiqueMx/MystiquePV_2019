using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using Firebase.Analytics;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.EventsArgs;
using MystiqueNative.Models.Platillos;
using MystiqueNative.ViewModels;
using Newtonsoft.Json;

namespace MystiqueNative.Droid.HazPedido.Platillos
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Selección del platillo")]
    public class SubPlatillosActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_subplatillos;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;

        #region EXPORTS

        public const string ExtraNivelMenu = "QDC_ExtraNivelMenu";
        public const string ExtraContenidosMenu = "QDC_ExtraContenidosMenu";
        public const string ExtraTitleMenu = "QDC_EXTRATITLE2MENU";
        public const string ExtraSeleccionMenu = "QDC_ExtraSeleccionMenu";
        #endregion

        #region FIELDS
        private readonly Dictionary<int, SeleccionPlatilloMultiNivel> _submenuSeleccionados =
            new Dictionary<int, SeleccionPlatilloMultiNivel>();
        private ObservableCollection<BasePlatilloMultiNivel> _subPlatillos;
        private SubPlatillosAdapter _adapter;
        private string _title;
        private NivelesPlatillo _nivel;
        private int _resultRequestId;
        private int _idSeleccionado;
        private SeleccionPlatilloMultiNivel _seleccionPrevia;
        private FirebaseAnalytics _firebaseAnalytics;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            GrabIntentParameters();
            GrabViews();
        }

        private void GrabIntentParameters()
        {
            var nivel = Intent.GetIntExtra(ExtraNivelMenu, 0);
            var items = Intent.GetStringExtra(ExtraContenidosMenu);
            var seleccionPreviaSerializada = Intent.GetStringExtra(ExtraSeleccionMenu);
            if (nivel == 0) throw new ArgumentException(nameof(ExtraNivelMenu));
            _title = Intent.GetStringExtra(ExtraTitleMenu);
            _nivel = (NivelesPlatillo)nivel;
            InitListaSubplatillos(items, seleccionPreviaSerializada);
        }

        private void GrabViews()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new SubPlatillosAdapter(this, _subPlatillos);
            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(_adapter);

            _adapter.ItemClick += Adapter_Button1Click;
            _adapter.NotifyDataSetChanged();

            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerView.AddItemDecoration(itemDecor);

            Title = ViewModels.RestaurantesViewModel.Instance.RestauranteActivo.Nombre;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Title = _title;
            ViewModels.PlatillosViewModel.Instance.PlatilloTerminado += PlatilloTerminado;
            ViewModels.PlatillosViewModel.Instance.CargarSubmenu += Instance_CargarSubmenu;
            ViewModels.PlatillosViewModel.Instance.SubplatilloCompletado += SubplatilloParcialmenteTerminado;
            UpdateTotalCarrito();
        }


        protected override void OnPause()
        {
            base.OnPause();
            ViewModels.PlatillosViewModel.Instance.PlatilloTerminado -= PlatilloTerminado;
            ViewModels.PlatillosViewModel.Instance.CargarSubmenu -= Instance_CargarSubmenu;
            ViewModels.PlatillosViewModel.Instance.SubplatilloCompletado -= SubplatilloParcialmenteTerminado;
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

        public override void OnBackPressed()
        {
            if (_nivel == NivelesPlatillo.PrimerNivel)
            {
                SendConfirmation("Este platillo no ha sido completado, ¿deseas desecharlo?", "Salir", "Desechar", "Volver",
                    ok =>
                    {
                        if (ok) base.OnBackPressed();
                    });
            }
            else
            {
                base.OnBackPressed();
            }

        }

        private void Adapter_Button1Click(object sender, RecyclerClickEventArgs e)
        {
            if (this.IsFinishing) return;
            var item = _subPlatillos[e.Position];
            _idSeleccionado = item.Id;
            ViewModels.PlatillosViewModel.Instance.SeleccionarSubPlatillo(_idSeleccionado, _nivel);
            UpdateTotalCarrito();
        }

        private void Instance_CargarSubmenu(object sender, CargarSubmenuEventArgs e)
        {
            UpdateTotalCarrito();
            _resultRequestId = new Random().Next(0, 2000);

            var intent = new Intent(this, typeof(SubPlatillosActivity));
            intent.PutExtra(ExtraTitleMenu, "");
            intent.PutExtra(ExtraContenidosMenu, JsonConvert.SerializeObject(e.Contenido));
            intent.PutExtra(ExtraNivelMenu, (int)e.Nivel);

            // Reingreso dentro de subplatillos de combos
            if (_nivel == NivelesPlatillo.PrimerNivel && _submenuSeleccionados.ContainsKey(_idSeleccionado))
            {
                intent.PutExtra(ExtraSeleccionMenu, JsonConvert.SerializeObject(_submenuSeleccionados[_idSeleccionado]));
            }
            if (_seleccionPrevia != null)
            {
                intent.PutExtra(ExtraSeleccionMenu, JsonConvert.SerializeObject(_seleccionPrevia));
            }
            StartActivityForResult(intent, _resultRequestId);
        }

        private void PlatilloTerminado(object sender, PlatilloTerminadoEventArgs e)
        {
            //if (MainApplication.LogAnalytics)
            //{
            //    var bundle = new Bundle();
            //    bundle.PutString(FirebaseAnalytics.Param.ItemName, e.Platillo.Nombre);
            //    bundle.PutString(FirebaseAnalytics.Param.ItemCategory, $"{CarritoViewModel.Instance.PedidoActual.Restaurante.Nombre}");
            //    _firebaseAnalytics.LogEvent(FirebaseAnalytics.Event.AddToCart, bundle);
            //}
            UpdateTotalCarrito();
            SetResult(Result.Ok);
            Finish();
        }

        private void SubplatilloParcialmenteTerminado(object sender, SubplatilloCompletado e)
        {
            UpdateTotalCarrito();
            if (_nivel != NivelesPlatillo.PrimerNivel)
            {
                var i = new Intent();
                i.PutExtra(ExtraSeleccionMenu, JsonConvert.SerializeObject(e.Seleccion));
                SetResult(Result.FirstUser, i);
                Finish();
            }
            else
            {
                SendToast(e.Message);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            UpdateTotalCarrito();
            switch (resultCode)
            {
                case Result.Canceled:
                    break;
                case Result.FirstUser:
                    var seleccion = data.GetStringExtra(ExtraSeleccionMenu);
                    if (_nivel == NivelesPlatillo.PrimerNivel)
                    {
                        var resultado = JsonConvert.DeserializeObject<SeleccionPlatilloMultiNivel>(seleccion);
                        _submenuSeleccionados[resultado.IdNivel1.Key] = resultado;
                        var item = _subPlatillos.First(c => c.Id == resultado.IdNivel1.Key);
                        item.Completado = true;
                        _adapter.NotifyItemChanged(_subPlatillos.IndexOf(item));
                    }
                    else
                    {
                        var i = new Intent();
                        i.PutExtra(ExtraSeleccionMenu, seleccion);
                        SetResult(Result.FirstUser, i);
                        Finish();
                    }

                    break;
                case Result.Ok:
                    SetResult(Result.Ok);
                    Finish();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultCode), resultCode, null);
            }

        }
        private void InitListaSubplatillos(string items, string seleccionPreviaSerializada)
        {
            var listaSubplatillos = JsonConvert.DeserializeObject<List<BasePlatilloMultiNivel>>(items);
            if (!string.IsNullOrEmpty(seleccionPreviaSerializada))
            {
                _seleccionPrevia = JsonConvert.DeserializeObject<SeleccionPlatilloMultiNivel>(seleccionPreviaSerializada);
                switch (_nivel)
                {
                    case NivelesPlatillo.SegundoNivel:
                        listaSubplatillos.First(c => c.Id == _seleccionPrevia.IdNivel2.Key).Completado = true;
                        break;
                    case NivelesPlatillo.TercerNivel:
                        listaSubplatillos.First(c => c.Id == _seleccionPrevia.IdNivel3.Key).Completado = true;
                        break;
                    case NivelesPlatillo.NoDefinido:
                    case NivelesPlatillo.PrimerNivel:
                    default:
                        Console.WriteLine(_nivel);
                        break;
                }
            }

            if (_nivel == NivelesPlatillo.PrimerNivel)
            {
                if (listaSubplatillos.Any(c => c.Completado))
                {
                    var subplatillosCompletados = listaSubplatillos.Where(c => c.Completado).ToList();
                    foreach (var basePlatilloMultiNivel in subplatillosCompletados)
                    {
                        if (ViewModels.PlatillosViewModel.Instance.EsTerminal(basePlatilloMultiNivel.Id, _nivel))
                        {
                            _submenuSeleccionados.Add(basePlatilloMultiNivel.Id, new SeleccionPlatilloMultiNivel
                            {
                                IdNivel1 = new KeyValuePair<int, string>(basePlatilloMultiNivel.Id, basePlatilloMultiNivel.Descripcion),
                            });
                        }
                    }
                }
            }
            _subPlatillos = new ObservableCollection<BasePlatilloMultiNivel>(listaSubplatillos);
        }
    }
}