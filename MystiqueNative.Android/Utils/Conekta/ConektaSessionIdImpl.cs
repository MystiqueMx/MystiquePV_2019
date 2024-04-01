using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using MystiqueNative.Interfaces;

namespace MystiqueNative.Droid.Utils.Conekta
{
    public class ConektaSessionIdImpl : IConektaSessionId
    {
        public string GetDeviceFingerPrint() => Build.Serial;

        public void SetFingerprint(string fingerprint)
        {
            var sessionId = fingerprint;
            var html = "<!DOCTYPE html><html><head></head><body style=\"background: blue;\">";
            html += "<script type=\"text/javascript\" src=\"https://conektaapi.s3.amazonaws.com/v0.5.0/js/conekta.js\" data-conekta-public-key=\"" + Configuration.ConektaApiConfig.PublicKey + "\" data-conekta-session-id=\"" + sessionId + "\"></script>";
            html += "</body></html>";

            var webView = new WebView(Application.Context);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.DomStorageEnabled = true;
            webView.LoadDataWithBaseURL(Configuration.ConektaApiConfig.UriConektaJs, html, "text/html", "UTF-8", null);
        }
    }
}