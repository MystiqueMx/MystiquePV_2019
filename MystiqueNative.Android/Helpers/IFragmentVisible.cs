using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MystiqueNative.Droid.Helpers
{
    /// <summary>
    /// <para> Fragments interface  </para>
    /// </summary>
    internal interface IFragmentVisible
    {
        void BecameVisible();
    }
}