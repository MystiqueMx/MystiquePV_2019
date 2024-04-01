using System;
using System.Threading.Tasks;
using Android.Provider;
using Android.Webkit;
using Java.Util;
using MystiqueNative.Interfaces;

namespace MystiqueNative.Droid.Utils.OpenPay
{
    public class OpenPaySessionIdImpl : IOpenPaySessionId
    {
        Task<string> IOpenPaySessionId.CreateDeviceSessionIdInternal(string merchantId, string apiKey, string baseUrl)
        {
            if (null == CurrentActivityDelegate.Instance.Activity)
            {
                throw new InvalidOperationException("Activity has not been initialized.");
            }

            var sessionId = UUID.RandomUUID().ToString();
            sessionId = sessionId.Replace("-", string.Empty);

            var identifierForVendor = Settings.Secure.GetString(CurrentActivityDelegate.Instance.Activity.ContentResolver, Settings.Secure.AndroidId);
            var identifierForVendorScript = $"var identifierForVendor = '{identifierForVendor}';";

            using (var webView = new WebView(CurrentActivityDelegate.Instance.Activity))
            {
                webView.SetWebViewClient(new WebViewClient());
                webView.Settings.JavaScriptEnabled = true;
                webView.EvaluateJavascript(identifierForVendorScript, null);

                var url = $"{baseUrl}/oa/logo.htm?m={merchantId}&s={sessionId}";
                Console.WriteLine($"IOpenPaySessionId.CreateDeviceSessionIdInternal: {url}");
                Console.WriteLine($"IOpenPaySessionId.CreateDeviceSessionIdInternal.identifierForVendorScript: {identifierForVendorScript}");
                webView.LoadUrl(url);

                return Task.FromResult(sessionId);
            }
        }
    }
}