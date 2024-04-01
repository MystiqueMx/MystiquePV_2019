using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace MystiqueNative.Droid.Utils.Views
{
    public class TextInputAutoCompleteTextView : AppCompatAutoCompleteTextView
    {
        protected TextInputAutoCompleteTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TextInputAutoCompleteTextView(Context context) : base(context)
        {
        }

        public TextInputAutoCompleteTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public TextInputAutoCompleteTextView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public override IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
        {
            var ic = base.OnCreateInputConnection(outAttrs);
            if (ic == null || outAttrs.HintText != null) return ic;

            var parent = Parent;
            if (parent is TextInputLayout parentTil)
            {
                outAttrs.HintText = new Java.Lang.String(parentTil.Hint);
            }
            return ic;
        }
    }
}