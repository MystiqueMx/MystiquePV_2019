using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Fragments;
using Android.Support.V4.Widget;
using BarronWellnessMovil.Droid.Helpers;
using MystiqueNative.ViewModels;
using FFImageLoading;
using FFImageLoading.Views;
using Android.Support.V4.Util;
using Android.Support.V4.App;
using MystiqueNative.Models;
using Android.Runtime;

namespace MystiqueNative.Droid
{
    [Activity(ParentActivity = typeof(LandingHasbroActivity), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".CitypointsActivity")]
    public class RecompensasActivity : BaseActivity
    {
        #region VIEWS

        private TextView _estadoCuenta;
        private TextView _labelVacio;
        private SwipeRefreshLayout _refreshLayout;
        private FrameLayout _progressBarHolder;
        private ProgressBar _progress;
        #endregion
        #region FIELDS

        private string _titleModal;
        private RecompensasAdapter _adapter;
        private Recompensa _recompensa;
        #endregion
        protected override int LayoutResource => Resource.Layout.activity_recompensas;

        public bool IsReturning { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _estadoCuenta = FindViewById<TextView>(Resource.Id.label_city_points_count);
            _labelVacio = FindViewById<TextView>(Resource.Id.label_vacio);
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            //progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            SetupRecyclerView();
        }

        private void SetupRecyclerView()
        {
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.HasFixedSize = true;
            recyclerView.SetLayoutManager(new StaggeredGridLayoutManager(2, LinearLayoutManager.Vertical));
            
            recyclerView.SetAdapter(_adapter = new RecompensasAdapter(this, MainApplication.ViewModelCitypoints));
            _refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refresh_layout);
            _refreshLayout.SetColorSchemeColors(Resource.Color.accent);
            _refreshLayout.Enabled = false;
            _progress = FindViewById<ProgressBar>(Resource.Id.progressbar);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (SupportActionBar != null)
                SupportActionBar.Title = GetString(Resource.String.title_canjear);

            if (MainApplication.ViewModelCitypoints == null)
                MainApplication.ViewModelCitypoints = new CitypointsViewModel();

            MainApplication.ViewModelCitypoints.OnEstadoCuentaFinished += ViewModelCitypoints_OnEstadoCuentaFinished;
            MainApplication.ViewModelCitypoints.OnCanjearRecompensaFinished += ViewModelCitypoints_OnCanjearRecompensaFinished;

            _adapter.ItemClick += Adapter_ItemClick;
            if (IsInternetAvailable)
            {
                //MainApplication.ViewModelCitypoints.ObtenerEstadoCuenta();

                MainApplication.ViewModelCitypoints.ObtenerRecompensas();
            }
            else
            {
                SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                {
                    if (accept)
                    {
                        StartActivity(new Intent(Android.Provider.Settings.ActionWirelessSettings));
                    }
                });
            }
            ShowQrIfReturning();

        }

        private void ShowQrIfReturning()
        {
            if (IsReturning)
            {
                if (MainApplication.ViewModelCitypoints.EstadoCuenta == null)
                {
                    SendToast(GetString(Resource.String.label_saldo_insuficiente));
                }
                else
                {
                    if (MainApplication.ViewModelCitypoints.EstadoCuenta.PuntosAsInt < _recompensa.CostoAsInt)
                    {
                        SendToast(GetString(Resource.String.label_saldo_insuficiente));
                    }
                    else
                    {
                        TryGetQr(_recompensa.Id);
                        Title = _recompensa.Nombre;
                    }
                }
            }
            
        }

        private void ViewModelCitypoints_OnCanjearRecompensaFinished(object sender, CanjearRecompensaArgs e)
        {
            StopLoadingAnimation();
            if (e.Success)
            {
                var dialog = new BenefitCardDetailDialogFragment("Recompensa seleccionada: " + Title, e.CodigoCanje.CodigoQR);
                dialog.Show(SupportFragmentManager, "CanjePuntos");
            }
            else
            {
                if (!string.IsNullOrEmpty(e.ErrorMessage))
                {
                    SendMessage(e.ErrorMessage);
                }
            }
        }

        private void ViewModelCitypoints_OnEstadoCuentaFinished(object sender, EstadoCuentaArgs e)
        {
            if (e.EstadoCuenta != null)
            {
                _estadoCuenta.Text = e.EstadoCuenta.PuntosAsInt.ToString();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _adapter.ItemClick -= Adapter_ItemClick;
            MainApplication.ViewModelCitypoints.OnEstadoCuentaFinished -= ViewModelCitypoints_OnEstadoCuentaFinished;
            MainApplication.ViewModelCitypoints.OnCanjearRecompensaFinished -= ViewModelCitypoints_OnCanjearRecompensaFinished;
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            _recompensa = MainApplication.ViewModelCitypoints.Recompensas[e.Position];
            if (ViewModels.AuthViewModelV2.Instance.Usuario.RegistroCompleto)
            {
                if (MainApplication.ViewModelCitypoints.EstadoCuenta == null)
                {
                    SendToast(GetString(Resource.String.label_saldo_insuficiente));
                }
                else
                {
                    if (MainApplication.ViewModelCitypoints.EstadoCuenta.PuntosAsInt < _recompensa.CostoAsInt)
                    {
                        SendToast(GetString(Resource.String.label_saldo_insuficiente));
                    }
                    else
                    {
                        TryGetQr(_recompensa.Id);
                        Title = _recompensa.Nombre;
                    }
                }
            }
            else
            {
                SendConfirmation(GetString(Resource.String.error_registro_incompleto), "", "Completar ahora", "Ahora no", accept =>
                {
                    if (!accept) return;
                    Intent intent = new Intent(this, typeof(Activities.UpdateProfileActivity));
                    StartActivityForResult(intent, Activities.UpdateProfileActivity.UpdateRequestCode);
                });
            }
            
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            IsReturning = requestCode == Activities.UpdateProfileActivity.UpdateRequestCode
                && resultCode == Result.Ok;
            base.OnActivityResult(requestCode, resultCode, data);
        }
        private void TryGetQr(string id)
        {
            if (IsInternetAvailable)
            {
                StartLoadingAnimation();
                MainApplication.ViewModelCitypoints.CanjearRecompensa(id);
            }
            else
            {
                SendConfirmation(GetString(Resource.String.error_no_conexion), "Sin conexión", accept =>
                {
                    if (accept)
                    {
                        StartActivity(new Intent(Android.Provider.Settings.ActionWirelessSettings));
                    }
                });
            }
        }

        private void TryMakeTransition(Intent intent, View v)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Pair p1 = Pair.Create(v.FindViewById<ImageViewAsync>(Resource.Id.image_icon), "profile");
                Pair p2 = Pair.Create(v.FindViewById<TextView>(Resource.Id.label_title), "title");
                Pair p3 = Pair.Create(FindViewById<LinearLayout>(Resource.Id.header_puntos), "header");
                ActivityOptionsCompat options = ActivityOptionsCompat.
                            MakeSceneTransitionAnimation(this, p1, p2, p3);
                StartActivity(intent, options.ToBundle());
            }
            else
            {
                StartActivity(intent);
            }

        }
        #region LOADING
        public void StartLoadingAnimation()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 200
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }
        public void StopLoadingAnimation()
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

    internal class RecompensasAdapter : BaseRecyclerViewAdapter
    {
        private readonly CitypointsViewModel _viewModel;
        private readonly Activity _context;

        public RecompensasAdapter(Activity context, CitypointsViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._context = context;

            this._viewModel.Recompensas.CollectionChanged += (sender, args) =>
            {
                this._context.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            const int id = Resource.Layout.viewholder_recompensas;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new RecompensasViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var beneficio = _viewModel.Recompensas[position];

            if (!(holder is RecompensasViewHolder myHolder)) return;

            myHolder.Title.Text = beneficio.Nombre;
            myHolder.Points.Text = $"{beneficio.CostoAsInt} pts";
            if (string.IsNullOrEmpty(beneficio.ImgRecompensa))
            {
                ImageService.Instance
                    .LoadEmbeddedResource("@drawable/imgMenuCPCanjear")
                    .DownSampleInDip(width: 400)
                    .IntoAsync(myHolder.Image);
            }
            else
            {
                ImageService.Instance
                    .LoadUrl(beneficio.ImgRecompensa)
                    .DownSampleInDip(width: 200)
                    .IntoAsync(myHolder.Image);
            }
        }
        public override int ItemCount => _viewModel.Recompensas.Count;
    }
    public class RecompensasViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Points { get; set; }
        public ImageViewAsync Image { get; set; }
        public RecompensasViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.label_title);
            Points = itemView.FindViewById<TextView>(Resource.Id.label_valor);
            Image = itemView.FindViewById<ImageViewAsync>(Resource.Id.image_icon);
            itemView.Click += delegate
            {
                clickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
            itemView.LongClick += (sender, e) =>
            {
                longClickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            };
        }

    }
}