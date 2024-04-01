using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Util;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BarronWellnessMovil.Droid.Helpers;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using MystiqueNative.Droid.Activities;
using MystiqueNative.Droid.Animations;
using MystiqueNative.Droid.Fragments;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.ViewModels;
using System;

namespace MystiqueNative.Droid
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ParentActivity = typeof(LandingHasbroActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".LandingActivity")]
    public class BeneficiosActivity : BaseActivity
    {
        private SwipeRefreshLayout _refreshLayout;
        private BeneficiosAdapter _adapter;
        private ProgressBar _progress;
        #region ActivityResult

        private Models.BeneficiosSucursal _b;
        private bool _isReturning;
        #endregion

        protected override int LayoutResource => Resource.Layout.activity_beneficios;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new StaggeredGridLayoutManager(2, LinearLayoutManager.Vertical));
            recyclerView.SetAdapter(_adapter = new BeneficiosAdapter(this, MainApplication.ViewModelBeneficios));

            _refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refresh_layout);
            _refreshLayout.SetColorSchemeColors(Resource.Color.accent);

            _progress = FindViewById<ProgressBar>(Resource.Id.progressbar);

            FindViewById<Button>(Resource.Id.ic_wallet).Click += (s, e) =>
            {
                StartRevealActivity(s as View);
            };
            _refreshLayout.Enabled = false;

        }

        private void ViewModelBeneficios_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                if (MainApplication.ViewModelBeneficios.IsBusy)
                {
                    _progress.Visibility = ViewStates.Visible;
                }
                else
                {
                    _progress.Visibility = ViewStates.Gone;
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (SupportActionBar != null)
                SupportActionBar.Title = GetString(Resource.String.title_beneficios);

            if (MainApplication.ViewModelWallet == null)
            {
                MainApplication.ViewModelWallet = new WalletViewModel();
            }
            MainApplication.ViewModelWallet.PropertyChanged += ViewModelWallet_PropertyChanged;
            MainApplication.ViewModelBeneficios.PropertyChanged += ViewModelBeneficios_PropertyChanged;
            _adapter.ItemClick += AgregarFavoritosClick;
            _adapter.ItemLongClick += CanjearClick;
            _adapter.ItemClick2 += Adapter_ItemClick2;

            if (MainApplication.ViewModelBeneficios.Beneficios.Count == 0)
                MainApplication.ViewModelBeneficios.ObtenerTodosLosBeneficios();

            ShowDialogAfterResult();
        }

        private void ShowDialogAfterResult()
        {
            if (_isReturning)
            {
                new BenefitCardDetailDialogFragment("Beneficio seleccionado: " + _b.Descripcion, _b.CadenaCodigo)
                    .Show(SupportFragmentManager, "QRBENEFICIO_MODAL");
                _isReturning = false;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            MainApplication.ViewModelBeneficios.PropertyChanged -= ViewModelBeneficios_PropertyChanged;
            MainApplication.ViewModelWallet.PropertyChanged -= ViewModelWallet_PropertyChanged;
            _adapter.ItemClick -= AgregarFavoritosClick;
            _adapter.ItemLongClick -= CanjearClick;
            _adapter.ItemClick2 -= Adapter_ItemClick2;
        }
        private void Adapter_ItemClick2(object sender, RecyclerClickEventArgs e)
        {
            var b = MainApplication.ViewModelBeneficios.Beneficios[e.Position];
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, string.Format("Obtén el beneficio {0} descargando la aplicación: {1}", b.Descripcion, "https://play.google.com/store/apps/details?id=com.GrupoRed.Fresco"));
            sendIntent.SetType("text/plain");
            StartActivity(Intent.CreateChooser(sendIntent, "Mira este beneficio"));
        }


        private void CanjearClick(object sender, RecyclerClickEventArgs e)
        {
            _b = MainApplication.ViewModelBeneficios.Beneficios[e.Position];
            if (AuthViewModelV2.Instance.Usuario.RegistroCompleto)
            {
                new BenefitCardDetailDialogFragment("Beneficio seleccionado: " + _b.Descripcion, _b.CadenaCodigo)
                    .Show(SupportFragmentManager, "QRBENEFICIO_MODAL");
            }
            else
            {
                SendConfirmation(GetString(Resource.String.error_registro_incompleto), "", "Completar ahora", "Ahora no", accept =>
                {
                    if (!accept) return;

                    Intent intent = new Intent(this, typeof(UpdateProfileActivity));
                    StartActivityForResult(intent, UpdateProfileActivity.UpdateRequestCode);
                });
            }

        }

        private static void AgregarFavoritosClick(object sender, RecyclerClickEventArgs e)
        {
            var b = MainApplication.ViewModelBeneficios.Beneficios[e.Position];

        }
        private void ViewModelWallet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                //if (MainApplication.ViewModelWallet.IsBusy)
                //{
                //    StartAnimatingLogin();
                //}
                //else
                //{
                //    StopAnimatingLogin();
                //}
            }
            if (e.PropertyName == "AgregarStatus")
            {
                if (!string.IsNullOrEmpty(MainApplication.ViewModelWallet.ErrorMessage))
                {
                    SendToast(MainApplication.ViewModelWallet.ErrorMessage);
                    MainApplication.ViewModelWallet.ErrorMessage = string.Empty;
                }

                if (MainApplication.ViewModelWallet.AgregarStatus)
                {
                    SendConfirmation(GetString(Resource.String.content_confirmacion_cerrar),
                        GetString(Resource.String.title_confirmacion_cerrar),
                        GetString(Resource.String.label_ir_wallet),
                        GetString(Resource.String.label_cancelar),
                        ok =>
                        {
                            if (!ok) return;

                            Intent intent = new Intent(this, typeof(LandingHasbroActivity));
                            StartActivity(intent);
                        });
                }

            }

        }
        private void StartRevealActivity(View v)
        {
            int revealX = (int)(v.GetX() + v.Width / 2);
            int revealY = (int)(v.GetY() + v.Height / 2);

            Intent intent = new Intent(this, typeof(LandingHasbroActivity));
            intent.PutExtra(RevealAnimation.ExtraCircularRevealX, revealX);
            intent.PutExtra(RevealAnimation.ExtraCircularRevealY, revealY);
            StartActivity(intent, null);

            OverridePendingTransition(0, 0);
        }
#pragma warning disable S1144 // Unused private types or members should be removed
        private void TryMakeTransition(Intent intent, View v)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Pair p1 = Pair.Create(v.FindViewById<ImageViewAsync>(Resource.Id.image_icon), "profile");
                Pair p2 = Pair.Create(v.FindViewById<TextView>(Resource.Id.label_title), "title");
                //Pair<View, String> p3 = Pair.create((View)tvName, "text");
                ActivityOptionsCompat options = ActivityOptionsCompat.
                            MakeSceneTransitionAnimation(this, p1, p2);
                //ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this, v, "profile");
                StartActivity(intent, options.ToBundle());
            }
            else
            {
                StartActivity(intent);
            }

        }
#pragma warning restore S1144 // Unused private types or members should be removed
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            _isReturning = requestCode == UpdateProfileActivity.UpdateRequestCode
                && resultCode == Result.Ok;
            base.OnActivityResult(requestCode, resultCode, data);
        }

    }

    internal class BeneficiosAdapter : BaseRecyclerViewAdapter
    {
        private BeneficiosViewModel _viewModel;
        private Activity _context;

        public BeneficiosAdapter(Activity context, BeneficiosViewModel viewModel)
        {
            _viewModel = viewModel;
            _context = context;

            _viewModel.Beneficios.CollectionChanged += (sender, args) =>
            {
                _context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.viewholder_beneficios;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new BeneficiosViewHolder(itemView, OnClick, OnLongClick, OnClick2);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel.Beneficios[position];
            var myHolder = holder as BeneficiosViewHolder;

            myHolder.Title.Text = beneficio.Descripcion;
            if (string.IsNullOrEmpty(beneficio.ImgBeneficio))
            {
                ImageService.Instance
                    .LoadCompiledResource("white_img2")
                    .Transform(new TintTransformation(0, 0, 0, 0) { EnableSolidColor = true })
                    .DownSampleInDip(width: 500)
                    .IntoAsync(myHolder.Image);
            }
            else
            {
                ImageService.Instance
                    .LoadUrl(beneficio.ImgBeneficio)
                    .LoadingPlaceholder("white_img2", FFImageLoading.Work.ImageSource.CompiledResource)
                    .Transform(new TintTransformation(0, 0, 0, 0) { EnableSolidColor = true })
                    .DownSampleInDip(width: 500)
                    .IntoAsync(myHolder.Image);
            }
        }
        public override int ItemCount => _viewModel.Beneficios.Count;
    }
    public class BeneficiosViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public ImageViewAsync Image { get; set; }
        public BeneficiosViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener, Action<RecyclerClickEventArgs> click2Listener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.image_icon);
            itemView.FindViewById<Button>(Resource.Id.btn_codigo_qr).Click += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.FindViewById<ImageButton>(Resource.Id.btn_share).Click += (sender, e) => click2Listener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

    }
}