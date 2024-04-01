using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Utils;
using static Android.Support.V4.Content.ContextCompat;

namespace MystiqueNative.Droid.Helpers
{
    /// <summary>
    /// <para> Wrapper para validar y solicitar permisos segun la version del API  </para>
    /// </summary>
    public class PermissionsHelper
    {
        #region REQUESTS ID
        public const int RequestFingerprintId = 666;
        public const int RequestStorageId = 667;
        public const int RequestCameraId = 668;
        public const int RequestLocationId = 669;
        #endregion
        #region CONST PERMISSIONS

        private const string ReadExternalStorage = Manifest.Permission.ReadExternalStorage;
        private const string WriteExternalStorage = Manifest.Permission.WriteExternalStorage;
        private const string UseFingerprint = Manifest.Permission.UseFingerprint;
        private const string UseCamera = Manifest.Permission.Camera;
        private const string UseFlashlight = Manifest.Permission.Flashlight;
        private const string UseCoarseLocation = Manifest.Permission.AccessCoarseLocation;
        private const string UseFineLocation = Manifest.Permission.AccessFineLocation;
        private const string UseLocationExtra = Manifest.Permission.AccessLocationExtraCommands;
        #endregion
        #region METHODS
        private static bool ValidateExternalStoragePermissions()
        {
            PermissionsToRequest = new List<string>();
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, ReadExternalStorage) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(ReadExternalStorage);
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, WriteExternalStorage) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(WriteExternalStorage);

            return PermissionsToRequest.Count == 0;
        }
        private static bool ValidateFingerprintPermissions()
        {
            PermissionsToRequest = new List<string>();
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseFingerprint) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(UseFingerprint);

            return PermissionsToRequest.Count == 0;
        }
        private static bool ValidateCameraPermissionss()
        {
            PermissionsToRequest = new List<string>();
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseCamera) != (int)Permission.Granted)
                PermissionsToRequest.Add(UseCamera);
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseFlashlight) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(UseFlashlight);
            return PermissionsToRequest.Count == 0;
        }
        private static bool ValidateLocationPermissions()
        {
            PermissionsToRequest = new List<string>();
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseCoarseLocation) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(UseCoarseLocation);
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseFineLocation) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(UseFineLocation);
            if (CheckSelfPermission(CurrentActivityDelegate.Instance.Activity, UseLocationExtra) !=
                (int) Permission.Granted)
                PermissionsToRequest.Add(UseLocationExtra);
            return PermissionsToRequest.Count == 0;
        }
        #endregion
        #region API
        public static List<string> PermissionsToRequest = new List<string>();
        //public static bool IsFingerprintDeviceEnabled()
        //{
        //    FingerprintManagerCompat fingerprintManager = FingerprintManagerCompat.From(CrossCurrentActivity.Current.Activity);
        //    return fingerprintManager.IsHardwareDetected;
        //}
        //public static bool ValidatePermissionsForFingerprint()
        //{
        //    if ((int)Build.VERSION.SdkInt < 23)
        //        return true;
        //    else
        //        return ValidateFingerprintPermissions();
        //}
        public static bool ValidatePermissionsForCamera() => Build.VERSION.SdkInt < BuildVersionCodes.M || ValidateCameraPermissionss();
        public static bool ValidatePermissionsForLocation() => Build.VERSION.SdkInt < BuildVersionCodes.M || ValidateLocationPermissions();
        public static bool ValidatePermissionsForStorageRw() => Build.VERSION.SdkInt < BuildVersionCodes.M || ValidateExternalStoragePermissions();

        #endregion

    }
}