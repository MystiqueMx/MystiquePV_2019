using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Com.Crashlytics.Android;
using Com.OneSignal;
using IO.Fabric.Sdk.Android;
using MystiqueNative.Droid.Utils;
using MystiqueNative.ViewModels;
using System;

namespace MystiqueNative.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }
        public const string OnesignalAppId = "6c0db721-4fdd-4fe2-961e-679bb6a67327";
        public static BeneficiosViewModel ViewModelBeneficios { get => BeneficiosViewModel.Instance; set => Console.WriteLine("Deprecated"); }
        public static NotificacionesViewModel ViewModelNotificaciones { get => NotificacionesViewModel.Instance; set => Console.WriteLine("Deprecated"); }
        public static ComentariosViewModel ViewModelComentarios { get => ComentariosViewModel.Instance; set => Console.WriteLine("Deprecated"); }
        public static WalletViewModel ViewModelWallet { get; set; }
        public static CitypointsViewModel ViewModelCitypoints { get => CitypointsViewModel.Instance; set => Console.WriteLine("Deprecated"); }

        public static bool LogAnalytics { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
#if DEBUG
                // DONT LOG DEBUG BUILD CRASHES :)
                //Fabric.With(this, new Crashlytics());
#else
                Fabric.With(this, new Crashlytics());
#endif

                MystiqueNative.Helpers.ServiceLocator.Instance.Register<Interfaces.IOpenPaySessionId, Utils.OpenPay.OpenPaySessionIdImpl>();
                //TODO REMOVE CONEKTA
                //HERE
                MystiqueNative.Helpers.ServiceLocator.Instance.Register<Interfaces.IConektaSessionId, Utils.Conekta.ConektaSessionIdImpl>();

                OneSignal
                    .Current
                    .StartInit(OnesignalAppId)
                    .EndInit();
                OneSignal.Current.IdsAvailable(RegistrarPlayerId);

                CurrentActivityDelegate.Instance.Init(this);

                MystiqueApp.DevicePlatform = "Android";
                MystiqueApp.DeviceModel = Build.Model;
                MystiqueApp.DeviceVersion = Build.VERSION.Sdk;
                MystiqueApp.DeviceId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                MystiqueApp.DeviceConnectionType = "Mobile";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

#if DEBUG
            MainApplication.LogAnalytics = false;
#else
            MainApplication.LogAnalytics = true;
#endif
        }

        private static void RegistrarPlayerId(string playerId, string pushToken)
        {
            MystiqueApp.PlayerId = playerId;
            try
            {
                OneSignal.Current.SendTag("Empresa", Configuration.MystiqueApiV2Config.MystiqueAppEmpresa.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
#pragma warning restore S1215 // "GC.Collect" should not be called
            base.OnTrimMemory(level);
        }
        public override void OnTerminate()
        {
            base.OnTerminate();
        }
    }
}