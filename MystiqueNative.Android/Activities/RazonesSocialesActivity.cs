using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using BarronWellnessMovil.Droid.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Runtime;
using MystiqueNative.Models;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Facturar consumo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RazonesSocialesActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_razones_sociales;
        #region VIEWS
        private BottomSheetBehavior _mBottomSheetBehavior;
        private Button _buttonEditar;
        private Button _buttonEliminar;
        private Button _buttonCancelar;
        private FrameLayout _progressBarHolder;
        #endregion
        #region FIELDS

        private int _editingPosition;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GrabViews();
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            var recyclerview = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var adapter = new RazonSocialAdapter(this, FacturacionViewModel.Instance.ReceptoresGuardados);
            FacturacionViewModel.Instance.AgregarCfdiVacio();

            recyclerview.HasFixedSize = true;
            recyclerview.SetAdapter(adapter);

            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_ItemLongClick;
            adapter.ItemClick2 += Adapter_ItemClick2;
            adapter.NotifyDataSetChanged();
        }

        private void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            _editingPosition = e.Position;
            _mBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
        }

        private void Adapter_ItemClick2(object sender, RecyclerClickEventArgs e)
        {
            StartActivityForResult(typeof(OtraRazonSocialActivity), OtraRazonSocialActivity.RequestNuevaRazonSocial);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode, Intent data)
        {
            var intent = new Intent(this, typeof(FacturaConfirmacionActivity));
            switch (requestCode)
            {
                case OtraRazonSocialActivity.RequestNuevaRazonSocial when resultCode == Android.App.Result.Ok:
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraCodigoPostal, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentCp));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaCfdi, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentCfdi));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRz, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentRz));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRfc, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentRfc));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaEmail, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentEmail));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaDireccion, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentDireccion));
                    StartActivity(intent);
                    break;
                case OtraRazonSocialActivity.RequestEditarRazonSocial when resultCode == Result.Ok:
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraCodigoPostal, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentCp));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaCfdi, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentCfdi));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRz, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentRz));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRfc, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentRfc));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaEmail, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentEmail));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaDireccion, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentDireccion));
                    intent.PutExtra(FacturaConfirmacionActivity.ExtraReceptorCliente, data.GetStringExtra(OtraRazonSocialActivity.ExtraIntentIdReceptor));
                    StartActivity(intent);
                    break;
                default:
                    break;
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = FacturacionViewModel.Instance.ReceptoresGuardados[e.Position];
            var intent = new Intent(this, typeof(FacturaConfirmacionActivity));
            var cfdiIdx = ViewModels.FacturacionViewModel.Instance.UsosCfdiAsStringList().IndexOf(item.UsoCFDI);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaCfdi, cfdiIdx);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRz, item.RazonSocial);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaRfc, item.Rfc);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaEmail, item.Email);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraFacturaDireccion, item.Direccion);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraReceptorCliente, item.Id);
            intent.PutExtra(FacturaConfirmacionActivity.ExtraCodigoPostal, item.CodigoPostal);
            StartActivity(intent);
        }

        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += SumarPuntosActivity_Click;
            var bottomSheet = FindViewById(Resource.Id.bottom_sheet);
            _mBottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
            _mBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

            _buttonEditar = FindViewById<Button>(Resource.Id.button_editar);
            _buttonEliminar = FindViewById<Button>(Resource.Id.button_eliminar);
            _buttonCancelar = FindViewById<Button>(Resource.Id.button_cancel);

            _buttonEditar.Click += ButtonEditar_Click;
            _buttonEliminar.Click += ButtonEliminar_Click;
            _buttonCancelar.Click += ButtonCancelar_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _mBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
            FacturacionViewModel.Instance.OnRemoverDatosFiscalesFinished += Instance_OnRemoverDatosFiscalesFinished;
        }

        protected override void OnPause()
        {
            base.OnPause();
            FacturacionViewModel.Instance.OnRemoverDatosFiscalesFinished -= Instance_OnRemoverDatosFiscalesFinished;
        }
        protected override void OnStop()
        {
            base.OnStop();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
            }
            return true;
        }
        public override void OnBackPressed()
        {
            if(_mBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded)
            {
                RunOnUiThread(() =>
                {
                    _mBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                });
                
            }
            else
            {
                base.OnBackPressed();
            }
        }


        private void ButtonCancelar_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void ButtonEliminar_Click(object sender, EventArgs e)
        {
            FacturacionViewModel.Instance.RemoverReceptor(_editingPosition);
            OnBackPressed();
            StartAnimatingLogin();
        }

        private void ButtonEditar_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(OtraRazonSocialActivity));
            var item = FacturacionViewModel.Instance.ReceptoresGuardados[_editingPosition];
            var itemPos = FacturacionViewModel.Instance.UsosCfdiAsStringList().IndexOf(item.UsoCFDI);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentIdReceptor,item.Id);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentCp, item.CodigoPostal);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentDireccion, item.Direccion);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentEmail, item.Email);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentRfc, item.Rfc);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentRz, item.RazonSocial);
            intent.PutExtra(OtraRazonSocialActivity.ExtraIntentCfdi, itemPos);
            StartActivityForResult(intent, OtraRazonSocialActivity.RequestEditarRazonSocial);

        }
        private void SumarPuntosActivity_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(TicketFacturaActivity));

        }
        private void StartAnimatingLogin()
        {

            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        private void StopAnimatingLogin()
        {
            var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = outAnimation;
            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        private void Instance_OnRemoverDatosFiscalesFinished(object sender, MystiqueNative.Helpers.BaseEventArgs e)
        {
            StopAnimatingLogin();
        }
    }
    public class RazonSocialAdapter : BaseRecyclerViewAdapter
    {
        private const int ViewtypeRazonSocial = 0;
        private const int ViewtypeAgregar = 1;
        private readonly ObservableCollection<ReceptorFactura> _viewModel;
        private readonly BaseActivity _context;

        public RazonSocialAdapter(BaseActivity context, ObservableCollection<ReceptorFactura> viewModel)
        {
            this._viewModel = viewModel;
            this._context = context;

            this._viewModel.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }
        public override int GetItemViewType(int position)
        {
            return _viewModel[position].Id != "0" ? ViewtypeRazonSocial : ViewtypeAgregar;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;
            const int idViewholderRazonSocial = Resource.Layout.viewholder_razon_social;
            const int idAgregarRazonSocial = Resource.Layout.viewholder_agregar_razon_social;
            if(ViewtypeRazonSocial == viewType)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(idViewholderRazonSocial, parent, false);
                var vh = new RazonSocialViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            else
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(idAgregarRazonSocial, parent, false);
                var vh = new AgregarRazonSocialViewHolder(itemView, OnClick2, OnClick2);
                return vh;
            }
            
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (GetItemViewType(position) != ViewtypeRazonSocial) return;

            var beneficio = _viewModel[position];

            if (!(holder is RazonSocialViewHolder myHolder)) return;

            myHolder.Rfc.Text = beneficio.Rfc;
            myHolder.Title.Text = beneficio.Email;
        }
        public override int ItemCount => _viewModel.Count;
    }
    public class RazonSocialViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Rfc { get; set; }

        public RazonSocialViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Rfc = itemView.FindViewById<TextView>(Resource.Id.label_item);
            itemView.Click += delegate
            {
                clickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
            itemView.LongClick += delegate
            {
                longClickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
        }
    }
    public class AgregarRazonSocialViewHolder : RecyclerView.ViewHolder
    {
        public AgregarRazonSocialViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
}