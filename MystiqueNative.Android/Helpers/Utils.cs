using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Helpers
{
    public static class Utils
    {
        public static string Md5Hash(string input)
        {
            var hash = new StringBuilder();
            var md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(input));

            foreach (var t in bytes)
                hash.Append(t.ToString("x2"));

            return hash.ToString();
        }

        public static CancellationToken TimeoutToken(CancellationToken cancellationToken, TimeSpan timeout)
        {
            // create a new linked cancellation token source
            var cancelTokenSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // if a timeout was given, make the token source cancel after it expires
            if (timeout > TimeSpan.Zero)
                cancelTokenSrc.CancelAfter(timeout);

            // our Cancel method will handle the actual cancellation logic
            return cancelTokenSrc.Token;
        }
    }
}