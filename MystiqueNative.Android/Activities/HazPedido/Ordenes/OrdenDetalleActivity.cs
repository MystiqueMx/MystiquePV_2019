using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Ordenes
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label="Pedido")]
    public class OrdenDetalleActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_orden_detalle;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;
        #region EXPORTS

        #endregion

        #region VIEWS

        private TextView _labelTotal;
        private TextView _labelCargoServicio;

        #endregion

        #region FIELDS

        private OrdenDetalleAdapter _adapter;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
        }
        private void GrabViews()
        {
            _labelTotal = FindViewById<TextView>(Resource.Id.carrito_label_total);
            _labelCargoServicio = FindViewById<TextView>(Resource.Id.carrito_label_cargo_servicio);

            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            _adapter = new OrdenDetalleAdapter(this, ViewModels.PedidosViewModel.Instance.OrdenSeleccionada.Detalle);

            _labelTotal.Text = $"{PedidosViewModel.Instance.OrdenSeleccionada.Total:C}";
            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(_adapter);
            
            var itemDecor = new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            recyclerview.AddItemDecoration(itemDecor);
            _adapter.ItemClick += Adapter_Button1Click;
            _adapter.NotifyDataSetChanged();
        }

        protected override void OnResume()
        {
            base.OnResume();
            
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        #endregion
        
        private void Adapter_Button1Click(object sender, RecyclerClickEventArgs e)
        {
            var item = PedidosViewModel.Instance.OrdenSeleccionada.Detalle[e.Position];
            var intent = new Intent(this, typeof(OrdenDetallePlatilloActivity));
            intent.PutExtra(OrdenDetallePlatilloActivity.ExtraContent, item.Descripcion);
            intent.PutExtra(OrdenDetallePlatilloActivity.ExtraImagen, item.Imagen);
            intent.PutExtra(OrdenDetallePlatilloActivity.ExtraPrecio, $"{item.Precio:C}");
            intent.PutExtra(OrdenDetallePlatilloActivity.ExtraTitle, item.Nombre);
            StartActivity(intent);
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

    }
}