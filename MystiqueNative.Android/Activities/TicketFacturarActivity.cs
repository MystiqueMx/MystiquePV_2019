using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using MystiqueNative.Droid.Helpers;
using Android.Support.V4.Widget;
using MystiqueNative.ViewModels;
using Android.Support.Design.Widget;
using MystiqueNative.Helpers;
using FFImageLoading;
using FFImageLoading.Views;
using ZXing.Mobile;
using ZXing;
using Android.Media;

namespace MystiqueNative.Droid
{
    [Activity(Label = "Escaner ticket", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TicketFacturaActivity : BaseActivity, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnPreparedListener
    {
        protected override int LayoutResource => Resource.Layout.activity_ticket_factura;
        #region FRAGMENTS

        private ZXingScannerFragment _scannerFragment;
        #endregion
        #region VIEWS

        private FrameLayout _progressBarHolder;
        private VideoView _tutorialVideo;
        private FrameLayout _scannerOverlay;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppendScannerFragment();

        }


        private void GrabViews()
        {
            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);
            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += SumarPuntosActivity_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            FacturacionViewModel.Instance.OnValidarTicketFinished += Instance_OnValidarTicketFinished;
            _scannerFragment.ResumeAnalysis();
            if (!PermissionsHelper.ValidatePermissionsForCamera())
            {
                Finish();
            }
            GrabViews();
            StartScanning();

        }

        protected override void OnPause()
        {
            StopScanning();
            base.OnPause();
            FacturacionViewModel.Instance.OnValidarTicketFinished -= Instance_OnValidarTicketFinished;
            _scannerFragment.PauseAnalysis();
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                _tutorialVideo.Pause();
            }
        }
        private void Instance_OnValidarTicketFinished(object sender, ValidarTicketEventArgs e)
        {
            StopAnimatingLogin();
            if (e.Success)
            {
                StartActivity(typeof(RazonesSocialesActivity));
            }
            else
            {
                SendConfirmation(e.Message, "", "Salir", "Escanear otro código", ok =>
                {
                    if (ok)
                    {
                        Finish();
                    }
                    else
                    {
                        _scannerFragment.ResumeAnalysis();
                    }

                });
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
            UnloadTutorialVideo();
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
        private void AppendScannerFragment()
        {
            var cameraOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.zxing_qr_overlay, null);
            _scannerOverlay = cameraOverlay.FindViewById<FrameLayout>(Resource.Id.sumar_camera_overlay);
            _tutorialVideo = cameraOverlay.FindViewById<VideoView>(Resource.Id.videoview);

            SetUpTutorialVideo();

            _scannerFragment = new ZXingScannerFragment
            {
                UseCustomOverlayView = true,
                CustomOverlayView = cameraOverlay,

            };
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.sumar_scanner_fragment, _scannerFragment)
                .Commit();

            _scannerOverlay.Click += ScannerOverlay_Click;
        }

        private void SetUpTutorialVideo()
        {
            var videoResource = Android.Net.Uri.Parse("android.resource://" + PackageName + "/raw/tutorial_video");
            _tutorialVideo.SetVideoURI(videoResource);
            _tutorialVideo.SetOnCompletionListener(this);
            _tutorialVideo.SetOnPreparedListener(this);
            _tutorialVideo.Start();
        }
        private void UnloadTutorialVideo()
        {
            _tutorialVideo.StopPlayback();
        }
        private void ScannerOverlay_Click(object sender, EventArgs e)
        {

            HideOverlay();
        }

        private void HideOverlay()
        {
            _scannerOverlay.Visibility = ViewStates.Gone;
            _tutorialVideo.Visibility = ViewStates.Gone;
            UnloadTutorialVideo();
        }

        private void SumarPuntosActivity_Click(object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;
            if (_scannerFragment == null) return;
            _scannerFragment.ToggleTorch();
            fab.SetImageResource(_scannerFragment.IsTorchOn
                ? Resource.Drawable.ic_flashlight_off_white_24dp
                : Resource.Drawable.ic_flashlight_white_24dp);
        }
        private void StartScanning()
        {


            var opts = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new System.Collections.Generic.List<BarcodeFormat> {
                    BarcodeFormat.QR_CODE
                },
                DelayBetweenContinuousScans = 3000,
            };
            _scannerFragment.StartScanning(OnScanResultReceived, opts);
        }
        private void StopScanning() => _scannerFragment?.StopScanning();
        private void OnScanResultReceived(ZXing.Result code)
        {
            RunOnUiThread(() => {
                HideOverlay();
                StartAnimatingLogin();
                _scannerFragment.PauseAnalysis();
            });
            if (code == null || string.IsNullOrEmpty(code.Text))
            {
                RunOnUiThread(() => {
                    SendToast("Scanning Cancelled");
                    StopAnimatingLogin();
                    _scannerFragment.ResumeAnalysis();
                });

            }
            else
            {
                RunOnUiThread(() => {
                    FacturacionViewModel.Instance.ValidarTicket(code.Text);
                    //StopAnimatingLogin();
                    //SendToast(code.Text);
                    //StartActivity(typeof(RazonesSocialesActivity));
                });

            }
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

        void MediaPlayer.IOnCompletionListener.OnCompletion(MediaPlayer mp)
        {
            if (_scannerOverlay.Visibility == ViewStates.Visible)
            {
                mp.SeekTo(1);
            }
        }

        void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer mp)
        {
            try
            {
                mp.SetVolume(0f, 0f);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.StackTrace);
#endif
            }
        }
    }
}