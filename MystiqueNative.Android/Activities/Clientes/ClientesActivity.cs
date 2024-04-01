using System;
using System.Collections.Generic;
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
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask)]
    public class ClientesActivity : BaseActivity
    {
        #region SINGLETON
        protected override int LayoutResource => Resource.Layout.activity_clientes;
        //protected override int BackButtonIcon => Resource.Drawable.ic_menu_white_24dp;
        protected override int BackButtonIcon => Resource.Drawable.ic_chevron_left_white_24dp;
        #endregion

        #region EXPORT

        #endregion

        #region FIELDS
        private FrameLayout progressBarHolder;
        private RecyclerView _recyclerView;
        private ClientesAdapter _adapter;

        private int _filtroSelecionado;
        #endregion

        #region LIFECYCLE
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _filtroSelecionado = (int)Models.Menu.FiltroRestaurante.Todos;

            GrabViews();

            var clientDialog = new ClienteBuscarDialogFragment();
            clientDialog.DialogClientSearchClosed += clientDialog_DialogClosed;
            clientDialog.Show(SupportFragmentManager, "dateSelectDialogFragment");
        }

        protected override void OnPause()
        {
            base.OnPause();
            ClientesViewModel.Instance.OnFinishObtenerClientes -= Instance_OnFinishObtenerClientes;
            RestaurantesViewModel.Instance.OnObtenerRestaurantesFinished -= Instance_OnObtenerRestaurantesFinished;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ClientesViewModel.Instance.OnFinishObtenerClientes += Instance_OnFinishObtenerClientes;
            RestaurantesViewModel.Instance.OnObtenerRestaurantesFinished += Instance_OnObtenerRestaurantesFinished;
            _adapter.NotifyDataSetChanged();
        }
        #endregion

        #region OVERRIDES
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            #region OnCreateOptionsMenu
            MenuInflater.Inflate(Resource.Menu.item_single_search, menu);
            return base.OnCreateOptionsMenu(menu); 
            #endregion
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            #region OnOptionsItemSelected
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
                case Resource.Id.item_search_modal:
                    var clientDialog = new ClienteBuscarDialogFragment();
                    clientDialog.DialogClientSearchClosed += clientDialog_DialogClosed;
                    clientDialog.Show(SupportFragmentManager, "dateSelectDialogFragment");
                    break;
                default: return base.OnOptionsItemSelected(item);
            }
            return true; 
            #endregion
        }
        #endregion

        #region EVENTS
        private void Adapter_CardClicked(object sender, RecyclerClickEventArgs e)
        {
            #region Adapter_CardClicked
            ClientesViewModel.Instance.ClienteSelected = ClientesViewModel.Instance.ClienteCallCenter[e.Position];

            var item = RestaurantesViewModel.Instance.Restaurantes[0];
            RestaurantesViewModel.Instance.SeleccionarRestaurante(item);
            var intent = new Intent(this, typeof(HazPedido.MenuRestauranteHazPedidoActivity));
            intent.PutExtra(HazPedido.MenuRestauranteHazPedidoActivity.ExtraIdRestaurante, item.Id);
            intent.AddFlags(ActivityFlags.NewTask);

            if (item.EstaAbierto && item.EstaOperando)
            {
                StartActivity(intent);
            }
            else
            {
                if (!item.EstaAbierto)
                {
                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el horario de atención es de {item.HoraApertura} a {item.HoraCierre}",
                        "Restaurante cerrado", "Continuar", "",
                        ok =>
                        {
                            intent.PutExtra(HazPedido.MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                            StartActivity(intent);
                        });
                }
                else
                {
                    SendConfirmation(
                        $"Puedes continuar explorando el menú, el restaurante no se encuentra disponible en este momento",
                        "Restaurante no disponible", "Continuar", "",
                        ok =>
                        {
                            intent.PutExtra(HazPedido.MenuRestauranteHazPedidoActivity.ExtraModoLectura, true);
                            StartActivity(intent);
                        });
                }
            }

            //StartActivity(typeof(HazPedido.HomeHazPedidoActivity));
            #endregion
        }
        #endregion

        #region METHODS
        private void GrabViews()
        {
            #region GrabViews
            progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new ClientesAdapter(this, ClientesViewModel.Instance.ClienteCallCenter);

            _recyclerView.HasFixedSize = true;
            _recyclerView.SetAdapter(_adapter);

            _adapter.ItemClick += Adapter_CardClicked;
            #endregion
        }
        #endregion

        #region CALLBACKS

        private void clientDialog_DialogClosed(object sender, ClientSearchEventArgs args)
        {
            #region clientDialog_DialogClosed
            if (args == null) return;
            StartLoading();
            ClientesViewModel.Instance.BuscarListaClienteCallCenter(args.Telefono, args.NombreCliente);
            #endregion
        }
               
        private void Instance_OnFinishObtenerClientes(object sender, BaseListEventArgs e)
        {
            #region Instance_OnFinishBuscarCliente
            
            if (e.Success)
            {
                MystiqueApp.UltimaUbicacionConocida = new Models.Location.Point
                {
                    Longitude = -115.449090655893,
                    Latitude = 32.6260518104036
                };
                RestaurantesViewModel.Instance.ObtenerRestaurantes((Models.Menu.FiltroRestaurante)_filtroSelecionado);
            }
            else
            {
                StopLoading();
                SendMessage(e.Message);
            }
            #endregion
        }

        private void Instance_OnObtenerRestaurantesFinished(object sender, ObtenerRestaurantesEventArgs e)
        {
            #region Instance_OnObtenerRestaurantesFinished
            StopLoading();
            if (e.Success && RestaurantesViewModel.Instance.Restaurantes.Count() > 0)
            {
                
            }
            else
            {

            }
            #endregion
        }
        #endregion

        #region LOADING

        private void StartLoading()
        {
            #region StartAnimating
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = inAnimation;
            progressBarHolder.Visibility = ViewStates.Visible;
            #endregion
        }

        private void StopLoading()
        {
            #region StopAnimating
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            progressBarHolder.Animation = outAnimation;
            progressBarHolder.Visibility = ViewStates.Gone;
            #endregion
        }

        #endregion
    }
}