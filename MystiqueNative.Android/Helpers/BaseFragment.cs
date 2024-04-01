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
using SupportFragment = Android.Support.V4.App.Fragment;

namespace MystiqueNative.Droid.Helpers
{
    public abstract class BaseFragment : SupportFragment, IFragmentVisible
    {
        protected virtual int LayoutResource
        {
            get;
        }
        public virtual void BecameVisible()
        {
            throw new NotImplementedException();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(LayoutResource, container, false);
        }
    }
}