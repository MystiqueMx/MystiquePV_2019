using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Text;
using Java.IO;
using Java.Lang;
using Java.Util;
using Android.Gms.Maps;
using Exception = Java.Lang.Exception;
using Math = System.Math;

namespace MystiqueNative.Droid.Utils
{
    [Service]
    public class AddressResolverIntentService : IntentService
    {
        #region EXPORTS
        public const string AddressReceiverExtra = "com.GrupoRed.Fresco.RECEIVER";
        public const string ResultCountryCodeExtra = "com.GrupoRed.Fresco.ResultCountryCodeExtra";
        public const string ResultCountryNameExtra = "com.GrupoRed.Fresco.ResultCountryNameExtra";
        public const string ResultLocalityExtra = "com.GrupoRed.Fresco.ResultLocalityExtra";
        public const string ResultSublocalityExtra = "com.GrupoRed.Fresco.ResultSublocalityExtra";
        public const string ResultPostalCodeExtra = "com.GrupoRed.Fresco.ResultPostalCodeExtra";
        public const string ResultAdminAreaExtra = "com.GrupoRed.Fresco.ResultAdminAreaExtra";
        public const string ResultSubAdminAreaExtra = "com.GrupoRed.Fresco.ResultSubAdminAreaExtra";
        public const string ResultThoroughfareExtra = "com.GrupoRed.Fresco.ResultThoroughfareExtra";
        public const string ResultSubThoroughfareExtra = "com.GrupoRed.Fresco.ResultSubThoroughfareExtra";
        public const string ResultFeatureExtra = "com.GrupoRed.Fresco.ResultFeatureExtra";
        public const string ResultAddrExtra = "com.GrupoRed.Fresco.ResultAddrExtra";
        public const string ResultErrorExtra = "com.GrupoRed.Fresco.RESULT_Error_message";
        public const string LatitudeDataExtra = "com.GrupoRed.Fresco.Latitude_DATA_EXTRA";
        public const string LongitudeDataExtra = "com.GrupoRed.Fresco.Longitude_DATA_EXTRA";
        #endregion
        protected ResultReceiver ResultReceiverObj;
        
        protected override void OnHandleIntent(Intent intent)
        {
            if (intent == null) return;
            var errorMessage = string.Empty;
            double latitude = 1000;
            double longitude = 1000;
            var geocoder = new Geocoder(this, new Locale("es","MX"));
            try
            {
                latitude = intent.GetDoubleExtra(LatitudeDataExtra, 1000);
                longitude = intent.GetDoubleExtra(LongitudeDataExtra, 1000);
                var rr = intent.GetParcelableExtra(AddressReceiverExtra);
                ResultReceiverObj = (ResultReceiver)rr;
                if(Math.Abs(latitude - 1000) < 1) throw new ArgumentException(nameof(latitude));
                if(Math.Abs(longitude - 1000) < 1) throw new ArgumentException(nameof(longitude));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Android.Util.Log.Error(nameof(AddressResolverIntentService), ex.Message);
                Android.Util.Log.Error(nameof(AddressResolverIntentService), ex.StackTrace);
                DeliverResultToReceiver(Result.Canceled, errorMessage);
            }
            
            try
            {
                var addresses = geocoder.GetFromLocation(latitude, longitude, 1);
                if (addresses == null || addresses.Count == 0)
                {
                    errorMessage = "No se encontro una direccion para la ubicación";
                    DeliverResultToReceiver(Result.Canceled, errorMessage);
                } 
                else 
                {
                    if (addresses.Count > 1)
                    {
                        var countryCode = addresses.Select(c => c.CountryCode)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var countryname = addresses.Select(c => c.CountryName)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var adminarea = addresses.Select(c => c.AdminArea)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var subadminarea = addresses.Select(c => c.SubAdminArea)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var locality = addresses.Select(c => c.Locality)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var subLocality = addresses.Select(c => c.SubLocality)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var postalCode = addresses.Select(c => c.PostalCode)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var thoroughfare = addresses.Select(c => c.Thoroughfare)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var subThoroughfare = addresses.Select(c => c.SubThoroughfare)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;
                        var featureName = addresses.Select(c => c.FeatureName)
                            .FirstOrDefault(c => !string.IsNullOrEmpty(c))
                            ?? string.Empty;

                        DeliverResultToReceiver(Result.Ok, countryCode, countryname, adminarea,
                            subadminarea, locality, subLocality, postalCode,
                            thoroughfare, subThoroughfare, featureName, string.Empty);
                    }
                    else
                    {
                        DeliverResultToReceiver(Result.Ok, addresses[0].CountryCode, addresses[0].CountryName, addresses[0].AdminArea,
                            addresses[0].SubAdminArea, addresses[0].Locality, addresses[0].SubLocality, addresses[0].PostalCode,
                            addresses[0].Thoroughfare, addresses[0].SubThoroughfare, addresses[0].FeatureName, string.Empty);
                    }
                    
                }
            }
            catch (IOException ioException)
            {
                errorMessage = "Servicio no disponible";
                Android.Util.Log.Error(nameof(AddressResolverIntentService), ioException.Message);
                Android.Util.Log.Error(nameof(AddressResolverIntentService), ioException.StackTrace);
                DeliverResultToReceiver(Result.Canceled, errorMessage);
            }
            catch (IllegalArgumentException iaException)
            {
                errorMessage = $"Error en las coordenadas: ({latitude},{longitude})";
                Android.Util.Log.Error(nameof(AddressResolverIntentService), iaException.Message);
                Android.Util.Log.Error(nameof(AddressResolverIntentService), iaException.StackTrace);
                DeliverResultToReceiver(Result.Canceled, errorMessage);
            }
            
        }

        private void DeliverResultToReceiver(Result ok, string countryCode, string countryName, string adminArea, string subAdminArea,
             string locality, string subLocality, string postalCode, string thoroughfare, string subThoroughfare, string featureName, string addressLines)
        {
            var bundle = new Bundle();
            
            bundle.PutString(ResultCountryCodeExtra, countryCode);
            bundle.PutString(ResultCountryNameExtra, countryName);
            bundle.PutString(ResultAdminAreaExtra, adminArea);
            bundle.PutString(ResultSubAdminAreaExtra, subAdminArea);
            bundle.PutString(ResultLocalityExtra, locality);
            bundle.PutString(ResultSublocalityExtra, subLocality);
            bundle.PutString(ResultPostalCodeExtra, postalCode);
            bundle.PutString(ResultThoroughfareExtra, thoroughfare);
            bundle.PutString(ResultSubThoroughfareExtra, subThoroughfare);
            bundle.PutString(ResultFeatureExtra, featureName);
            bundle.PutString(ResultAddrExtra, addressLines);

            ResultReceiverObj.Send(ok, bundle);
        }

        private void DeliverResultToReceiver(Result resultCode, string message) {
            var bundle = new Bundle();
            
            bundle.PutString(ResultErrorExtra, message);

            ResultReceiverObj.Send(resultCode, bundle);
        }
    }
}